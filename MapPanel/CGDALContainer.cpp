#pragma once
#include "stdafx.h"
#include "GDALContainer.h"

CGDALContainer::CGDALContainer(int width, int height, double scale)
	{
		GDALAllRegister();

		this->width = width;
		this->height = height;
		this->bufferSize = sizeof(byte)*width*height;
		this->buffer = NULL;
		nDisplayDX = nDisplayDY = 0;
		this->dCurrentScale = scale;
	}

CGDALContainer::~CGDALContainer(void)
	{
		int length = geoTiffs.size();
		for (int i = 0; i<length; i++) {
			delete(geoTiffs[i]);
		}
		delete(buffer);
	}

ImageLayer * CGDALContainer::AddFile(const wchar_t * w_pszFileName)
{
	ImageLayer * il = new ImageLayer();
	il->id = -1;
	il->maxBX = il->maxBY = il->minBX = il->minBY = 0;

	try {
		// convert the wide char file name to locale aware ANSI chars
		const int	newsize			= wcslen(w_pszFileName);
		char	   *pszFileName 	= (char *)malloc(newsize + 1);

		int count = WideCharToMultiByte(CP_UTF8, 0, w_pszFileName, newsize, pszFileName, newsize, NULL, 0);	

		if (count == 0) {
			FILE * pFile = _wfopen(L"Gravur_ErrorLog.log",L"a");
			fwprintf(pFile, L"Error: Der Dateiname \"%s\" konnte nicht nach Const Char * transformiert werden!\n", w_pszFileName);
			fclose(pFile);
    		return il;
		}
		pszFileName[newsize] = '\0'; // one never knows...
		
		GDALDataset  *poDataset = (GDALDataset *) GDALOpen(pszFileName, GA_ReadOnly );

		if (poDataset == NULL) {
			FILE * pFile = _wfopen(L"Gravur_ErrorLog.log",L"a");
			fwprintf(pFile, L"Error: Das GDALDataset der Datei \"%s\" konnte nicht erstellt werden!\n", w_pszFileName);
			fprintf(pFile, "       CPLError: %s\n", (CPLGetLastErrorMsg() == NULL) ? "Null" : CPLGetLastErrorMsg());
			fclose(pFile);
			return il;
		}
	
		int id = geoTiffs.size();
		CGDALGeoTiff* newLayer = new CGDALGeoTiff(poDataset, this->width, this->height, id);
		geoTiffs.push_back(newLayer);
		return geoTiffs.at(id)->getImageLayer();
	}
	catch (std::bad_alloc e) {
		FILE * pFile = _wfopen(L"Gravur_ErrorLog.log",L"a");
		fwprintf(pFile, L"Error: Nicht genügen Speicher vorhanden um die Datei \"%s\" zu laden!\n", w_pszFileName);
		fprintf(pFile, "Message: %s", e.what());
		fclose(pFile);
		return il;
	}
	catch (std::exception e) {
		FILE * pFile = _wfopen(L"Gravur_ErrorLog.log",L"a");
		const char * msg = CPLGetLastErrorMsg();
		fprintf(pFile, "Error: %s (GDAL) %s (Layer)/\n", (msg == NULL) ? "None" : msg, e.what());
		delete(msg);
		fclose(pFile);
		return il;
	}
}


void CGDALContainer::DrawAllImages(HDC hDC, double dWorldOriginX, double dWorldOriginY) {
	int length = geoTiffs.size();
	for (int i = 0; i<length; i++) {
		DrawImage(hDC, dWorldOriginX, dWorldOriginY, i);		
	}
}

void CGDALContainer::DrawImage(HDC hDC, double dWorldOriginX, double dWorldOriginY, uint index) {
	
	if (hDC == NULL) return;
	int		length	= geoTiffs.size();
	HDC		hDCMem	= CreateCompatibleDC(hDC);
	HBITMAP hMemBitmap;
	HBITMAP hbMask;
	HGDIOBJ oldObject = NULL;


	// have we got a device context?
	if (hDCMem) {
		// are we in range?
		if (index >= 0 && index < geoTiffs.size()) {
			
			// draw only if layer is uptodate
			if (geoTiffs[index]->isUptodate) {

				int currentBufferSize = geoTiffs[index]->getBufferSize();
				
				// create new DIBSection for drawing the layer in
				hMemBitmap = ::CreateDIBSection(hDCMem, geoTiffs[index]->getBitmapInfo(), DIB_RGB_COLORS, (void **)&buffer, NULL, 0);
				if (hMemBitmap) {
					
					// copy the layer data into the DIBSection
					// memcpy_s(void *dest, size_t numberOfElements, const void *src, size_t count);
					memcpy_s(this->buffer, currentBufferSize, geoTiffs[index]->getBytes(), currentBufferSize);
					
					// select the DIBSection into temp dc
					oldObject = SelectObject(hDCMem, hMemBitmap);

					// get the mask of the layer
					geoTiffs[index]->getMask(hbMask);

					// blt the drawn layer through the mask onto our real device context
					MaskBlt(hDC, geoTiffs[index]->getRelativeX(), geoTiffs[index]->getRelativeY(), width, height,
						hDCMem, 0, 0, hbMask, 0, 0,
						MAKEROP4(geoTiffs[index]->getRasterOperation(), 0x00AA0029)); // 0x00AA0029 == Destination
					
					// Select old object back into dc (one can not delete an object if it is in the dc)
					SelectObject(hDCMem, oldObject);

					// delete the now unselected object
					DeleteObject(hMemBitmap);
				}
			}
		}
		DeleteDC(hDCMem);
	}
}

void CGDALContainer::SetLayerTransparency(uint layer, bool isTransparent) {
	if (layer >= 0 && layer < geoTiffs.size())
		geoTiffs[layer]->setTransparency(isTransparent);
}

bool CGDALContainer::RecalculateImages(double scale, double dXWorldOffset, double dYWorldOffset) {
	int		length	= geoTiffs.size();
	
	for (int i = 0; i<length; i++)
		RecalculateImage(scale, dXWorldOffset, dYWorldOffset, i);

	return true;
}

void CGDALContainer::setDisplayDelta(int dX, int dY) {
	this->nDisplayDX = dX;
	this->nDisplayDY = dY;
}

void CGDALContainer::OnZoom(double dCurrentZoomFactor, int zoomRecX, int zoomRecY, int zommRecWidth, int zoomRectHeight) {

}

void CGDALContainer::OnScaleChanged(double scale) {
	this->dCurrentScale = scale;
}

bool CGDALContainer::RecalculateImage(double scale, double dXWorldOffset, double dYWorldOffset, uint index) {
	if (index >= 0 && index < geoTiffs.size())
		geoTiffs[index]->recalculateImages(scale, dXWorldOffset, dYWorldOffset, width, height);
	else
		return false;

	return true;
}

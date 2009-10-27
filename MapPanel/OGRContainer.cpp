#include "OGRContainer.h"
#include <ogrsf_frmts/ogrsf_frmts.h>

OGRContainer::OGRContainer(int width, int height) {
	RegisterOGRShape();
	RegisterOGRCSV();
	this->width = width;
	this->height = height;
}

OGRContainer::~OGRContainer(void) {
	int length = layers.size();
	for (int i = 0; i<length; i++) {
		delete(layers[i]);
	}
}

VectorLayerInfo * OGRContainer::AddFile(const wchar_t * w_pszFileName, const wchar_t * w_pszNewSpatialRef) {

	VectorLayerInfo * il = new VectorLayerInfo();
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

		OGRDataSource *poDS;
		poDS = OGRSFDriverRegistrar::Open( pszFileName, FALSE );

		if( poDS == NULL )
		{
			FILE * pFile = _wfopen(L"Gravur_ErrorLog.log",L"a");
			fwprintf(pFile, L"Error: Die OGRDataSource der Datei \"%s\" konnte nicht erstellt werden!\n", w_pszFileName);
			fprintf(pFile, "       Message: %s\n", (CPLGetLastErrorMsg() == NULL) ? "None" : CPLGetLastErrorMsg());
			fclose(pFile);
			return il;
		}

		// check for existense of spatial references

		OGRSpatialReference * sr = poDS->GetLayer(0)->GetSpatialRef();

		char* text;
		
		if (sr == NULL)
			if (w_pszNewSpatialRef == NULL) {
				// we were not able to add a new spatial reference to the data source, quit
				// Either have a .prj file or specify a suitable alternative spatial reference!
				il->id = -2;
				return il;
			} else {

				char * pszFileNameNoExt;
				char ** ppszFileNameNoExt = &pszFileNameNoExt;

				MPGetFileName(pszFileName, ppszFileNameNoExt);
				pszFileNameNoExt = *ppszFileNameNoExt;

				
				sr = poDS->GetLayer(0)->GetSpatialRef();
			}

		sr->exportToWkt(&text);
		OGRFree(text);


		int id = layers.size();
		VectorLayer* newLayer = new VectorLayer(poDS, this->width, this->height, id);
		layers.push_back(newLayer);
		return layers.at(id)->getInfo();

		return NULL;
	}
	catch (std::bad_alloc) {
		return il;
	}
	catch (std::exception) {

		return il;
	}
}

void OGRContainer::DrawAllImages(HDC hDC, double scale, double dWorldOriginX, double dWorldOriginY) {

}

void OGRContainer::DrawImage(HDC hDC, double scale, double dWorldOriginX, double dWorldOriginY, uint index) {
	if (hDC == NULL) return;
	int		length	= layers.size();

	// have we got a device context?
	if (hDC) {
		// are we in range?
		if (index >= 0 && index < length) {
			layers[index]->render(hDC, scale, dWorldOriginX, dWorldOriginY);
		}
	}
}

bool OGRContainer::RecalculateImages(double scale, double dXWorldOffset, double dYWorldOffset) {
	return true;
}

bool OGRContainer::RecalculateImage(double scale, double dXWorldOffset, double dYWorldOffset, uint index) {
	return true;
}
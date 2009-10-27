// MapPanel.cpp : Defines the entry point for the DLL application.
//
#pragma once
#include "stdafx.h"
#include <gdal_priv.h>
//#include <cpl_string.h>
#include "GDALContainer.h"
#include "GDALCoordTransformer.h"
#include "OGRContainer.h"
#include "MandelbrotLayer.h"
#include <windows.h>

BOOL APIENTRY DllMain( HANDLE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	return TRUE;
}

/////////////////////////////////////////////
/// OGRContainer
/////////////////////////////////////////////

__declspec(dllexport) OGRContainer* InitOGR(int width, int height) {
	GDALSetCacheMax(4194304); // limit the GDAL Cache to 4MB (1024*1024*4)
	return new OGRContainer(width, height);
}

__declspec(dllexport) bool CloseOGR(OGRContainer* container) {
	delete(container);
	return true;
}

__declspec(dllexport) void OGRDrawImage(OGRContainer * container, HDC hDC, double scale, double dX, double dY, int index) {
	if(container != NULL) container->DrawImage(hDC, scale, dX, dY, index);
}

__declspec(dllexport) VectorLayerInfo * AddFileToOGRContainer(OGRContainer * container, wchar_t * file, wchar_t * SpatialReference) {
	
	if (container == NULL) {
		VectorLayerInfo* il = new VectorLayerInfo();
		il->id = -1;
		il->maxBX = il->maxBY = il->minBX = il->minBY = 0;
		
		return il;
	} else
		return container->AddFile(file, SpatialReference);
}

/////////////////////////////////////////////
/// GDALContainer
/////////////////////////////////////////////

__declspec(dllexport) CGDALContainer* InitGDAL(int width, int height, double scale) {
	GDALSetCacheMax(4194304); // limit the GDAL Cache to 4MB (1024*1024*4)
	return new CGDALContainer(width, height, scale);
}

__declspec(dllexport) bool CloseGDAL(CGDALContainer* container) {
	delete(container);
	return true;
}

__declspec(dllexport) ImageLayer * AddFileToGDALContainer(CGDALContainer * container, wchar_t * file) {
	ImageLayer* il = new ImageLayer();
	il->id = -1;
	il->maxBX = il->maxBY = il->minBX = il->minBY = 0;

	try {
		if (container == NULL) return il;
		return container->AddFile(file);
	}
	catch (...) {
		return il;
	}
}

__declspec(dllexport) void SetLayerTransparencyWrapper(CGDALContainer * container, int index, bool isTransparent) {
	if (container != NULL) container->SetLayerTransparency(index, isTransparent);
}

__declspec(dllexport) bool WriteImageInformationToFile(wchar_t * imageFile, const wchar_t * infoFile) {
	FILE * pFile = _wfopen(infoFile,L"w");

	/*double adfGeoTransform[6];

	if (poDataset->GetDriver()->GetDescription() != NULL)
		fprintf(pFile, "Driver: %s/",poDataset->GetDriver()->GetDescription());

	if (poDataset->GetDriver()->GetMetadataItem( GDAL_DMD_LONGNAME ) != NULL)
		fprintf(pFile, "%s\n",poDataset->GetDriver()->GetMetadataItem( GDAL_DMD_LONGNAME ));
	
    fprintf(pFile, "Size is %dx%dx%d\n", 
            poDataset->GetRasterXSize(), poDataset->GetRasterYSize(),
            poDataset->GetRasterCount() );

    if( poDataset->GetProjectionRef()  != NULL )
        fprintf(pFile, "Projection is `%s'\n", poDataset->GetProjectionRef() );

    if( poDataset->GetGeoTransform( adfGeoTransform ) == CE_None )
    {
        fprintf(pFile, "Origin = (%.6f,%.6f)\n",
                adfGeoTransform[0], adfGeoTransform[3] );

        fprintf(pFile, "Pixel Size = (%.6f,%.6f)\n",
                adfGeoTransform[1], adfGeoTransform[5] );
    }

	GDALRasterBand  *poBand;
    int             nBlockXSize, nBlockYSize;
    int             bGotMin, bGotMax;
    double          adfMinMax[2];
    
    poBand = poDataset->GetRasterBand( 1 );
    poBand->GetBlockSize( &nBlockXSize, &nBlockYSize );
    fprintf(pFile, "Block=%dx%d Type=%s, ColorInterp=%s\n",
            nBlockXSize, nBlockYSize,
            GDALGetDataTypeName(poBand->GetRasterDataType()),
            GDALGetColorInterpretationName(
                poBand->GetColorInterpretation()) );

    adfMinMax[0] = poBand->GetMinimum( &bGotMin );
    adfMinMax[1] = poBand->GetMaximum( &bGotMax );
    if( ! (bGotMin && bGotMax) )
        GDALComputeRasterMinMax((GDALRasterBandH)poBand, TRUE, adfMinMax);

    fprintf(pFile, "Min=%.3fd, Max=%.3f\n", adfMinMax[0], adfMinMax[1] );
    
    if( poBand->GetOverviewCount() > 0 )
        fprintf(pFile, "Band has %d overviews.\n", poBand->GetOverviewCount() );

    if( poBand->GetColorTable() != NULL )
        fprintf(pFile, "Band has a color table with %d entries.\n", 
                 poBand->GetColorTable()->GetColorEntryCount() );
				 
				 
	const char *pszFormat = "GTiff";
    GDALDriver *poDriver;
    char **papszMetadata;

    poDriver = GetGDALDriverManager()->GetDriverByName(pszFormat);

    if( poDriver == NULL )
        exit( 1 );
	papszMetadata = poDriver->GetMetadata();
    if( CSLFetchBoolean( papszMetadata, GDAL_DCAP_CREATE, FALSE ) )
        fprintf(pFile,  "Driver %s supports Create() method.\n", pszFormat );
    if( CSLFetchBoolean( papszMetadata, GDAL_DCAP_CREATECOPY, FALSE ) )
        fprintf(pFile, "Driver %s supports CreateCopy() method.\n", pszFormat );*/


	fclose(pFile);
	return true;
}

__declspec(dllexport) void GDALDrawAllImages(CGDALContainer * container, HDC hDC, double dX, double dY) {
	if(container != NULL) container->DrawAllImages(hDC, dX, dY);
}

__declspec(dllexport) void GDALDrawImage(CGDALContainer * container, HDC hDC, double dX, double dY, int index) {
	if(container != NULL) container->DrawImage(hDC, dX, dY, index);
}

__declspec(dllexport) void OnPanWrapper(CGDALContainer * container, int dX, int dY) {
	if (container != NULL) container->setDisplayDelta(dX, dY);
}

__declspec(dllexport) void OnZoomWrapper(CGDALContainer * container, double dCurrentZoomFactor, int zoomRecX,
								  int zoomRecY, int zommRecWidth, int zoomRectHeight) {

	if (container != NULL)
		container->OnZoom(dCurrentZoomFactor, zoomRecX, zoomRecY, zommRecWidth, zoomRectHeight);
}

__declspec(dllexport) void OnScaleChangedWrapper(CGDALContainer * container, double scale) {
	if (container != NULL) container->OnScaleChanged(scale);
}

__declspec(dllexport) bool RecalculateImagesWrapper(CGDALContainer * container, double scale, double dXWorldOffset, double dYWorldOffset) {
	if (container != NULL) return container->RecalculateImages(scale, dXWorldOffset, dYWorldOffset);
	else return false;
}

__declspec(dllexport) bool RecalculateImageWrapper(CGDALContainer * container, double scale, double dXWorldOffset, double dYWorldOffset, int index) {
	if (container != NULL) return container->RecalculateImage(scale, dXWorldOffset, dYWorldOffset, index);
	else return false;
}

/////////////////////////////////////////////
/// MandelbrotLayer
/////////////////////////////////////////////

__declspec(dllexport) void DrawMandelbrot(CMandelbrotLayer * mbr, HDC hDC, double dX, double dY,
										  int maxIterations, double xPos, double yPos, double size)
{
	if (hDC == NULL || mbr == NULL) return;
	if (!mbr->initialized) mbr->init(hDC);
	mbr->recalculateImage(dX, dY, maxIterations, xPos, yPos, size);
	mbr->draw(hDC);
}

__declspec(dllexport) CMandelbrotLayer* GetNewMandelbrotLayer(int width, int height) {
	return new CMandelbrotLayer(width, height);
}

/////////////////////////////////////////////
/// GDALCoordTransformer
/////////////////////////////////////////////

__declspec(dllexport) GDALCoordTransformer* GetNewCoordTransformer() {
	return new GDALCoordTransformer();
}

__declspec(dllexport) GKCoord* Transform(GDALCoordTransformer * trans,
										 double x, double y) {
	if (trans != NULL) return trans->GDALTransform(x,y);
	else return NULL;
}

__declspec(dllexport) bool TransformerSetSourceGK_TargetWGS(GDALCoordTransformer * trans,
															int stripe) {
	if (trans != NULL) return trans->setSourceGK_TargetWGS(stripe);
	else return false;
}

__declspec(dllexport) bool TransformerSetSourceWGS_TargetGK(GDALCoordTransformer * trans,
															int stripe) {
	if (trans != NULL) return trans->setSourceWGS_TargetGK(stripe);
	else return false;
}

__declspec(dllexport) void clearSourceWrapper(GDALCoordTransformer * trans) {
	if (trans != NULL) trans->clearSource();
}

__declspec(dllexport) void clearTargetWrapper(GDALCoordTransformer * trans) {
	if (trans != NULL) trans->clearTarget();
}

__declspec(dllexport) bool setTargetTMWrapper(GDALCoordTransformer * trans, double dfCenterLat,
									   double dfCenterLong, double dfScale, double dfFalseEasting,
									   double dfFalseNorthing) {
	if (trans != NULL) return trans->setTargetTM(dfCenterLat, dfCenterLong, dfScale, dfFalseEasting, dfFalseNorthing);
	else return false;
}

__declspec(dllexport) bool setTargetGeoCSWrapper(GDALCoordTransformer * trans,
										  double dfSemiMajor, double dfInvFlattening) {
	if (trans != NULL) return trans->setTargetGeoCS(dfSemiMajor, dfInvFlattening);
	else return false;
}

__declspec(dllexport) bool setTargetTOWGS84Wrapper(GDALCoordTransformer * trans,
										  double dfDX, double dfDY, double dfDZ, double dfEX,
										  double dfEY, double dfEZ, double dfPPM) {
	if (trans != NULL) return trans->setTargetTOWGS84(dfDX,dfDY,dfDZ,dfEX,dfEY,dfEZ,dfPPM);
	else return false;
}

__declspec(dllexport) bool createTrafo_SourceToTargetWrapper(GDALCoordTransformer * trans) {
	if (trans != NULL) return trans->createTrafo_SourceToTarget();
	else return false;

}

__declspec(dllexport) bool setSourceGeoCSWrapper(GDALCoordTransformer * trans) {
	if (trans != NULL) return trans->setSourceGeoCS();
	else return false;
}

__declspec(dllexport) void deleteGKCoordPointerWrapper(GDALCoordTransformer * trans, GKCoord * ptr) {
	if (trans != NULL) trans->DestroyCoordPointer(ptr);
}
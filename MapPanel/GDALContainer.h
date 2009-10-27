#pragma once
#include <list>
#include <gdal_priv.h>
#include "GDALGeoTiff.h"

class __declspec(dllexport) CGDALContainer
{

private:
	std::vector<CGDALGeoTiff*> geoTiffs;
	byte *buffer;
	int width, height;
	int bufferSize;
	int nDisplayDX, nDisplayDY;
	double dCurrentZoomFactor;
	double dCurrentScale;

public:
	explicit CGDALContainer(int width, int height, double scale);
	~CGDALContainer(void);
	ImageLayer * CGDALContainer::AddFile(const wchar_t * w_pszFileName);
	void DrawAllImages(HDC hDC, double dWorldOriginX, double dWorldOriginY);
	void DrawImage(HDC hDC, double dWorldOriginX, double dWorldOriginY, uint index);
	bool RecalculateImages(double scale, double dXWorldOffset, double dYWorldOffset);
	bool RecalculateImage(double scale, double dXWorldOffset, double dYWorldOffset, uint index);
	void setDisplayDelta(int dX, int dY);
	void OnZoom(double dCurrentZoomFactor, int zoomRecX, int zoomRecY, int zommRecWidth, int zoomRectHeight);
	void OnScaleChanged(double scale);
	void SetLayerTransparency(uint layer, bool isTransparent);
};
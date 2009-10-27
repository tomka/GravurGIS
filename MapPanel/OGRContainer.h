#pragma once

#include "VectorLayer.h"
#include <vector>
#include "StdAfx.h"

class OGRContainer {

private:
	std::vector<VectorLayer*> layers;
	int width, height;

public:
	explicit OGRContainer(int width, int height);
	~OGRContainer(void);
	VectorLayerInfo * AddFile(const wchar_t * w_pszFileName, const wchar_t * w_pszNewSpatialRef);
	void DrawAllImages(HDC hDC, double scale, double dWorldOriginX, double dWorldOriginY);
	void DrawImage(HDC hDC, double scale, double dWorldOriginX, double dWorldOriginY, uint index);
	bool RecalculateImages(double scale, double dXWorldOffset, double dYWorldOffset);
	bool RecalculateImage(double scale, double dXWorldOffset, double dYWorldOffset, uint index);
};
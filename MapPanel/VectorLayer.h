#pragma once

#include <ogrsf_frmts/ogrsf_frmts.h>
#include "StdAfx.h"

class VectorLayer {

private:
	int id;
	int width;
	int height;
	OGRDataSource* dataSource;

public:
	VectorLayer(OGRDataSource* dataSource, int width, int height, int id);
	~VectorLayer();
	VectorLayerInfo* getInfo();
	bool recalculateData(double scale, double dXWorldOffset, double dYWorldOffset, int width, int height);
	bool render(HDC hDC, double scale, double dWorldOriginX, double dWorldOriginY);
};
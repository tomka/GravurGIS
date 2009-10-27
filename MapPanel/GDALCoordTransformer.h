#pragma once
#include "stdafx.h"
#include <ogr_spatialref.h>

class GDALCoordTransformer
{
private:
	OGRSpatialReference oSourceSRS, oTargetSRS;
	OGRCoordinateTransformation *poCT;

public:
	GDALCoordTransformer(void);
public:
	~GDALCoordTransformer(void);
	GKCoord * GDALTransform(double x, double y);
	void DestroyCoordPointer(GKCoord *);
	bool setSourceWGS_TargetGK(int stripe);
	bool setSourceGK_TargetWGS(int stripe);
	
	void clearSource();
	void clearTarget();
	bool setSourceGeoCS();
	bool setTargetTM(double dfCenterLat, double dfCenterLong, double dfScale, double dfFalseEasting, double dfFalseNorthing);
	bool setTargetGeoCS(double dfSemiMajor, double dfInvFlattening);
	bool setTargetTOWGS84(double dfDX, double dfDY, double dfDZ, double dfEX = 0.0, double dfEY = 0.0, double dfEZ = 0.0, double dfPPM = 0.0);
	bool createTrafo_SourceToTarget();
};

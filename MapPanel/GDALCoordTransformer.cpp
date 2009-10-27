#include "GDALCoordTransformer.h"

GDALCoordTransformer::GDALCoordTransformer(void)
{
	//oSRS.SetGeogCS( "My geographic coordinate system",
  //                      "WGS_1984", 
  //                      "My WGS84 Spheroid", 
  //                      SRS_WGS84_SEMIMAJOR, SRS_WGS84_INVFLATTENING, 
  //                      "Greenwich", 0.0, 
  //                      "degree", SRS_UA_DEGREE_CONV );

		// EPSG Kennziffern für Gauß Krüger Streifen
		//31466 für GK 2
		//31467 für GK 3
		//31468 für GK 4
		//31469 für GK 5

		//oTargetSRS.SetWellKnownGeogCS( "EPSG:31466" );
		// Gauß Krüger - Zone 2

	// temp uncomment
	poCT = NULL;
	setSourceWGS_TargetGK(2);

	oSourceSRS.Clear();
}

GDALCoordTransformer::~GDALCoordTransformer(void)
{
	delete poCT;
}

bool GDALCoordTransformer::setTargetTM(double dfCenterLat, double dfCenterLong,
											double dfScale, double dfFalseEasting, double dfFalseNorthing) {
	OGRErr err = -1;
	err = oTargetSRS.SetTM(dfCenterLat, dfCenterLong, dfScale, dfFalseEasting, dfFalseNorthing);
	
	return (err == 0);
}

// set up the underlying geographich coordinate system
bool GDALCoordTransformer::setTargetGeoCS(double dfSemiMajor, double dfInvFlattening) {

	OGRErr err = -1;
	err = oTargetSRS.SetGeogCS("MyGEOGCS", "MyDatum", "MyEllipsoid", dfSemiMajor,  dfInvFlattening );
	
	return (err == 0);
}

// set up the helmet parameters
bool GDALCoordTransformer::setTargetTOWGS84(double dfDX, double dfDY, double dfDZ, double dfEX, double dfEY, double dfEZ, double dfPPM) {
	
	OGRErr err = -1;
	err = oTargetSRS.SetTOWGS84(dfDX,dfDY,dfDZ,dfEX,dfEY,dfEZ,dfPPM);

	return (err == 0);
}

// clear the target SRS
void GDALCoordTransformer::clearTarget() {
	oTargetSRS.Clear();
}

// clear the target SRS
void GDALCoordTransformer::clearSource() {
	oSourceSRS.Clear();
}

// set up the coordinate transformation
bool GDALCoordTransformer::createTrafo_SourceToTarget() {
	if (poCT != NULL) delete poCT;
	poCT = OGRCreateCoordinateTransformation( &oSourceSRS , &oTargetSRS );

	if (poCT == NULL) return false;
	
	return true;
}

bool GDALCoordTransformer::setSourceGeoCS() {

	OGRErr err = -1;
	err = oSourceSRS.SetWellKnownGeogCS( "WGS84" );

	return (err == 0);
}


bool GDALCoordTransformer::setSourceWGS_TargetGK(int stripe){
	oSourceSRS.Clear();
	oTargetSRS.Clear();

	OGRErr err = -1;
	if (stripe == 2) err = oTargetSRS.SetTM(0.0, 6.0, 1.0, 2500000.0, 0.0);
	else if (stripe == 3) err = oTargetSRS.SetTM(0.0, 9.0, 1.0, 3500000.0, 0.0);
	else if (stripe == 4) err = oTargetSRS.SetTM(0.0, 12.0, 1.0, 4500000.0, 0.0);
	else if (stripe == 5) err = oTargetSRS.SetTM(0.0, 15.0, 1.0, 5500000.0, 0.0);
	else return false;

	// set up the underlying geographich coordinate system
	err = oTargetSRS.SetGeogCS("MyGEOGCS", "MyDatum", "Bessel 1841", 6377397.155,  299.1528128 );
	
	// set up the helmet parameters
	err = oTargetSRS.SetTOWGS84(582.0,105.0,414.0,1.04,0.35,-3.08,8.3);
	
	err = oSourceSRS.SetWellKnownGeogCS( "WGS84" );

	if (poCT != NULL) delete poCT;
	poCT = OGRCreateCoordinateTransformation( &oSourceSRS, &oTargetSRS );

	return !(poCT == NULL);
}
bool GDALCoordTransformer::setSourceGK_TargetWGS(int stripe){
	oSourceSRS.Clear();
	oTargetSRS.Clear();

	oSourceSRS.SetProjCS("GK / WGS84");
	oSourceSRS.SetWellKnownGeogCS( "WGS84" );
	if (stripe == 2) oSourceSRS.SetTM(0.0, 6.0, 1.0, 2500000.0, 0.0);
	else if (stripe == 3) oSourceSRS.SetTM(0.0, 9.0, 1.0, 3500000.0, 0.0);
	else if (stripe == 4) oSourceSRS.SetTM(0.0, 12.0, 1.0, 4500000.0, 0.0);
	else if (stripe == 5) oSourceSRS.SetTM(0.0, 15.0, 1.0, 5500000.0, 0.0);
	else return false;

	oTargetSRS.SetTOWGS84(-582.0,-105.0,-414.0,-1.04,-0.35,3.08,-8.3);

	//oTargetSRS.SetWellKnownGeogCS( "WGS84" );
	oTargetSRS = *(oSourceSRS.CloneGeogCS());

	if (poCT != NULL) delete poCT;
	poCT = OGRCreateCoordinateTransformation( &oSourceSRS, &oTargetSRS );

	if (poCT == NULL) {
		//MessageBoxW(NULL, L"Null", L"mist", MB_OK);
		return false;
	}

	return true;
}

GKCoord * GDALCoordTransformer::GDALTransform(double x2, double y2) {

		GKCoord * gkCoord = new GKCoord();
		gkCoord->easting = y2;
		gkCoord->northing = x2;

		if( poCT == NULL || !poCT->Transform( 1, &(gkCoord->easting), &(gkCoord->northing) ) ) {
			// TODO: Fehlerbehandlung
			gkCoord->state = 1;
            return gkCoord;
		}
		else {
			gkCoord->state = 0;
            return gkCoord;
		}
}

void GDALCoordTransformer::DestroyCoordPointer(GKCoord * ptr) {
	if (ptr != NULL)
		delete ptr;
}
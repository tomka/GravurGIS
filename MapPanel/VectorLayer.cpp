#include "VectorLayer.h"
#include <string>
#include "Timer.h"

VectorLayer::VectorLayer(OGRDataSource* dataSource, int width, int height, int id) {
	this->dataSource = dataSource;
	this->width = width;
	this->height = height;
	this->id = id;

	char * pszFileNameNoExt;
	char ** ppszFileNameNoExt = &pszFileNameNoExt;

	MPGetFileName(dataSource->GetName(), ppszFileNameNoExt);
	pszFileNameNoExt = *ppszFileNameNoExt;

	std::string * query = new std::string("CREATE SPATIAL INDEX ON ");
	query->append(pszFileNameNoExt);

	OGRLayer * result = dataSource->ExecuteSQL(query->c_str(), NULL , "");

	delete query;
}

VectorLayer::~VectorLayer() {
	OGRDataSource::DestroyDataSource( dataSource );
}
VectorLayerInfo* VectorLayer::getInfo() {
	VectorLayerInfo * vl = new VectorLayerInfo();
	vl->id = this->id;

	OGREnvelope * envelope = new OGREnvelope();
	this->dataSource->GetLayer(0)->GetExtent(envelope);

	vl->minBX = envelope->MinX;
	vl->minBY = envelope->MinY;
	vl->maxBX = envelope->MaxX;
	vl->maxBY = envelope->MaxY;

	return vl;
}

bool VectorLayer::render(HDC hDC, double scale, double dWorldOriginX, double dWorldOriginY) {

	Timer * timer = new Timer();
	timer->start();
	OGRLayer  *poLayer;

	poLayer = dataSource->GetLayer(0);

	poLayer->SetSpatialFilterRect(dWorldOriginX, dWorldOriginY - height / scale, dWorldOriginX + width / scale, dWorldOriginY);

	OGRFeature *poFeature;

    poLayer->ResetReading();

	HGDIOBJ oldObject = SelectObject(hDC, GetStockObject(BLACK_BRUSH));

    while( (poFeature = poLayer->GetNextFeature()) != NULL )
    {
		OGRGeometry *poGeometry;

        poGeometry = poFeature->GetGeometryRef();
		if( poGeometry != NULL) {

			//OGRwkbGeometryType type = wkbFlatten(poGeometry->getGeometryType());
			OGRwkbGeometryType type = poGeometry->getGeometryType();
			
			if (type == wkbPoint )
			{
				OGRPoint *poPoint = static_cast<OGRPoint *>(poGeometry);
			}
			else if (type == wkbLineString)
			{
				OGRLineString *poLineString = static_cast<OGRLineString *>(poGeometry);

				int numPoints = poLineString->getNumPoints();
				POINT * points = (POINT *)CPLMalloc(sizeof(POINT) * numPoints);

				if (points == NULL) return false;

				for (int i = 0; i < numPoints; ++i) {
					points[i].x = (LONG)((poLineString->getX(i) - dWorldOriginX) * scale + 0.5);
					points[i].y = (LONG)((dWorldOriginY - poLineString->getY(i)) * scale + 0.5);
				}

				Polyline(hDC, points, numPoints);

				free(points);
			}

			OGRFeature::DestroyFeature( poFeature );
		}
    }

	// Select old object back into dc (one can not delete an object if it is in the dc)
	SelectObject(hDC, oldObject);
	
	timer->stop();
	double timeTaken = timer->diffTime();
	MessageBox(0,doubleToString(timeTaken).c_str(),L"Time to render",MB_OK);

	return true;
}
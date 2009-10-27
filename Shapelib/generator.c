//#define MSGBOX_DEBUG
#include "generator.h"

PointObject SHPAPI_CALL1(*)
SHPGetPolyLineList( const wchar_t * pszLayer, const wchar_t * pszAccess,
				   int mapPanelWidth, int mapPanelHeight, int margin, BOOL recalculateBB,
				   double oldScale, BOOL useOldScale,int pointSize, BOOL getRawData)
{
    SHPHandle   psSHP;
	SHPObject	*shpObject;
	PointObject *pntObject;
	double		lMinB[4];
	double		lMaxB[4];
	double		tempScale, height;
	int			i, j, nrOfElements, momentaryPointCount,
				nParts, nVertices, currPoint, vertexCount = 0,
				partListCount = 0,	pointListCount = 0;
	double		tempMinX = 0, tempMinY = 0, tempHeight = 0;

	int partCount	= 0;		/* this variable will be used for the first  dimension */
	int pointCount	= 0;		/* this variable will be used for the second dimension */

	// Allocate and minimally initialize the object
	pntObject		= (PointObject *) calloc(1,sizeof(PointObject));
	psSHP			= (SHPHandle) calloc(1,sizeof(SHPInfo));
    psSHP->bUpdated = FALSE;
	

	psSHP = SHPOpenW( pszLayer, pszAccess);
	if (psSHP == NULL) {
		return( NULL );
	}
	
	// Do the GetInfo() on our own :)
	pntObject->nSHPType = psSHP->nShapeType;
	
	// Get the extends
	for( i = 0; i < 4; i++ )
    {
            lMinB[i] = psSHP->adBoundsMin[i];
            lMaxB[i] = psSHP->adBoundsMax[i];
    }
	
	nrOfElements	= psSHP->nRecords;

	// allocate memory for our arrayin the size of psSHP->nRecords
	// QUESTION: Is it better to get all ShpObjects now into one lst or not
	shpObject		= (SHPObject*) malloc(nrOfElements*sizeof(SHPObject));
	
	// if we want to, we can check if the given scale (based on the extend-information)
	// in den shp-file is correct and correct it if needed
	if (recalculateBB == 1) {
		
		lMinB[0] = lMinB[1] = lMaxB[0] = lMaxB[1] = 0.0;

		if (nrOfElements > 0) {
			
			shpObject[0] = *SHPReadObject(psSHP, 0);

			lMinB[0] = shpObject[0].dfXMin;
			lMinB[1] = shpObject[0].dfYMin;
			lMaxB[0] = shpObject[0].dfXMax;
			lMaxB[1] = shpObject[0].dfYMax;

			partCount   = shpObject[0].nParts;
			vertexCount = shpObject[0].nVertices;
			
			for (i = 1; i < nrOfElements; i++)
			{
				shpObject[i] = *SHPReadObject(psSHP, i);

				partCount   += shpObject[i].nParts;
				vertexCount += shpObject[i].nVertices;

				// check if we must recalculate the bounding box of our shapefile
				if (shpObject[i].dfXMin < lMinB[0])
					lMinB[0] = shpObject[i].dfXMin;
				if (shpObject[i].dfXMax > lMaxB[0])
					lMaxB[0] = shpObject[i].dfXMax;
				if (shpObject[i].dfYMin < lMinB[1])
					lMinB[1] = shpObject[i].dfYMin;
				if (shpObject[i].dfYMax > lMaxB[1])
					lMaxB[1] = shpObject[i].dfYMax;
			}

				// if at least one shape was outside of the boudingbox recalculate the scale
			if (oldScale == 0)
				pntObject->scale = calculateScale(lMinB, lMaxB,
				mapPanelWidth  - 2*(margin) - 2,
				mapPanelHeight  - 2*(margin) - 2);

		}
	
	} else { // even if we do not use recalculateBB we have to find out how many parts are there in total (nParts * resp. shapes)
		for (i = 0; i < nrOfElements; i++)
        {
            shpObject[i] = *SHPReadObject(psSHP, i);
            partCount	+= shpObject[i].nParts;
			vertexCount += shpObject[i].nVertices;
        }

	}

	// calculate the scale if this is the first layer
	if (useOldScale)
		pntObject->scale = oldScale;
	else
	{
		if (nrOfElements == 0) {
			pntObject->isPoint = FALSE;
			pntObject->scale = 1.0;

		} else if ((lMaxB[0]-lMinB[0]==0)&&(lMaxB[1]-lMinB[1]==0))
		{
			pntObject->isPoint = TRUE;
			pntObject->scale = 1.0;
		}
		else
		{
			pntObject->isPoint = FALSE;
			pntObject->scale = calculateScale(lMinB, lMaxB,
			mapPanelWidth  - 2*(margin) - 2,
			mapPanelHeight  - 2*(margin) - 2);
			
			// tom: Das sollten wir vielleicht nicht so machen da die Punktgröße ja nur
			// für die Anzeige relevant ist aber nicht in den Daten vorliegt
			//if ((pntObject->nSHPType == SHPT_MULTIPOINT) || (pntObject->nSHPType == SHPT_MULTIPOINTZ)) //change boundings depending on pointsize and size of shapeObject
			//{
			//	lMinB[0] -= pointSize/pntObject->scale;
			//	lMinB[1] -= pointSize/pntObject->scale;
			//	lMaxB[0] += pointSize/pntObject->scale;
			//	lMaxB[1] += pointSize/pntObject->scale;
			//}
			pntObject->scale = calculateScale(lMinB, lMaxB,
				mapPanelWidth  - 2*(margin) - 2,
				mapPanelHeight  - 2*(margin) - 2); //recalculate scale with new boundings
			
		}
	}

	pntObject->minBX = lMinB[0];
	pntObject->minBY = lMinB[1];
	pntObject->maxBX = lMaxB[0];
	pntObject->maxBY = lMaxB[1];

	height = (lMaxB[1] - lMinB[1])  * pntObject->scale;
	
	if (nrOfElements > 0) {

		//////////////////////////////
		// build up the data structure
		//////////////////////////////

		// dimension and allocate the outer array with length of total vertices in this shape
		pntObject->x = malloc(vertexCount * sizeof(double)); // double for world coordinates
		pntObject->y = malloc(vertexCount * sizeof(double));
		pntObject->dispX = malloc(vertexCount * sizeof(int)); // int for display coordinates
		pntObject->dispY = malloc(vertexCount * sizeof(int));
		pntObject->partLengths = malloc(partCount * (sizeof(int)));
		pntObject->nVertices = vertexCount;
		
		if (pntObject->x == NULL || pntObject->y == NULL ||
			pntObject->partLengths == NULL ||
			pntObject->dispX == NULL || pntObject->dispY == NULL ){
			return NULL;
		}

		tempScale = pntObject->scale;

		if (getRawData) {
			tempMinX = tempMinY = tempHeight = 0.0;
		} else {
			tempMinX = lMinB[0];
			tempMinY = lMinB[1];
			tempHeight = height;
		}

		if(    (pntObject->nSHPType != SHPT_MULTIPOINT)
			&& (pntObject->nSHPType != SHPT_MULTIPOINTZ)
			&& (pntObject->nSHPType != SHPT_MULTIPOINTM)
			&& (pntObject->nSHPType != SHPT_POINT)
			&& (pntObject->nSHPType != SHPT_POINTM)
			&& (pntObject->nSHPType != SHPT_POINTZ))
			{
			pntObject->partCount = partCount;
			pntObject->shapeCount = nrOfElements;
			
			if (nrOfElements == partCount) pntObject->isWellDefined = TRUE;
			else pntObject->isWellDefined = FALSE;

			for (i = 0; i < nrOfElements; i++)
			{
				nParts = shpObject[i].nParts;
				nVertices = shpObject[i].nVertices;
				
				if (shpObject[i].nSHPType != SHPT_NULL) {
					for (j = 0; j < nParts; j++) {

						//find out how many points are in here...
						if (j != (nParts - 1))
						   momentaryPointCount = shpObject[i].panPartStart[j + 1] - shpObject[i].panPartStart[j];
						else
						   momentaryPointCount = nVertices - shpObject[i].panPartStart[j];

						pntObject->partLengths[partListCount] = momentaryPointCount;

						// actually we should check here if malloc did what it should by testing on NULL
						// but we let it be due to performance


						// add new points
						for (currPoint = 0; currPoint < momentaryPointCount; currPoint++)
						{
							int shapeDataIndex = shpObject[i].panPartStart[j] + currPoint;

							pntObject->x[pointListCount] = shpObject[i].padfX[shapeDataIndex];
							pntObject->dispX[pointListCount] = (int)(
								(pntObject->x[pointListCount] - tempMinX) * tempScale + 0.5); // the 0.5 is for correct rounding
							

							pntObject->y[pointListCount] = shpObject[i].padfY[shapeDataIndex];

							// we need to substract from the objects height as we like to mirror horizontally
							pntObject->dispY[pointListCount] = (int)(
								tempHeight - ((pntObject->y[pointListCount]
								- tempMinY) * tempScale) + 0.5); // the 0.5 is for correct rounding
							
							// generate the bounding box for this part:
							pointListCount++;
						}
						partListCount++;
					}
				} else {
					pntObject->partCount = partCount;
					pntObject->shapeCount = 0;
					pntObject->isPoint = FALSE;
					pntObject->maxBX = pntObject->maxBY = pntObject->minBX = pntObject->minBY = 0.0;

					// In this case we have a Null-Shape (which does not consist of further parts and
					// is often ment as a place-holder. We just ignore this kind of shape.

					// MessageBox(NULL, L"TODO: Handle 0-Parts without info!", L"Shapelib.dll", MB_OK);
					// return NULL;
				}
			}
		}
		else //nShapeType == Multipoint
		{
			pntObject->nSHPType = SHPT_MULTIPOINT;
			pntObject->partCount = nrOfElements;
			pntObject->shapeCount = nrOfElements;
			pntObject->isWellDefined = TRUE;

			for (i = 0; i < nrOfElements; i++) //nrOfElements == 4
			{
					nParts = shpObject[i].nParts; //nParts == 0
					nVertices = shpObject[i].nVertices; //nVertices == 1
					
					pntObject->x[i] = shpObject[i].padfX[0];
					pntObject->dispX[i] = (int)(
						((pntObject->x[i] - tempMinX) * tempScale + 0.5)); // the 0.5 is for correct rounding

					// we need to substract from the objects height as we like to mirror horizontally
					pntObject->y[i] = shpObject[i].padfY[0];
					pntObject->dispY[i] = (int)(
						tempHeight - ((pntObject->y[i] - tempMinY) * tempScale) + 0.5); // the 0.5 is for correct rounding
			}	
		}
	}

	SHPClose(psSHP);
	free(psSHP);
	free(shpObject);

	return pntObject;
}

double calculateScale(double *minB, double *maxB, int drawingAreaWidth, int drawingAreaHeight)
{
    double scale;
	double width = maxB[0] - minB[0];
    double height = maxB[1] - minB[1];
	
    // Find longest side of shapefile bouding box
    if (width > height)
        scale = drawingAreaWidth / width;
    else
        scale = drawingAreaHeight / height;

	return scale;
}

void SHPAPI_CALL deletePointObject(PointObject* po) {

	if (po != NULL) {
		if (po->x != NULL) {
			free(po->x);
		}
		if (po->y != NULL) {
			free(po->y);
		}
		if (po->dispX !=  NULL) {
			free(po->dispX);
		}
		if (po->dispY !=  NULL) {
			free(po->dispY);
		}
		free(po);
	}
}

SHPTree * getQuadTree(const wchar_t * pszLayer, const wchar_t * pszAccess) {
	
	SHPTree * tree		= (SHPTree*) calloc(1, sizeof(SHPTree));
	SHPHandle   psSHP	= (SHPHandle) calloc(1,sizeof(SHPInfo));
    psSHP->bUpdated		= FALSE;

	psSHP				= SHPOpenW( pszLayer, pszAccess);

	if (psSHP == NULL) {
		return( NULL );
	}

	tree = SHPCreateTree(psSHP,2, 0, NULL, NULL);
	
	return(tree);
}

#pragma once
#include "shapefil.h"
#include <windows.h>
#include <stdio.h>

#ifndef MAX
#  define MIN(a,b)      ((a<b) ? a : b)
#  define MAX(a,b)      ((a>b) ? a : b)
#endif

typedef struct tagPointObject
{
    double		*x;
	double		*y;
	int			*dispX;
	int			*dispY;
	int			*partLengths;	// an array with the lengths of every part 
	double		scale;
	int			isPoint;
	int			nSHPType; 
	int			partCount;
	int			nVertices;
	int			shapeCount;
	double		minBX;
    double		minBY;
    double		maxBX;
    double		maxBY;
	BOOL		isWellDefined; // true (=1) if there is a one to one erlation between parts and shapes
} PointObject;


PointObject SHPAPI_CALL1(*) SHPGetPolyLineList( const wchar_t * pszLayer, const wchar_t * pszAccess,
				   int mapPanelWidth, int mapPanelHeight, int margin,
				   BOOL recalculateBB, double oldScale, BOOL useOldScale,
				   int pointSize, BOOL getRawData);

double calculateScale		(double *minB, double *maxB, int drawingAreaWidth, int drawingAreaHeight);
SHPTree SHPAPI_CALL1(*) getQuadTree(const wchar_t * pszLayer, const wchar_t * pszAccess);

void SHPAPI_CALL deletePointObject(PointObject* po);
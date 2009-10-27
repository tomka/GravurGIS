#pragma once
#include <gdal_priv.h>

class CGDALGeoTiff
{
private:
	BYTE *			pafByte;
	GDALDataset*	pDataset;
	GDALColorTable *colorTable;
	HPALETTE *		palette;
	HBITMAP			hbmMask;
	BITMAPINFO *	bmi;
	double			pixelSize[2];
	double			dOriginX, dOriginY;		
	int				dRelativeX, dRelativeY;
	int				bufferSize;
	int				iRelativeWidth, iRelativeHeight;
	int				id;
	bool			hasColorTable;
	DWORD			rasterOperation;
	int *			bands;

public:
	CGDALGeoTiff(GDALDataset*, int width, int height, int id);
	~CGDALGeoTiff(void);
	GDALDataset* getDataset();
	ImageLayer * getImageLayer();
	int getOriginalWidth();
	int getOriginalHeight();
	int getRelativeX();
	int getRelativeY();
	int getRelativeWidth();
	int getRelativeHeight();
	byte* getBytes();
	int getBufferSize();
	HPALETTE * getPalette();
	GDALColorTable * getColorTable();
	bool recalculateImages(double scale, double dXWorldOffset, double dYWorldOffset, int width, int height);
	bool isUptodate;
	void getMask(HBITMAP &mask);
	BITMAPINFO * getBitmapInfo();
	void setTransparency(bool isTransparent);
	DWORD getRasterOperation();
};
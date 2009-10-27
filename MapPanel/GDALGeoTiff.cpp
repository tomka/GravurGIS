#include "stdafx.h"
#include "GDALGeoTiff.h"
#include <new>

CGDALGeoTiff::CGDALGeoTiff(GDALDataset* pDataset, int width, int height, int id)
{
	this->pDataset = pDataset;
	int nBands = pDataset->GetRasterCount();
	this->bufferSize = sizeof(byte) * width * height * nBands;
	this->pafByte = (byte *) CPLMalloc(bufferSize);
	if (this->pafByte == NULL) {
		throw new std::bad_alloc("Not enough memory to create layer.");
	}

	/* get color table information */
	GDALRasterBand * poBand = pDataset->GetRasterBand(1);

	if( poBand == NULL) {
		throw new std::domain_error("No raster bands found");
	}

	this->colorTable = poBand->GetColorTable();
	this->hasColorTable = (this->colorTable != NULL);

	this->isUptodate = true;

	this->id = id;
	this->rasterOperation = SRCAND; /* make GeoTiff default transparent (otherwise it would be SRCCOPY) */
	double * adfGeoTransform = new double[6];
	
	if (pDataset->GetGeoTransform(adfGeoTransform) == CE_None) // TODO: be aware of not north facing images! (use formula)
    {
		this->dOriginX		= adfGeoTransform[0];
		this->dOriginY		= adfGeoTransform[3];
		this->pixelSize[0]	= adfGeoTransform[1];
		if (pixelSize[0] < 0) pixelSize[0] = -pixelSize[0];
		this->pixelSize[1]	= adfGeoTransform[5];
		if (pixelSize[1] < 0) pixelSize[1] = -pixelSize[1];
	} else {
		this->pixelSize[0]	= 1;
		this->pixelSize[1]	= 1;
		this->dOriginX		= 0;
		this->dOriginY		= 0;
		MessageBoxW(NULL, L"Es wurden keine Referenzierungsdaten gefunden!\nDas Bild wird an Stelle (0,0) mit seiner Originalgröße platziert.", L"Keine Referenzierung", MB_OK);
	}

	dRelativeX = dRelativeY = 0;

	this->hbmMask = CreateBitmap(width, height, 1, 1, NULL);

	/* create palette - currently only 8bit palette support */

	int countEntries = this->hasColorTable ? colorTable->GetColorEntryCount() : 0;
	this->bmi = (BITMAPINFO*) malloc(sizeof(BITMAPINFOHEADER) + countEntries * sizeof(RGBQUAD));

	::ZeroMemory(&(bmi->bmiHeader), sizeof(BITMAPINFOHEADER)); /* default everything to zero */
	bmi->bmiHeader.biWidth = width;
	bmi->bmiHeader.biHeight = -(height); /*negative indicate top down, not bottom up */
	bmi->bmiHeader.biPlanes = 1;
	bmi->bmiHeader.biBitCount = nBands * 8;
	bmi->bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
	bmi->bmiHeader.biCompression = BI_RGB;
	bmi->bmiHeader.biSizeImage = bmi->bmiHeader.biWidth * bmi->bmiHeader.biHeight * bmi->bmiHeader.biBitCount / 8;
	bmi->bmiHeader.biClrUsed = bmi->bmiHeader.biClrImportant = countEntries;

	for (int j = 0; j<countEntries; j++) {
		const GDALColorEntry * entry = colorTable->GetColorEntry(j);
		bmi->bmiColors[j].rgbRed   = static_cast<BYTE>(entry->c1); // (j*80)%255; //
		bmi->bmiColors[j].rgbGreen = static_cast<BYTE>(entry->c2); // (j*80)%255; //
		bmi->bmiColors[j].rgbBlue  = static_cast<BYTE>(entry->c3); // (j*80)%255; //
		bmi->bmiColors[j].rgbReserved = 0;
	}

	// make an array of band numbers which represent the order of which bands get drawn
	bands = (int*) CPLMalloc(nBands * sizeof(int));
	for (int band = nBands - 1; band >= 0; --band) {
		bands[band] = nBands - band;
	}
}

CGDALGeoTiff::~CGDALGeoTiff(void)
{
	GDALClose(this->pDataset);
	delete(this->pafByte);
	delete(this->pDataset);
}

void CGDALGeoTiff::setTransparency(bool isTranspatent) {

	if (isTranspatent) this->rasterOperation = SRCAND;
	else this->rasterOperation = SRCCOPY;
}

DWORD CGDALGeoTiff::getRasterOperation() {
	return this->rasterOperation;
}

ImageLayer * CGDALGeoTiff::getImageLayer() {
	ImageLayer * il = new ImageLayer();
	il->id = this->id;
	il->minBX = this->dOriginX;
	il->minBY = this->dOriginY - this->pDataset->GetRasterBand( 1 )->GetYSize() * this->pixelSize[1];
	il->maxBX = this->dOriginX + this->pDataset->GetRasterBand( 1 )->GetXSize() * this->pixelSize[0];
	il->maxBY = this->dOriginY;
	il->resolutionX = this->pixelSize[0];
	il->resolutionY = this->pixelSize[1];

	return il;
}

byte* CGDALGeoTiff::getBytes() {
	return this->pafByte;
}

int CGDALGeoTiff::getBufferSize() {
	return this->bufferSize;
}

GDALDataset* CGDALGeoTiff::getDataset() {
	return this->pDataset;
}

int CGDALGeoTiff::getOriginalWidth() {
	return this->pDataset->GetRasterBand( 1 )->GetXSize();
}
int CGDALGeoTiff::getOriginalHeight() {
	return this->pDataset->GetRasterBand( 1 )->GetYSize();
}

GDALColorTable* CGDALGeoTiff::getColorTable() {
	return this->colorTable;
}

int CGDALGeoTiff::getRelativeX() {
	return this->dRelativeX;
}
int CGDALGeoTiff::getRelativeY() {
	return this->dRelativeY;
}

// dXDisplayOffset = dOriginX - (dX / currentScale) und analog für dYDisplayOffset
// width, height = Clientsize width, height
// scale = currentScale
//
// World:   Gauß Krüger in Shape files / GeoTiffs
// Pixel:   Raster-Pixel im GeoTiff
// Display: Display Koordinaten auf PDA

bool CGDALGeoTiff::recalculateImages(double scale, double currentViewWorldX, double currentViewWorldY, int width, int height) {

	int nBands = this->pDataset->GetRasterCount();
	GDALRasterBand * poBand = this->pDataset->GetRasterBand(1);

	// Höhe und Breite des Bitmaps
	int pixelWidth = poBand->GetXSize();
	int pixelHeight = poBand->GetYSize();

	// Was sehen wir gerade alles? -> width/height : scale
	double currentViewWorldWidth = width / scale;
	double currentViewWorldHeight = height / scale;

	// Wie breit ist unser Layer in WeltKoordinaten?
	double widthWorld  = pixelWidth * this->pixelSize[0];
	double heihgtWorld = pixelHeight * this->pixelSize[1];
	::ZeroMemory(this->pafByte, bufferSize);		// notwendig?


	// Sehen wir überhaupt etwas von dem Layer?
	if ((this->dOriginX > (currentViewWorldX + currentViewWorldWidth))
		||
		(this->dOriginX < (currentViewWorldX - widthWorld))
		||
		(this->dOriginY > (currentViewWorldY + heihgtWorld))
		||
		(this->dOriginY < (currentViewWorldY - currentViewWorldHeight)))
	{
			this->isUptodate = false;	// da wir ja nichts neu berechnen
			return true;				// -> wir sehen nix
	}

	/* -> wir sehen was! */


	// Der Untschied zw. ViewPort und Image
	double worldDifX = this->dOriginX - currentViewWorldX;
	double worldDifY = currentViewWorldY - this->dOriginY;

	// Die Höhe und Breite des auszuschneidenden Teils
	int pixelCutXSize = MIN(pixelWidth, static_cast<int>(currentViewWorldWidth  / pixelSize[0]));
	int pixelCutYSize = MIN(pixelHeight, static_cast<int>(currentViewWorldHeight / pixelSize[1]));
	
	int iDisplayCutWidth = MIN(width, static_cast<int>(pixelWidth * pixelSize[0] * scale));
	int iDisplayCutHeight =MIN(height, static_cast<int>(pixelHeight * pixelSize[1] * scale));

	double pixelXOffset = 0;
	if (worldDifX < 0) {
		pixelXOffset = widthWorld - pixelCutXSize * pixelSize[0];
		if (currentViewWorldX <= (this->dOriginX + pixelXOffset)) {
			pixelXOffset = currentViewWorldX - this->dOriginX;
			if(worldDifX - currentViewWorldWidth + widthWorld > 0) this->dRelativeX = 0;
		}
		else /* if (pixelXOffset != currentViewWorldX) */
			this->dRelativeX = static_cast<int>((worldDifX + pixelXOffset) * scale);

		pixelXOffset = pixelXOffset / this->pixelSize[0];
	} else {
		this->dRelativeX = static_cast<int>(worldDifX * scale);
	}

	double pixelYOffset = 0;
	if (worldDifY < 0) { // ist kleiner 0 wenn ViewPort-Oberkante unter Bildoberkante
		pixelYOffset = heihgtWorld - pixelCutYSize * pixelSize[1];
		if (currentViewWorldY >= (this->dOriginY - pixelYOffset)) { //pixelYOffset ist noch in Welt-Koordinaten!
			pixelYOffset = this->dOriginY - currentViewWorldY;
			if(worldDifY - currentViewWorldHeight + heihgtWorld > 0) this->dRelativeY = 0;
		}
		else /* if (pixelYOffset != currentViewWorldY) */
			this->dRelativeY = static_cast<int>((worldDifY + pixelYOffset) * scale);
		
		pixelYOffset = pixelYOffset / this->pixelSize[1];
	} else
		this->dRelativeY = static_cast<int>(worldDifY * scale);

	// Das Spiegeln hat zur Folge dass das Bild nicht mehr an der Richtigen Stelle ausgelesen wird.
	// Das Verschieben muss anders gemacht werden!

	int nXBlockSize, nYBlockSize;
	poBand->GetBlockSize( &nXBlockSize, &nYBlockSize ); // TODO: Actually use the blocksize to read in multiples of it - is said to be faster
	CPLErr ret;
	/*
	for (int band = 1; band <= nBands; band++) {
		poBand = this->pDataset->GetRasterBand(band);

		// PERFORMANCE: Do the lines below make things faster?
		//CPLErr ret =  poBand->AdviseRead( static_cast<int>(pixelXOffset), static_cast<int>(pixelYOffset),
		//pixelCutXSize, pixelCutYSize, iDisplayCutWidth*nBands, iDisplayCutHeight*nBands, GDT_Byte, NULL );

		GDALColorInterp ci = poBand->GetColorInterpretation();
		int offset = 0;

		if (ci == GCI_RedBand) {
			offset = 2;
		} else if (ci == GCI_GreenBand) {
			offset = 1;
		} else if (ci == GCI_BlueBand || ci == GCI_PaletteIndex) {
			offset = 0;
		}

		ret =  poBand->RasterIO( GF_Read, static_cast<int>(pixelXOffset), static_cast<int>(pixelYOffset),
			pixelCutXSize, pixelCutYSize, this->pafByte + offset, iDisplayCutWidth,
			iDisplayCutHeight, GDT_Byte, GDT_Byte * nBands, width * nBands );
	}
	*/

	// alternative, to the last for loop
	// PERFORMANCE: Is this faster? Probably not - since Frank Warmerdam said internally is often the per band RasterIO called
	ret = this->pDataset->RasterIO( GF_Read, static_cast<int>(pixelXOffset), static_cast<int>(pixelYOffset),
			pixelCutXSize, pixelCutYSize, this->pafByte, iDisplayCutWidth, iDisplayCutHeight, GDT_Byte,
			nBands, bands, GDT_Byte * nBands, width * nBands, GDT_Byte);

	// Create Mask
	HDC hdcMemMask = CreateCompatibleDC(NULL);

	SelectObject(hdcMemMask, hbmMask);
	SelectObject(hdcMemMask, GetStockObject(BLACK_BRUSH));
	Rectangle(hdcMemMask, 0,0, width, height);
	SelectObject(hdcMemMask, GetStockObject(WHITE_BRUSH));
	Rectangle(hdcMemMask, 0,0, iDisplayCutWidth, iDisplayCutHeight);
	DeleteDC(hdcMemMask);

	this->isUptodate = true;
	return true;
}

int CGDALGeoTiff::getRelativeWidth() {
	return this->iRelativeWidth;
}
int CGDALGeoTiff::getRelativeHeight() {
	return this->iRelativeHeight;
}

void CGDALGeoTiff::getMask(HBITMAP &mask) {
	mask = this->hbmMask;
}

BITMAPINFO * CGDALGeoTiff::getBitmapInfo() {
	return this->bmi;	
}
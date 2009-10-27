#include "stdafx.h"
#include "PackedDib.h"


HPALETTE PackedDibCreatePalette(BITMAPINFO * pPackedDib) {

	HPALETTE	hPalette;
	int			i, iNumColors;
	LOGPALETTE *plp;
	RGBQUAD	   *prgb;

	if (0 == (iNumColors = PackedDibGetNumColors (pPackedDib)))
		return NULL;

	plp = (LOGPALETTE *) malloc(sizeof(LOGPALETTE) * (iNumColors - 1) * sizeof(PALETTEENTRY));

	plp->palVersion = 0x0300;
	plp->palNumEntries = iNumColors;

	for (i = 0 ; i < iNumColors ; i++) {
		prgb = PackedDibGetColorTableEntry(pPackedDib, i);

		plp->palPalEntry[i].peRed = prgb->rgbRed;
		plp->palPalEntry[i].peGreen = prgb->rgbGreen;
		plp->palPalEntry[i].peBlue = prgb->rgbBlue;
		plp->palPalEntry[i].peFlags = 0;
	}

	hPalette = CreatePalette(plp);
	delete(plp);

	return hPalette;
}

int PackedDibGetNumColors (BITMAPINFO * pPackedDib) {
	int iNumColors;
	iNumColors = PackedDibGetColorsUsed (pPackedDib);

	if (iNumColors == 0&& PackedDibGetBitCount (pPackedDib) < 16)
		iNumColors = 1 << PackedDibGetBitCount (pPackedDib);

	return iNumColors;
}

int PackedDibGetColorsUsed(BITMAPINFO * pPackedDib) {

	if (pPackedDib->bmiHeader.biSize == sizeof(BITMAPCOREHEADER))
		return 0;
	else
		return pPackedDib->bmiHeader.biClrUsed;
}

int PackedDibGetBitCount(BITMAPINFO * pPackedDib) {
	
	if (pPackedDib->bmiHeader.biSize == sizeof(BITMAPCOREHEADER))
		return ((PBITMAPCOREINFO)pPackedDib)->bmciHeader.bcBitCount;
	else
		return pPackedDib->bmiHeader.biBitCount;
}

RGBQUAD * PackedDibGetColorTableEntry(BITMAPINFO * pPackedDib, int i) {
	
	if (PackedDibGetNumColors (pPackedDib) == 0)
		return NULL;

	if (pPackedDib->bmiHeader.biSize == sizeof(BITMAPCOREHEADER))
		return (RGBQUAD *) (((RGBTRIPLE *) PackedDibGetColorTablePtr (pPackedDib)) + 1);
	else
		return PackedDibGetColorTablePtr(pPackedDib) + i;
}

RGBQUAD * PackedDibGetColorTablePtr(BITMAPINFO * pPackedDib)
{
	if(PackedDibGetNumColors(pPackedDib) == 0)
		return 0;

	return (RGBQUAD *) (((BYTE *) pPackedDib) + PackedDibGetInfoHeaderSize(pPackedDib));
}

int PackedDibGetInfoHeaderSize(BITMAPINFO * pPackedDib) {
	
	if (pPackedDib->bmiHeader.biSize == sizeof(BITMAPCOREHEADER))
		return ((PBITMAPCOREINFO)pPackedDib)->bmciHeader.bcSize;
	else if (pPackedDib->bmiHeader.biSize == sizeof(BITMAPINFOHEADER))
		return pPackedDib->bmiHeader.biSize + (pPackedDib->bmiHeader.biCompression == BI_BITFIELDS ? 12 : 0);
	else return pPackedDib->bmiHeader.biSize;
}
#pragma once
#include <windows.h>

HPALETTE PackedDibCreatePalette(BITMAPINFO * pPackedDib);
int PackedDibGetNumColors (BITMAPINFO * pPackedDib);
int PackedDibGetColorsUsed(BITMAPINFO * pPackedDib);
int PackedDibGetBitCount(BITMAPINFO * pPackedDib);
RGBQUAD * PackedDibGetColorTableEntry(BITMAPINFO * pPackedDib, int i);
int PackedDibGetNumColors (BITMAPINFO * pPackedDib);
RGBQUAD * PackedDibGetColorTablePtr(BITMAPINFO * pPackedDib);
int PackedDibGetInfoHeaderSize(BITMAPINFO * pPackedDib);
#pragma once

__declspec(dllexport) void RotateDC( HDC hDC, char * sourceBytes, double angle);

void RotBlt(HDC destDC, int srcx1, int srcy1, int srcx2, int srcy2,
HDC srcDC , int destx1, int desty1 ,int thetaInDegrees ,DWORD mode);
__declspec(dllexport) HBITMAP GetRotatedBitmap( HBITMAP hBitmap, float radians, COLORREF clrBack );
__declspec(dllexport) HBITMAP GetRotatedBitmapNT( HBITMAP hBitmap, float degrees, COLORREF clrBack );
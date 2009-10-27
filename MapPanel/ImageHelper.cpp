#include "stdafx.h"
#include "ImageHelper.h"

__declspec(dllexport) void RotateDC( HDC hDC, char * destBytes, double angle, int width, int height) {
	
	// copy source bytes
	char * sourcebytes = NULL;
	memcpy(destBytes, sourcebytes, 3 * width * height);


	HDC sourceDC = CreateCompatibleDC(hDC);

	//RotBlt(

}

__declspec(dllexport) HBITMAP GetRotatedBitmapNT( HBITMAP hBitmap, float degrees, COLORREF clrBack )
{
	float theta = degrees * (3.14159/180);

	// Create a memory DC compatible with the display
	HDC sourceDC, destDC;
	sourceDC = CreateCompatibleDC( NULL );
	destDC = CreateCompatibleDC( NULL );

	// Get logical coordinates
	BITMAP bm;
	::GetObject( hBitmap, sizeof( bm ), &bm );

	float cosine = (float)cos(theta);
	float sine = (float)sin(theta);

	// Compute dimensions of the resulting bitmap
	// First get the coordinates of the 3 corners other than origin
	int x1 = (int)(bm.bmHeight * sine);
	int y1 = (int)(bm.bmHeight * cosine);
	int x2 = (int)(bm.bmWidth * cosine + bm.bmHeight * sine);
	int y2 = (int)(bm.bmHeight * cosine - bm.bmWidth * sine);
	int x3 = (int)(bm.bmWidth * cosine);
	int y3 = (int)(-bm.bmWidth * sine);

	int minx = min(0,min(x1, min(x2,x3)));
	int miny = min(0,min(y1, min(y2,y3)));
	int maxx = max(0,max(x1, max(x2,x3)));
	int maxy = max(0,max(y1, max(y2,y3)));

	int w = maxx - minx;
	int h = maxy - miny;

	// Create a bitmap to hold the result
	HBITMAP hbmResult = CreateCompatibleBitmap(destDC, w, h);

	HBITMAP hbmOldSource = (HBITMAP)::SelectObject( sourceDC, hBitmap );
	HBITMAP hbmOldDest = (HBITMAP)::SelectObject( destDC, hbmResult );

	// Draw the background color before we change mapping mode
	HBRUSH hbrBack = CreateSolidBrush( clrBack );
	HBRUSH hbrOld = (HBRUSH)::SelectObject( destDC, hbrBack );
	PatBlt(destDC, 0, 0, w, h, PATCOPY );
	::DeleteObject( ::SelectObject( destDC, hbrOld ) );

	// We will use world transform to rotate the bitmap
	/*SetGraphicsMode(destDC, GM_ADVANCED);
	XFORM xform;
	xform.eM11 = cosine;
	xform.eM12 = -sine;
	xform.eM21 = sine;
	xform.eM22 = cosine;
	xform.eDx = (float)-minx;
	xform.eDy = (float)-miny;

	SetWorldTransform( destDC, &xform );*/

	// Now do the actual rotating - a pixel at a time
	//BitBlt(destDC, 0,0,bm.bmWidth, bm.bmHeight, sourceDC, 0, 0, SRCCOPY );

	RotBlt(destDC, 0, 0, bm.bmWidth, bm.bmHeight,
		sourceDC , 0, 0 , degrees , SRCCOPY);

	// Restore DCs
	::SelectObject( sourceDC, hbmOldSource );
	::SelectObject( destDC, hbmOldDest );

	return hbmResult;
}


//Function  : RotBlt
//Parameters :
//  HDC destDC, //destDC onto which to perform blt operation
//  int srcx1,  //start x coordinate of src DC
//  int srcy1,  //start y coordinate of src DC
//  int srcx2,  //end x coordinate of src DC
//              //remember this isn't width
//  int srcy2,  //end y coordinate of src DC
//              //remember this isn't height
//  HDC srcDC , //srcDC
//  int destx1, //left Xcoordinate of destDC where rotated DC will
//              //be blt (top -left)
//  int desty1 ,//top Ycoordinate of destDC where rotated DC will
//              //be blt (top - left)
//  int thetaInDegrees , // angle in degrees in which to rotate srcDC
//  DWORD mode  //ROp code same as Bitblt
//
//
//Return value : NULL


void RotBlt(HDC destDC, int srcx1, int srcy1, int srcx2, int srcy2,
  HDC srcDC, int destx1, int desty1, int thetaInDegrees, DWORD mode)
{
  double theta = thetaInDegrees * (3.14159/180);
  //multiply degrees by PI/180 to convert to radians

  //determine width and height of source
  int width = srcx2 - srcx1;
  int height = srcy2 - srcy1;

  //determine centre/pivot point ofsource
  int centreX = int(float(srcx2 + srcx1)/2);
  int centreY = int(float(srcy2 + srcy1)/2);

  //since image is rotated we need to allocate a rectangle
  //which can hold the image in any orientation
  if(width>height)height = width;
  else
    width = height;


  //allocate memory and blah blah
  HDC memDC = CreateCompatibleDC(destDC);
  HBITMAP memBmp = CreateCompatibleBitmap(destDC, width, height);

  HBITMAP obmp = (HBITMAP) SelectObject(memDC, memBmp);

  //pivot point of our mem DC
  int newCentre = int(float(width)/2);

  //hmmm here's the rotation code. X std maths :|
  for(int x = srcx1; x<=srcx2; x++)
    for(int y = srcy1; y<=srcy2; y++)
    {
      COLORREF col = GetPixel(srcDC,x,y);

      int newX = int((x-centreX)*sin(theta)+(y-centreY)*cos(theta));
      int newY = int((x-centreX)*cos(theta)-(y-centreY)*sin(theta));


      SetPixel(memDC , newX + newCentre, newY + newCentre, col);
    }

  //splash onto the destination
  BitBlt(destDC, destx1, desty1, width, height, memDC, 0,0,mode);


  //free mem and blah
  SelectObject(memDC, obmp);

  DeleteDC(memDC);
  DeleteObject(memBmp);
}


// Returns		- Returns new bitmap with rotated image
// hDIB			- Device-independent bitmap to rotate
// radians		- Angle of rotation in radians
// clrBack		- Color of pixels in the resulting bitmap that do
//			  not get covered by source pixels
__declspec(dllexport) HANDLE GetRotatedBitmap( HANDLE hDIB, float radians, COLORREF clrBack )
{
	// Get source bitmap info
	BITMAPINFO &bmInfo = *(LPBITMAPINFO)hDIB ;
	int bpp = bmInfo.bmiHeader.biBitCount;		// Bits per pixel

	int nColors = bmInfo.bmiHeader.biClrUsed ? bmInfo.bmiHeader.biClrUsed :
					1 << bpp;
	int nWidth = bmInfo.bmiHeader.biWidth;
	int nHeight = bmInfo.bmiHeader.biHeight;
	int nRowBytes = ((((nWidth * bpp) + 31) & ~31) / 8);

	// Make sure height is positive and biCompression is BI_RGB or BI_BITFIELDS
	DWORD &compression = bmInfo.bmiHeader.biCompression;
	if( nHeight < 0 || (compression!=BI_RGB && compression!=BI_BITFIELDS))
		return NULL;

	LPVOID lpDIBBits;
	if( bmInfo.bmiHeader.biBitCount > 8 )
		lpDIBBits = (LPVOID)((LPDWORD)(bmInfo.bmiColors +
			bmInfo.bmiHeader.biClrUsed) +
			((compression == BI_BITFIELDS) ? 3 : 0));
	else
		lpDIBBits = (LPVOID)(bmInfo.bmiColors + nColors);


	// Compute the cosine and sine only once
	float cosine = (float)cos(radians);
	float sine = (float)sin(radians);

	// Compute dimensions of the resulting bitmap
	// First get the coordinates of the 3 corners other than origin
	int x1 = (int)(-nHeight * sine);
	int y1 = (int)(nHeight * cosine);
	int x2 = (int)(nWidth * cosine - nHeight * sine);
	int y2 = (int)(nHeight * cosine + nWidth * sine);
	int x3 = (int)(nWidth * cosine);
	int y3 = (int)(nWidth * sine);

	int minx = min(0,min(x1, min(x2,x3)));
	int miny = min(0,min(y1, min(y2,y3)));
	int maxx = max(x1, max(x2,x3));
	int maxy = max(y1, max(y2,y3));

	int w = maxx - minx;
	int h = maxy - miny;


	// Create a DIB to hold the result
	int nResultRowBytes = ((((w * bpp) + 31) & ~31) / 8);
	long len = nResultRowBytes * h;
	int nHeaderSize = ((LPBYTE)lpDIBBits-(LPBYTE)hDIB) ;
	HANDLE hDIBResult = GlobalAlloc(GMEM_FIXED,len+nHeaderSize);
	// Initialize the header information
	memcpy( (void*)hDIBResult, (void*)hDIB, nHeaderSize);
	BITMAPINFO &bmInfoResult = *(LPBITMAPINFO)hDIBResult ;
	bmInfoResult.bmiHeader.biWidth = w;
	bmInfoResult.bmiHeader.biHeight = h;
	bmInfoResult.bmiHeader.biSizeImage = len;

	LPVOID lpDIBBitsResult = (LPVOID)((LPBYTE)hDIBResult + nHeaderSize);

	// Get the back color value (index)
	ZeroMemory( lpDIBBitsResult, len );
	DWORD dwBackColor;
	switch(bpp)
	{
	case 1:	//Monochrome
		if( clrBack == RGB(255,255,255) )
			memset( lpDIBBitsResult, 0xff, len );
		break;
	case 4:
	case 8:	//Search the color table
		int i;
		for(i = 0; i < nColors; i++ )
		{
			if( bmInfo.bmiColors[i].rgbBlue ==  GetBValue(clrBack)
				&& bmInfo.bmiColors[i].rgbGreen ==  GetGValue(clrBack)
				&& bmInfo.bmiColors[i].rgbRed ==  GetRValue(clrBack) )
			{
				if(bpp==4) i = i | i<<4;
				memset( lpDIBBitsResult, i, len );
				break;
			}
		}
		// If not match found the color remains black
		break;
	case 16:
		// Windows95 supports 5 bits each for all colors or 5 bits for red & blue
		// and 6 bits for green - Check the color mask for RGB555 or RGB565
		if( *((DWORD*)bmInfo.bmiColors) == 0x7c00 )
		{
			// Bitmap is RGB555
			dwBackColor = ((GetRValue(clrBack)>>3) << 10) +
					((GetRValue(clrBack)>>3) << 5) +
					(GetBValue(clrBack)>>3) ;
		}
		else
		{
			// Bitmap is RGB565
			dwBackColor = ((GetRValue(clrBack)>>3) << 11) +
					((GetRValue(clrBack)>>2) << 5) +
					(GetBValue(clrBack)>>3) ;
		}
		break;
	case 24:
	case 32:
		dwBackColor = (((DWORD)GetRValue(clrBack)) << 16) |
				(((DWORD)GetGValue(clrBack)) << 8) |
				(((DWORD)GetBValue(clrBack)));
		break;
	}


	// Now do the actual rotating - a pixel at a time
	// Computing the destination point for each source point
	// will leave a few pixels that do not get covered
	// So we use a reverse transform - e.i. compute the source point
	// for each destination point

	for( int y = 0; y < h; y++ )
	{
		for( int x = 0; x < w; x++ )
		{
			int sourcex = (int)((x+minx)*cosine + (y+miny)*sine);
			int sourcey = (int)((y+miny)*cosine - (x+minx)*sine);
			if( sourcex >= 0 && sourcex < nWidth && sourcey >= 0
				&& sourcey < nHeight )
			{
				// Set the destination pixel
				switch(bpp)
				{
					BYTE mask;
				case 1:		//Monochrome
					mask = *((LPBYTE)lpDIBBits + nRowBytes*sourcey +
						sourcex/8) & (0x80 >> sourcex%8);
					//Adjust mask for destination bitmap
					mask = mask ? (0x80 >> x%8) : 0;
					*((LPBYTE)lpDIBBitsResult + nResultRowBytes*(y) +
								(x/8)) &= ~(0x80 >> x%8);
					*((LPBYTE)lpDIBBitsResult + nResultRowBytes*(y) +
								(x/8)) |= mask;
					break;
				case 4:
					mask = *((LPBYTE)lpDIBBits + nRowBytes*sourcey +
						sourcex/2) & ((sourcex&1) ? 0x0f : 0xf0);
					//Adjust mask for destination bitmap
					if( (sourcex&1) != (x&1) )
						mask = (mask&0xf0) ? (mask>>4) : (mask<<4);
					*((LPBYTE)lpDIBBitsResult + nResultRowBytes*(y) +
							(x/2)) &= ~((x&1) ? 0x0f : 0xf0);
					*((LPBYTE)lpDIBBitsResult + nResultRowBytes*(y) +
							(x/2)) |= mask;
					break;
				case 8:
					BYTE pixel ;
					pixel = *((LPBYTE)lpDIBBits + nRowBytes*sourcey +
							sourcex);
					*((LPBYTE)lpDIBBitsResult + nResultRowBytes*(y) +
							(x)) = pixel;
					break;
				case 16:
					DWORD dwPixel;
					dwPixel = *((LPWORD)((LPBYTE)lpDIBBits +
							nRowBytes*sourcey + sourcex*2));
					*((LPWORD)((LPBYTE)lpDIBBitsResult +
						nResultRowBytes*y + x*2)) = (WORD)dwPixel;
					break;
				case 24:
					dwPixel = *((LPDWORD)((LPBYTE)lpDIBBits +
						nRowBytes*sourcey + sourcex*3)) & 0xffffff;
					*((LPDWORD)((LPBYTE)lpDIBBitsResult +
						nResultRowBytes*y + x*3)) |= dwPixel;
					break;
				case 32:
					dwPixel = *((LPDWORD)((LPBYTE)lpDIBBits +
						nRowBytes*sourcey + sourcex*4));
					*((LPDWORD)((LPBYTE)lpDIBBitsResult +
						nResultRowBytes*y + x*4)) = dwPixel;
				}
			}
			else
			{
				// Draw the background color. The background color
				// has already been drawn for 8 bits per pixel and less
				switch(bpp)
				{
				case 16:
					*((LPWORD)((LPBYTE)lpDIBBitsResult +
						nResultRowBytes*y + x*2)) =
						(WORD)dwBackColor;
					break;
				case 24:
					*((LPDWORD)((LPBYTE)lpDIBBitsResult +
						nResultRowBytes*y + x*3)) |= dwBackColor;
					break;
				case 32:
					*((LPDWORD)((LPBYTE)lpDIBBitsResult +
						nResultRowBytes*y + x*4)) = dwBackColor;
					break;
				}
			}
		}
	}

	return hDIBResult;
}
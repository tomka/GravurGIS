#pragma once
#include "StdAfx.h"

#define fixpt(n) ((long)((n)*(1<<25)))
#define mul(a,b) ((((long long)a)*(b))>>25)

class CMandelbrotLayer
{
private:
	BITMAPINFO * m_lpBMIH;
	HBITMAP m_hBitmap;
	LPBYTE m_lpImage;
	int width, height;
	RGBQUAD colorTable[256];
	void GetColors(wchar_t * filename);
	bool useColorTables;

	double Sx, Sy, Fx, Fy;
	
public:
	CMandelbrotLayer(int width, int height);
	~CMandelbrotLayer(void);
	void recalculateImage(double dX, double dY, unsigned int maxIterations,
										  double xPos, double yPos, double size);
	void draw(HDC hDC);
	int calc_point_float(double x0, double y0, unsigned int maxIterations);
	int calc_point_fixed(long x0, long y0, unsigned int maxIterations);
	bool initialized;
	void init(HDC hDC);
};
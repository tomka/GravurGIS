#include "stdafx.h"
#include "MandelbrotLayer.h"

CMandelbrotLayer::CMandelbrotLayer(int width, int height)
{
	this->width = width;
	this->height = height;
	this->initialized = false;

	BITMAPINFO * m_lpBMIH = NULL;
	HBITMAP m_hBitmap = NULL;
	LPBYTE m_lpImage = NULL;
	Sx = -2.1;
	Sy = -1.3;
	Fx = 1;
	Fy = 1.3;
	useColorTables = false;

}

CMandelbrotLayer::~CMandelbrotLayer(void)
{
	delete m_hBitmap;
	delete m_lpBMIH;
	delete m_lpImage;
}

void CMandelbrotLayer::GetColors(wchar_t * filename) {
	
}

void CMandelbrotLayer::draw(HDC hDC) {
	if (m_lpImage == NULL) return;
	SetStretchBltMode(hDC, COLORONCOLOR);

	HDC hdMem = CreateCompatibleDC(hDC);
	SelectObject(hdMem, m_hBitmap);
	
	/*::StretchDIBits(hDC, 0, 0, width, height,
		0, 0, m_lpBMIH->biWidth, m_lpBMIH->biHeight,
		m_lpImage, (LPBITMAPINFO) m_lpBMIH, DIB_RGB_COLORS, SRCCOPY);*/
	::BitBlt(hDC, 0,0, width, height, hdMem, 0, 0, SRCCOPY);
	DeleteDC(hdMem);
}

void CMandelbrotLayer::init(HDC hDC) {
	HDC hdMem = CreateCompatibleDC(hDC);

	if (!useColorTables) {
		//if (m_lpBMIH != NULL) delete(m_lpBMIH);

		m_lpBMIH = (BITMAPINFO *) new char[sizeof(BITMAPINFOHEADER)]; // we do not need a color table!
		m_lpBMIH->bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
		m_lpBMIH->bmiHeader.biWidth = width;
		m_lpBMIH->bmiHeader.biHeight = height;
		m_lpBMIH->bmiHeader.biPlanes = 1;
		m_lpBMIH->bmiHeader.biBitCount = 24;
		m_lpBMIH->bmiHeader.biCompression = BI_RGB;
		m_lpBMIH->bmiHeader.biSizeImage = 0;
		m_lpBMIH->bmiHeader.biXPelsPerMeter = 0;
		m_lpBMIH->bmiHeader.biYPelsPerMeter = 0;
		m_lpBMIH->bmiHeader.biClrUsed = 0;
		m_lpBMIH->bmiHeader.biClrImportant = 0;

		DWORD m_dwSizeImage = m_lpBMIH->bmiHeader.biSizeImage;
		
		if(m_dwSizeImage == 0) {
			DWORD dwBytes = ((DWORD) m_lpBMIH->bmiHeader.biWidth * m_lpBMIH->bmiHeader.biBitCount) / 32;
			if(((DWORD) m_lpBMIH->bmiHeader.biWidth * m_lpBMIH->bmiHeader.biBitCount) % 32) {
				dwBytes++;
			}
			dwBytes *= 4;
			m_dwSizeImage = dwBytes * m_lpBMIH->bmiHeader.biHeight; // no compression
		}

		m_hBitmap = ::CreateDIBSection(hdMem, m_lpBMIH, // create section and alloc memory
			DIB_RGB_COLORS,	(LPVOID*) &m_lpImage, NULL, 0);
	
	} else {

		m_lpBMIH = (BITMAPINFO*) malloc(sizeof(BITMAPINFOHEADER) + 256 * sizeof(RGBQUAD));
		::ZeroMemory(&(m_lpBMIH->bmiHeader), sizeof(BITMAPINFOHEADER));
		m_lpBMIH->bmiHeader.biWidth = width;
		m_lpBMIH->bmiHeader.biHeight = -(height); //negative indicate top down, not bottom up
		m_lpBMIH->bmiHeader.biPlanes = 1;
		m_lpBMIH->bmiHeader.biBitCount = 8;
		m_lpBMIH->bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
		m_lpBMIH->bmiHeader.biCompression = BI_RGB;
		
		m_lpBMIH->bmiHeader.biSizeImage = m_lpBMIH->bmiHeader.biWidth * m_lpBMIH->bmiHeader.biHeight * m_lpBMIH->bmiHeader.biBitCount / 8;
		m_lpBMIH->bmiHeader.biClrUsed = m_lpBMIH->bmiHeader.biClrImportant = 256;

		// Fill color Table:
		for (int i = 256; i--; ) {
			this->colorTable[i].rgbRed = 0;
			this->colorTable[i].rgbGreen = i;
			this->colorTable[i].rgbBlue = 0;
			this->colorTable[i].rgbReserved = 0;
			m_lpBMIH->bmiColors[i].rgbRed = 0;
			m_lpBMIH->bmiColors[i].rgbGreen = i;
			m_lpBMIH->bmiColors[i].rgbBlue = 0;
			m_lpBMIH->bmiColors[i].rgbReserved;
		}

		m_hBitmap = ::CreateDIBSection(hdMem, m_lpBMIH, // create section and alloc memory
			DIB_PAL_COLORS,	(LPVOID*) &m_lpImage, NULL, 0);
	}

	
	
	this->initialized = true;
}

void CMandelbrotLayer::recalculateImage(double dX, double dY, unsigned int maxIterations,
										  double xPos, double yPos, double size)
{
	if (m_lpImage == NULL) return;

	BYTE* dibits = m_lpImage;

	if (!useColorTables) {
		

		double viewWidth = (static_cast<double>(width) / height) * size;
		double viewHeight = size;

		float xStep = static_cast<float>(viewWidth / width);
		float yStep = static_cast<float>(viewHeight / height);

		viewWidth  /= 2;
		viewHeight /= 2;

		for(int j = height; j--; ) {
			for(int i = width; i--; ) {

				double xCoord = (xPos - viewWidth) + (double)i * xStep;
				double yCoord = (yPos - viewHeight) + (double)j * yStep;

				//int value = calc_point_float(xCoord, yCoord, maxIterations);
				int value = calc_point_fixed(fixpt(xCoord),fixpt(yCoord), maxIterations);

				//SetPixel(hDC, i, j, RGB(value*2, value*4, value));
				*(dibits++) = value<<1;  // red - <<1 is faster as *2
				*(dibits++) = value<<4;  // green <<2 is faster than *4
				*(dibits++) = value;	 // blue
			}
		}
	} else {


		double x, y, x1, y1, xx, xmin, xmax, ymin, ymax = 0.0;
		int looper, s, z = 0;
		double intigralX, intigralY = 0.0;
		xmin = Sx;
		ymin = Sy;
		xmax = Fx;
		ymax = Fy;
		intigralX = (xmax - xmin) / width;
		intigralY = (ymax - ymin) / height;


		x = xmin;
		for(s = 1; s < width; s++)
		{
			y = ymin;
			for(z = 1; z < height; z++)
			{
				x1 = 0;
				y1 = 0;
				looper = 0;
				while(looper < 100 && ::sqrt((x1 * x1) + (y1 * y1)) < 2)
				{
					looper++;
					xx = (x1 * x1) - (y1 * y1) + x;
					y1 = 2 * x1 * y1 + y;
					x1 = xx;
				}
				double perc = looper / (100.0);
				//b.SetPixel(s,z,cs[val]);
				*(dibits++) = ((int)(perc * 255));

				y += intigralY;
			}
			x += intigralX;
		}
	}
}

int CMandelbrotLayer::calc_point_float(double x0, double y0, unsigned int maxIterations) {

	double x = 0;
	double y = 0;
	double xSucc;
	double ySucc;

	for(unsigned int i = 1; i < maxIterations; ++i) {
		xSucc = x * x - y * y + x0;
		ySucc = 2.0 * x * y + y0;
		x = xSucc;
		y = ySucc;

		if((x*x + y*y) > 4.0)
			return i;
	}
	return 0;
}

int CMandelbrotLayer::calc_point_fixed(long x0, long y0, unsigned int maxIterations) {

	long x = 0;
	long y = 0;
	long xSucc;
	long ySucc;
	long h;

	for(unsigned int i = 1; i < maxIterations; ++i) {
		xSucc = mul(x, x) - mul(y, y) + x0;
		h = mul(x, y);
		ySucc = mul(fixpt(2), h) + y0;
		x = xSucc;
		y = ySucc;

		if((mul(x, x) + mul(y, y)) > fixpt(4))
			return i;
	}
	return 0;
}
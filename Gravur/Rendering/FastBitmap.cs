using System.Drawing;
using System.Drawing.Imaging;
using System;
using System.Runtime.InteropServices;

namespace GravurGIS.Rendering
{
    public class FastBitmap : IDisposable
    {

        private Bitmap image;

        private BitmapData bitmapData;

        private int height;

        private int width;

        private byte[] rgbValues;

        bool locked = false;



        public int Height
        {
            get { return this.height; }
        }



        public int Width
        {
            get { return this.width; }
        }



        public FastBitmap(int x, int y)
        {

            width = x;

            height = y;

            image = new Bitmap(x, y);

        }

        public FastBitmap(Bitmap bitmap)
        { 
            width = bitmap.Width;
            height = bitmap.Height;
            image = bitmap;
        }



        public byte[] GetAllPixels()
        {

            return rgbValues;

        }



        public void SetAllPixels(byte[] pixels)
        {
            rgbValues = pixels;
        }



        public Color GetPixel(int x, int y)
        {

            int blue = rgbValues[(y * image.Width + x) * 3];

            int green = rgbValues[(y * image.Width + x) * 3 + 1];

            int red = rgbValues[(y * image.Width + x) * 3 + 2];



            return Color.FromArgb(red, green, blue);

        }



        public void SetPixel(int x, int y, Color cIn)
        {

            rgbValues[(y * image.Width + x) * 3] = cIn.B;

            rgbValues[(y * image.Width + x) * 3 + 1] = cIn.G;

            rgbValues[(y * image.Width + x) * 3 + 2] = cIn.R;

        }



        public static implicit operator Image(FastBitmap bmp)
        {
            return bmp.image;
        }



        public static implicit operator Bitmap(FastBitmap bmp)
        {
            return bmp.image;
        }



        public void LockPixels()
        {

            LockPixels(new Rectangle(0, 0, image.Width, image.Height));

        }



        private void LockPixels(Rectangle area)
        {

            if (locked)

                return;

            locked = true;

            bitmapData = image.LockBits(area, ImageLockMode.ReadWrite,

                PixelFormat.Format24bppRgb);

            IntPtr ptr = bitmapData.Scan0;

            int stride = bitmapData.Stride;

            int numBytes = image.Width * image.Height * 3;

            rgbValues = new byte[numBytes];

            Marshal.Copy(ptr, rgbValues, 0, numBytes);
        }



        public void UnlockPixels()
        {

            if (!locked)

                return;

            locked = false;

            Marshal.Copy(rgbValues, 0, bitmapData.Scan0, image.Width * image.Height * 3);

            image.UnlockBits(bitmapData);
            rgbValues = null; // since we do not use it anymore until it gets locked again

        }

        public IntPtr HBitmap
        {
            get { return this.image.GetHbitmap(); }
        }


        #region IDisposable Members

        public void Dispose()
        {
            this.image.Dispose();
        }

        #endregion
    }
}
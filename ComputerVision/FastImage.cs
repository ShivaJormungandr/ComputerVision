using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace ComputerVision
{
    public class FastImage
    {
        public int Height = 0;
        public int Width = 0;
        private Rectangle rectangle;
        private BitmapData bitmapData = null;
        private Color color;
        private Point size;
        private int currentBitmapWidth = 0;

        private Bitmap _image = null;
        public Bitmap Image => _image;

        struct PixelData
        {
            public byte red, green, blue;
        }

        public FastImage(Bitmap bitmap)
        {
            _image = bitmap;
            Width = _image.Width;
            Height = _image.Height;
            size = new Point(_image.Size);
            currentBitmapWidth = size.X;
        }

        public void Lock()
        {
            // Rectangle For Locking The Bitmap In Memory
            rectangle = new Rectangle(0, 0, _image.Width, _image.Height);
            // Get The Bitmap's Pixel Data From The Locked Bitmap
            bitmapData = _image.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
        }

        public void Unlock()
        {
            _image.UnlockBits(bitmapData);
        }

        public Color GetPixel(int col, int row)
        {
            unsafe
            {
                PixelData* pBase = (PixelData*)bitmapData.Scan0;
                PixelData* pPixel = pBase + row * currentBitmapWidth + col;
                color = Color.FromArgb(pPixel->red, pPixel->green, pPixel->blue);
            }
            return color;
        }

        public void SetPixel(int col, int row, Color c)
        {
            unsafe
            {
                PixelData* pBase = (PixelData*)bitmapData.Scan0;
                PixelData* pPixel = pBase + row * currentBitmapWidth + col;
                pPixel->red = c.R;
                pPixel->green = c.G;
                pPixel->blue = c.B;
            }
        }

        public void DrawCross(Point cornerTopLeft, Point cornerBottomRight, Color c)
        {
            int middle;

            int temp = (cornerBottomRight.Y - cornerTopLeft.Y) / 2;
            middle = temp + cornerTopLeft.Y;

            for (int i = cornerTopLeft.X; i < cornerBottomRight.X; i++)
            {
                SetPixel(i, middle, c);
            }

            temp = (cornerBottomRight.X - cornerTopLeft.X) / 2;
            middle = temp + cornerTopLeft.X;

            for (int i = cornerTopLeft.Y; i < cornerBottomRight.Y; i++)
            {
                SetPixel(middle, i, c);
            }
        }
    }
}

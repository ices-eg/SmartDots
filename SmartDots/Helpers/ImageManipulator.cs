using Accord.Imaging;
using Accord.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SmartDots.Helpers
{
    public static class ImageManipulator
    {
        //public static BitmapImage SetBrightness(BitmapImage img)
        //{
        //    Stopwatch stopwatch = new Stopwatch();

        //    // Begin timing.
        //    stopwatch.Start();
        //    unsafe
        //    {
        //        double brightness = Brightness;
        //        Bitmap bmap = BitmapConverter.BitmapImage2Bitmap(img);
        //        BitmapData bitmapData = bmap.LockBits(new System.Drawing.Rectangle(0, 0, bmap.Width, bmap.Height),
        //            ImageLockMode.ReadWrite, bmap.PixelFormat);

        //        int bytesPerPixel = Bitmap.GetPixelFormatSize(bmap.PixelFormat) / 8;
        //        int heightInPixels = bitmapData.Height;
        //        int widthInBytes = bitmapData.Width * bytesPerPixel;
        //        byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

        //        Parallel.For((long)0, heightInPixels, y =>
        //        {
        //            byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);

        //            for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
        //            {
        //                double oldBlue = currentLine[x] + brightness;
        //                if (oldBlue < 0) oldBlue = 1;
        //                if (oldBlue > 255) oldBlue = 255;

        //                double oldGreen = currentLine[x + 1] + brightness;
        //                if (oldGreen < 0) oldGreen = 1;
        //                if (oldGreen > 255) oldGreen = 255;

        //                double oldRed = currentLine[x + 2] + brightness;
        //                if (oldRed < 0) oldRed = 1;
        //                if (oldRed > 255) oldRed = 255;

        //                currentLine[x] = (byte)oldBlue;
        //                currentLine[x + 1] = (byte)oldGreen;
        //                currentLine[x + 2] = (byte)oldRed;
        //            }
        //        });
        //        bmap.UnlockBits(bitmapData);

        //        // Stop timing.
        //        stopwatch.Stop();

        //        // Write result.
        //        Console.WriteLine("Time elapsed for SetBrightness: " + stopwatch.Elapsed);
        //        return BitmapConverter.Bitmap2BitmapImage(bmap);
        //    }
        //}

        //public static BitmapImage SetContrast(BitmapImage img)
        //{
        //    Stopwatch stopwatch = new Stopwatch();

        //    // Begin timing.
        //    stopwatch.Start();

        //    unsafe
        //    {
        //        double contrast = Contrast;
        //        Bitmap bmap = BitmapConverter.BitmapImage2Bitmap(img);
        //        contrast = (100.0 + contrast) / 100.0;
        //        contrast *= contrast;
        //        BitmapData bitmapData = bmap.LockBits(new System.Drawing.Rectangle(0, 0, bmap.Width, bmap.Height),
        //            ImageLockMode.ReadWrite, bmap.PixelFormat);

        //        int bytesPerPixel = Bitmap.GetPixelFormatSize(bmap.PixelFormat) / 8;
        //        int heightInPixels = bitmapData.Height;
        //        int widthInBytes = bitmapData.Width * bytesPerPixel;
        //        byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

        //        Parallel.For((long)0, heightInPixels, y =>
        //        {
        //            byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);

        //            for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
        //            {
        //                double oldBlue = currentLine[x] / 255.0;
        //                oldBlue -= 0.5;
        //                oldBlue *= contrast;
        //                oldBlue += 0.5;
        //                oldBlue *= 255;
        //                if (oldBlue < 0) oldBlue = 0;
        //                if (oldBlue > 255) oldBlue = 255;

        //                double oldGreen = currentLine[x + 1] / 255.0;
        //                oldGreen -= 0.5;
        //                oldGreen *= contrast;
        //                oldGreen += 0.5;
        //                oldGreen *= 255;
        //                if (oldGreen < 0) oldGreen = 0;
        //                if (oldGreen > 255) oldGreen = 255;

        //                double oldRed = currentLine[x + 2] / 255.0;
        //                oldRed -= 0.5;
        //                oldRed *= contrast;
        //                oldRed += 0.5;
        //                oldRed *= 255;
        //                if (oldRed < 0) oldRed = 0;
        //                if (oldRed > 255) oldRed = 255;

        //                currentLine[x] = (byte)oldBlue;
        //                currentLine[x + 1] = (byte)oldGreen;
        //                currentLine[x + 2] = (byte)oldRed;
        //            }
        //        });
        //        bmap.UnlockBits(bitmapData);

        //        // Stop timing.
        //        stopwatch.Stop();

        //        // Write result.
        //        Console.WriteLine("Time elapsed for SetContrast: " + stopwatch.Elapsed);
        //        return BitmapConverter.Bitmap2BitmapImage(bmap);
        //    }
        //}

        public static BitmapSource SetBrightnessContrast(Bitmap img, double b, double c)
        {
            double contrastFactor = (100.0 + c) / 100.0;
            contrastFactor *= contrastFactor;

            byte[] lut = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                double val = i + b;
                val = val / 255.0;
                val -= 0.5;
                val *= contrastFactor;
                val += 0.5;
                val *= 255.0;
                lut[i] = (byte)Math.Max(0, Math.Min(255, val));
            }

            Bitmap bmap = img;
            unsafe
            {
                BitmapData bitmapData = bmap.LockBits(new System.Drawing.Rectangle(0, 0, bmap.Width, bmap.Height),
                    ImageLockMode.ReadWrite, bmap.PixelFormat);

                int bytesPerPixel = Bitmap.GetPixelFormatSize(bmap.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                Parallel.For((long)0, heightInPixels, y =>
                {
                    byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);
                    for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                    {
                        currentLine[x]     = lut[currentLine[x]];
                        currentLine[x + 1] = lut[currentLine[x + 1]];
                        currentLine[x + 2] = lut[currentLine[x + 2]];
                    }
                });

                System.Windows.Media.PixelFormat wpfFormat;
                switch (bmap.PixelFormat)
                {
                    case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                        wpfFormat = PixelFormats.Bgra32;
                        break;
                    case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                        wpfFormat = PixelFormats.Bgr32;
                        break;
                    default:
                        wpfFormat = PixelFormats.Bgr24;
                        break;
                }

                var wb = new WriteableBitmap(bitmapData.Width, bitmapData.Height, 96, 96, wpfFormat, null);
                wb.WritePixels(new Int32Rect(0, 0, bitmapData.Width, bitmapData.Height),
                    bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

                bmap.UnlockBits(bitmapData);
                wb.Freeze();
                return wb;
            }
        }
    }
}

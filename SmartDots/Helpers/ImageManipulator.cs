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

        public static BitmapImage SetBrightnessContrast(Bitmap img, double b, double c)
        {
            Stopwatch stopwatch = new Stopwatch();

            // Begin timing.
            stopwatch.Start();

            unsafe
            {
                double brightness = b;

                double contrast = c;
                Bitmap bmap = img;
                contrast = (100.0 + contrast) / 100.0;
                contrast *= contrast;
                BitmapData bitmapData = bmap.LockBits(new System.Drawing.Rectangle(0, 0, bmap.Width, bmap.Height),
                    ImageLockMode.ReadWrite, bmap.PixelFormat);

                int bytesPerPixel = Bitmap.GetPixelFormatSize(bmap.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

                Parallel.For((long)0, heightInPixels, y =>
                {
                    byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);

                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        double oldBlue = currentLine[x] + brightness;
                        if (oldBlue < 0) oldBlue = 1;
                        if (oldBlue > 255) oldBlue = 255;
                        oldBlue = oldBlue / 255.0;
                        oldBlue -= 0.5;
                        oldBlue *= contrast;
                        oldBlue += 0.5;
                        oldBlue *= 255;
                        if (oldBlue < 0) oldBlue = 0;
                        if (oldBlue > 255) oldBlue = 255;

                        double oldGreen = currentLine[x + 1] + brightness;
                        oldGreen = oldGreen / 255.0;
                        oldGreen -= 0.5;
                        oldGreen *= contrast;
                        oldGreen += 0.5;
                        oldGreen *= 255;
                        if (oldGreen < 0) oldGreen = 0;
                        if (oldGreen > 255) oldGreen = 255;

                        double oldRed = currentLine[x + 2] + brightness;
                        oldRed = oldRed / 255.0;
                        oldRed -= 0.5;
                        oldRed *= contrast;
                        oldRed += 0.5;
                        oldRed *= 255;
                        if (oldRed < 0) oldRed = 0;
                        if (oldRed > 255) oldRed = 255;

                        currentLine[x] = (byte)oldBlue;
                        currentLine[x + 1] = (byte)oldGreen;
                        currentLine[x + 2] = (byte)oldRed;
                    }
                });
                bmap.UnlockBits(bitmapData);

                // Stop timing.
                stopwatch.Stop();

                // Write result.
                Console.WriteLine("Time elapsed for SetBrightnessContrast: " + stopwatch.Elapsed);
                return BitmapConverter.Bitmap2BitmapImage(bmap);
            }
        }
    }
}

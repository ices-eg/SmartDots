using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math;

namespace SmartDots.Helpers
{
    public static class BitmapConverter
    {
        public static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            try
            {
                //todo memoryexception
                // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

                using (MemoryStream outStream = new MemoryStream())
                {
                    Stopwatch stopwatch = new Stopwatch();

                    // Begin timing.
                    stopwatch.Start();

                    BitmapEncoder enc = new BmpBitmapEncoder();
                    enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                    enc.Save(outStream);
                    Bitmap bitmap = new Bitmap(outStream);

                    // Stop timing.
                    stopwatch.Stop();

                    // Write result.
                    Console.WriteLine("Time elapsed for BitmapImage2Bitmap: {0}", stopwatch.Elapsed);

                    return new Bitmap(bitmap);
                }
            }
            catch (Exception e)
            {
                //32 bit memory exception, best to ignore instead of crashing the application
            }
            return null;
        }

        public static BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            try
            {
                using (var memory = new MemoryStream())
                {
                    // Create new stopwatch.
                    Stopwatch stopwatch = new Stopwatch();

                    // Begin timing.
                    stopwatch.Start();

                    bitmap.Save(memory, ImageFormat.Png);
                    memory.Position = 0;

                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                    // Stop timing.
                    stopwatch.Stop();

                    // Write result.
                    Console.WriteLine("Time elapsed for Bitmap2BitmapImage: {0}", stopwatch.Elapsed);

                    return bitmapImage;
                }
            }
            catch (Exception e)
            {
                //32 bit memory exception, best to ignore instead of crashing the application
            }
            return null;
        }

        public static Bitmap ColorToGrayscale(Bitmap bmp)
        {
            int w = bmp.Width,
            h = bmp.Height,
            r, ic, oc, bmpStride, outputStride, bytesPerPixel;
            PixelFormat pfIn = bmp.PixelFormat;
            ColorPalette palette;
            Bitmap output;
            BitmapData bmpData, outputData;

            //Create the new bitmap
            output = new Bitmap(w, h, PixelFormat.Format8bppIndexed);

            //Build a grayscale color Palette
            palette = output.Palette;
            for (int i = 0; i < 256; i++)
            {
                Color tmp = Color.FromArgb(255, i, i, i);
                palette.Entries[i] = Color.FromArgb(255, i, i, i);
            }
            output.Palette = palette;

            //No need to convert formats if already in 8 bit
            if (pfIn == PixelFormat.Format8bppIndexed)
            {
                output = (Bitmap)bmp.Clone();

                //Make sure the palette is a grayscale palette and not some other
                //8-bit indexed palette
                output.Palette = palette;

                return output;
            }

            //Get the number of bytes per pixel
            switch (pfIn)
            {
                case PixelFormat.Format24bppRgb: bytesPerPixel = 3; break;
                case PixelFormat.Format32bppArgb: bytesPerPixel = 4; break;
                case PixelFormat.Format32bppRgb: bytesPerPixel = 4; break;
                default: throw new InvalidOperationException("Image format not supported");
            }

            //Lock the images
            bmpData = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly,
            pfIn);
            outputData = output.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly,
            PixelFormat.Format8bppIndexed);
            bmpStride = bmpData.Stride;
            outputStride = outputData.Stride;

            //Traverse each pixel of the image
            unsafe
            {
                byte* bmpPtr = (byte*)bmpData.Scan0.ToPointer(),
                outputPtr = (byte*)outputData.Scan0.ToPointer();

                if (bytesPerPixel == 3)
                {
                    //Convert the pixel to it's luminance using the formula:
                    // L = .299*R + .587*G + .114*B
                    //Note that ic is the input column and oc is the output column
                    for (r = 0; r < h; r++)
                        for (ic = oc = 0; oc < w; ic += 3, ++oc)
                            outputPtr[r * outputStride + oc] = (byte)(int)
                            (0.299f * bmpPtr[r * bmpStride + ic] +
                            0.587f * bmpPtr[r * bmpStride + ic + 1] +
                            0.114f * bmpPtr[r * bmpStride + ic + 2]);
                }
                else //bytesPerPixel == 4
                {
                    //Convert the pixel to it's luminance using the formula:
                    // L = alpha * (.299*R + .587*G + .114*B)
                    //Note that ic is the input column and oc is the output column
                    for (r = 0; r < h; r++)
                        for (ic = oc = 0; oc < w; ic += 4, ++oc)
                            outputPtr[r * outputStride + oc] = (byte)(int)
                            ((bmpPtr[r * bmpStride + ic] / 255.0f) *
                            (0.299f * bmpPtr[r * bmpStride + ic + 1] +
                            0.587f * bmpPtr[r * bmpStride + ic + 2] +
                            0.114f * bmpPtr[r * bmpStride + ic + 3]));
                }
            }

            //Unlock the images
            bmp.UnlockBits(bmpData);
            output.UnlockBits(outputData);

            return output;
        }

        public static float AverageBrightness(Bitmap bitmap)
        {
            //var colors = new List<Color>();
            //for (int x = 0; x < bitmap.Size.Width; x++)
            //{
            //    for (int y = 0; y < bitmap.Size.Height; y++)
            //    {
            //        colors.Add(bitmap.GetPixel(x, y));
            //    }
            //}

            //return colors.Average(color => color.GetBrightness());

            ImageStatisticsHSL stat = new ImageStatisticsHSL(bitmap);
            return stat.Luminance.Mean;
            // check mean value of saturation channel
            //if (saturation.Mean > 0.5)
            //{
            //    // do further processing
            //}
        }

        public static Bitmap PreProcessForScaleDetection(BitmapImage bi, int threshold)
        {
            var temp = BitmapImage2Bitmap(bi);
            //temp.Save("step1.jpg");

            Bitmap newImage = ColorToGrayscale(temp);
            //newImage.Save("step2.jpg");

            float averageBrightness = AverageBrightness(temp);

            if (averageBrightness > 0.5)
            {
                
                Invert filter = new Invert();
                filter.ApplyInPlace(newImage);
                //newImage.Save("step3.jpg");
                //newImage.Save("newImage.jpg");
            }
            // create filter
            Threshold filter2 = new Threshold(threshold);
            // apply the filter
            filter2.ApplyInPlace(newImage);
            //newImage.Save("step4.jpg");
            //newImage.Save("bw.jpg");
            return newImage;
        }
    }
}

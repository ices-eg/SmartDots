using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

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
    }
}

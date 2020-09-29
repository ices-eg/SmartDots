using Accord.Imaging;
using Accord.Imaging.Filters;
using IronOcr;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static SmartDots.Helpers.BitmapSourceHelper;

namespace SmartDots.Helpers
{
    public static class ScaleMeasureTool
    {
        public static int Measure(Bitmap bitmap, int thresholdValue)
        {
            int result = 0;
            decimal extractedScale = 0;

            var preProcessedImage = PreProcess(bitmap, thresholdValue);

            Rectangle? rect = ExtractScaleRectangle(preProcessedImage);
            if (rect == null && thresholdValue - 50 >= 28) result = Measure(bitmap, thresholdValue - 50);
            if (rect != null)
            {
                extractedScale = ExtractScaleValue(preProcessedImage, (Rectangle)rect);
            }
            if (extractedScale == 0 || (int)(rect?.Width / extractedScale) == 0)
            {
                rect = ExtractScaleRectangleOld(BitmapConverter.Bitmap2BitmapImage(bitmap));
                if (rect != null)
                {
                    extractedScale = ExtractScaleValue(preProcessedImage, (Rectangle)rect);
                }
            }

            if (rect != null && extractedScale != 0)
            {
                result = (int)(rect?.Width / extractedScale);
            }

            return result;
        }

        public static int Measure(BitmapImage bitmap, int thresholdValue)
        {
            return Measure(BitmapConverter.BitmapImage2Bitmap(bitmap), thresholdValue);
        }

        private static Bitmap PreProcess(Bitmap bitmap, int thresholdValue)
        {
            Grayscale grayscaleFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = grayscaleFilter.Apply(bitmap);
            //grayImage.Save("preprocess-1-grayscale.jpg");

            grayImage = InvertIfNeeded(bitmap, grayImage);
            //grayImage.Save("preprocess-2-invertifneeded.jpg");

            Threshold threshold = new Threshold(thresholdValue);
            Bitmap thresholdImage = threshold.Apply(grayImage);
            //thresholdImage.Save("preprocess-3-threshold-" + thresholdValue + ".jpg");

            return thresholdImage;
        }

        private static Bitmap InvertIfNeeded(Bitmap original, Bitmap grayscale)
        {
            ImageStatisticsHSL stat = new ImageStatisticsHSL(original, grayscale);
            if (stat.Luminance.Mean > 0.5)
            {
                Invert filter = new Invert();
                filter.ApplyInPlace(grayscale);
            }

            return grayscale;
        }

        private static Rectangle? ExtractScaleRectangle(Bitmap bitmap)
        {
            // create an instance of blob counter algorithm
            BlobCounterBase bc = new BlobCounter();
            // set filtering options
            bc.FilterBlobs = true;
            bc.MinWidth = bitmap.Width / 25;
            bc.MinHeight = 0;
            bc.MaxHeight = bitmap.Height / 15;
            bc.MaxWidth = bitmap.Width / 3;
            // set ordering options
            bc.ObjectsOrder = ObjectsOrder.Size;
            // process binary image
            bc.ProcessImage(bitmap);
            //bitmap.Save("blobfilter.jpg");
            List<Blob> allBlobs = bc.GetObjectsInformation().ToList();
            List<Blob> blobs = allBlobs.Where(x => ((double)x.Rectangle.Width / (double)x.Rectangle.Height) > 3.7).OrderByDescending(x => x.Rectangle.Width / x.Rectangle.Height).ToList();
            if(!blobs.Any()) blobs = allBlobs.Where(x => ((double)x.Rectangle.Width / (double)x.Rectangle.Height) > 2).OrderByDescending(x => x.Rectangle.Width / x.Rectangle.Height).ToList();
            // extract the biggest blob
            if (blobs.Count > 0)
            {
                int milimeterPixels = blobs[0].Rectangle.Width;
                try
                {
                    if (milimeterPixels == 0) return null;
                    var rect = new System.Drawing.Rectangle();
                    rect.X = blobs[0].Rectangle.X;
                    if (blobs[0].Rectangle.Y < bitmap.Height / 15) rect.Y = 0;
                    else
                    {
                        rect.Y = blobs[0].Rectangle.Y - bitmap.Height / 15;
                    }
                    if (blobs[0].Rectangle.Y + bitmap.Height / 15 > bitmap.Height)
                    {

                        rect.Height = bitmap.Height / 15 + bitmap.Height - blobs[0].Rectangle.Y;
                    }
                    else
                    {
                        rect.Height = bitmap.Height / 7;
                    }
                    rect.Width = (int)milimeterPixels;
                    if (rect.Y + rect.Height > bitmap.Height) return null;
                    if (rect.X + rect.Width > bitmap.Width) return null;
                    //ScaleRectangle = rect;
                    return rect;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private static decimal ExtractScaleValue(Bitmap bitmap, Rectangle rect)
        {
            AdvancedOcr ocr = new AdvancedOcr();
            ocr.AcceptedOcrCharacters = "123456789.,µmncl";
            ocr.InputImageType = AdvancedOcr.InputTypes.Snippet;
            ocr.DetectWhiteTextOnDarkBackgrounds = false;
            ocr.ReadBarCodes = false;
            ocr.RotateAndStraighten = false;
            Bitmap croppedBitmap = bitmap.Clone(rect, bitmap.PixelFormat);
            //croppedBitmap.Save("croppedbitmap.jpg");
            croppedBitmap = RemoveWideBlobs(croppedBitmap);
            Invert filter = new Invert();
            filter.ApplyInPlace(croppedBitmap);
            //croppedBitmap.Save("croppedbitmap2.jpg");
            var temp = ocr.Read(croppedBitmap);
            var text = temp.Text;
            return ParseScale(text);
        }

        private static decimal ParseScale(string text)
        {
            text = Regex.Replace(text, " ", "");
            if (text.StartsWith(".")) text = text.Substring(1, text.Length - 1);
            if (text.StartsWith(",")) text = text.Substring(1, text.Length - 1);


            double factor = 1;
            int resultIndex = 0;
            if (text.Contains("mm"))
            {
                resultIndex = text.IndexOf("mm");
            }
            else if (text.Contains("cm"))
            {
                factor = 10;
                resultIndex = text.IndexOf("cm");
            }
            else if (text.Contains("µm"))
            {
                factor = 0.001;
                resultIndex = text.IndexOf("µm");
            }
            else if (text.Contains("um"))
            {
                factor = 0.001;
                resultIndex = text.IndexOf("um");
            }
            else if (text.Contains("m")) // asuming mm
            {
                factor = 1;
                resultIndex = text.IndexOf("m");
            }

            if (resultIndex != 0) text = text.Substring(0, resultIndex);

            while (text.Contains(Environment.NewLine))
            {
                text = text.Substring(text.IndexOf(Environment.NewLine) + 2);
            }
            decimal number = 0;

            text = text.Replace(',', '.');
            text = new String(text.Where(Char.IsDigit).ToArray());
            try
            {
                number = decimal.Parse(text, CultureInfo.InvariantCulture);
                return (decimal)(number / (decimal)factor);
            }
            catch (Exception e)
            {
                return 0;
            }

            return 0;
        }

        private static Rectangle? ExtractScaleRectangleOld(BitmapImage image)
        {
            var pixels = GetPixels(image);
            decimal milimeterPixels = 0;
            int x = 0, y = 0;
            for (int i = 0; i < image.PixelHeight; i++)
            {
                for (int j = 0; j < image.PixelWidth; j++)
                {
                    if (pixels[j, i].Red >= 250 && pixels[j, i].Blue >= 250 && pixels[j, i].Green >= 250)
                    {
                        try
                        {
                            for (int k = 1; k < image.PixelHeight - j; k++)
                            {
                                if (j + k < image.PixelWidth &&
                                    (pixels[j + k, i].Red >= 250 || pixels[j + k, i].Blue >= 250 ||
                                     pixels[j + k, i].Green >= 250))
                                {
                                    if (k > milimeterPixels)
                                    {
                                        milimeterPixels = k;
                                        x = j;
                                        y = i;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            //
                        }
                    }
                }
            }
            try
            {
                if (milimeterPixels == 0) return null;
                var rect = new System.Drawing.Rectangle();
                rect.X = x;
                if (y < image.PixelHeight / 15) rect.Y = 0;
                else
                {
                    rect.Y = y - (int)image.PixelHeight / 15;
                }
                if (y + (int)image.PixelHeight / 15 > (int)image.PixelHeight) rect.Height = (int)image.PixelHeight - rect.Y;
                else
                {
                    rect.Height = (int)image.PixelHeight / 15;
                }
                rect.Width = (int)milimeterPixels;
                //rect.Height = (int)image.PixelHeight / 7;
                return rect;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private static Bitmap RemoveWideBlobs(Bitmap bitmap)
        {
            // create filter
            BlobsFiltering filter = new BlobsFiltering();
            // configure filter
            filter.MaxWidth = bitmap.Width / 2;
            // apply the filter
            filter.ApplyInPlace(bitmap);
            return bitmap;
        }

        private static PixelColor[,] GetPixels(BitmapSource source)
        {
            //Application.Current.Dispatcher.Invoke((Action)delegate {

            if (source.Format != PixelFormats.Bgra32)
                source = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);

            int width = source.PixelWidth;
            int height = source.PixelHeight;
            PixelColor[,] result = new PixelColor[width, height];

            BitmapSourceHelper.CopyPixels(source, result, width * 4, 0);

            //});
            return result;
        }
    }
}

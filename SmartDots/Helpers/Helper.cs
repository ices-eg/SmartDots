using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using DevExpress.Mvvm;
using DevExpress.Xpf.WindowsUI;
using SmartDots.Model;
using SmartDots.ViewModel;

namespace SmartDots.Helpers
{
    public static class Helper
    {

        public static float Version { get; } = 4.1f;

        public static void ShowWinUIMessageBox(string message, string caption, MessageBoxButton msgBoxButton, MessageBoxImage img, Exception e = null)
        {
            //Log("errors.txt", message + e?.StackTrace, e);

            try
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {

                    new WinUIMessageBoxService().Show(message, caption, msgBoxButton, img);

                });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }



        }

        public static MessageBoxResult ShowWinUIDialog(string message, string caption, MessageBoxImage img, Exception e = null)
        {
            var messageBoxResult = System.Windows.MessageBoxResult.No;

            Application.Current.Dispatcher.Invoke((Action)delegate
            {

                messageBoxResult = new WinUIMessageBoxService().Show(message, caption, MessageBoxButton.YesNo, img);

            });
            return messageBoxResult;
        }



        public static void Log(string file, string message, Exception e = null, bool logInRelease = false)
        {
#if DEBUG
            try
            {
                using (StreamWriter writer = new StreamWriter(file, true))
                {
                    writer.WriteLine(DateTime.Now + Environment.NewLine + "Computer: " + Environment.MachineName + Environment.NewLine +
                                     "User: " + System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString() + Environment.NewLine +
                                     message + Environment.NewLine + e?.InnerException?.ToString() + Environment.NewLine);
                }
            }
            catch (Exception)
            {

            }
#else

            if (logInRelease)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(file, true))
                    {
                        writer.WriteLine(DateTime.Now + Environment.NewLine + "Computer: " + Environment.MachineName + Environment.NewLine +
                                         "User: " + System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString() + Environment.NewLine +
                                         message + Environment.NewLine + e?.InnerException?.ToString() + Environment.NewLine + e?.StackTrace?.ToString() + Environment.NewLine);
                    }
                }
                catch (Exception)
                {

                }
            }
#endif
        }

        public static void LogWebAPIResult(string result)
        {
#if DEBUG
            try
            {
                using (StreamWriter writer = new StreamWriter("webapi.txt", true))
                {
                    writer.WriteLine(result + Environment.NewLine + Environment.NewLine);
                }
            }
            catch (Exception)
            {

            }
#endif
        }

        public static object ConvertType(object sourceObject, Type targetType)
        {
            var obj = Activator.CreateInstance(targetType);

            var sourceProps = sourceObject.GetType().GetProperties().Where(x => x.CanRead).ToList();
            var targetProps = targetType.GetProperties().Where(x => x.CanWrite).ToList();
            foreach (var prop in targetProps)
            {
                var propMatch = sourceProps.FirstOrDefault(x => x.Name.ToLower() == prop.Name.ToLower());
                if (propMatch == null) continue;
                try
                {
                    var value = propMatch.GetValue(sourceObject);
                    prop.SetValue(obj, value);
                }
                catch { }
            }

            var sourceFields = sourceObject.GetType().GetFields().Where(x => x.IsPublic).ToList();
            var targetFields = targetType.GetFields().Where(x => x.IsPublic).ToList();
            foreach (var prop in targetFields)
            {
                var propMatch = sourceFields.FirstOrDefault(x => x.Name.ToLower() == prop.Name.ToLower());
                if (propMatch == null) continue;
                try
                {
                    var value = propMatch.GetValue(sourceObject);
                    prop.SetValue(obj, value);
                }
                catch { }
            }

            return obj;
        }

        public static dynamic ToDynamic(this object value)
        {
            IDictionary<string, object> expando = new ExpandoObject();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
                expando.Add(property.Name, property.GetValue(value));

            return expando as ExpandoObject;
        }

        public static T DeepCopy<T>(T item)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, item);
            stream.Seek(0, SeekOrigin.Begin);
            T result = (T)formatter.Deserialize(stream);
            stream.Close();
            return result;
        }

        public static void DoAsync(Action action)
        {
            new System.Threading.Tasks.Task(action).Start();
        }

        public static Size MeasureString(TextBlock tb)
        {
            var formattedText = new FormattedText(
                tb.Text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch),
                tb.FontSize,
                Brushes.Black,
                new NumberSubstitution());

            return new Size(formattedText.Width, formattedText.Height);
        }

        public static List<string> MultiUserColors { get; } = new List<string>() {
            "#00FFFF",
            "#FFFF00",
            "#FF0000",
            "#FF8000",
            "#8000FF",
            "#00FF00",
            "#80FF00",
            "#FF0080",
            "#0080FF",
            "#00FF80",
            "#8080FF",
            "#FF8080",
            "#80FF80",
            "#44DDDD",
            "#DDDD44",
            "#DD4444",
            "#DD8044",
            "#8440DD",
            "#44DD44",
            "#80DD44",
            "#DD4480",
            "#4480DD",
            "#44DD80",
            "#8080DD",
            "#DD8080",
            "#80DD80",
        };

        public static Dictionary<Guid, string> MultiUserColorsDict { get; } = new Dictionary<Guid, string>();

        public static bool FolderExists(string path)
        {
            if (path.StartsWith("http"))
            {
                //todo have to implement
                return true;
            }
            else
            {
                return Directory.Exists(path);
            }
        }

        public static string DownloadImages(Guid analysisid, List<string> images)
        {
            try
            {
                Directory.CreateDirectory($@"temp\{analysisid.ToString()}");
                DirectoryInfo di = new DirectoryInfo($@"temp\{analysisid.ToString()}");

                int totalFiles = images.Count;
                int downloadedFiles = 0;
                int alreadyExistingFiles = 0;

                foreach (var img in images)
                {

                    bool isUncPath = img.StartsWith("\\");
                    var filename = "";
                    if (img.Contains("/"))
                    {
                        filename = img.Split('/').Last();
                    }
                    else if (img.Contains("\\"))
                    {
                        filename = img.Split('\\').Last();
                    }

                    var localFileLocation = $@"temp\{analysisid.ToString()}" + filename;

                    if (!System.IO.File.Exists(localFileLocation))
                    {
                        CopyFileAsync(img, localFileLocation);
                        downloadedFiles++;
                    }
                    else
                    {
                        alreadyExistingFiles++;
                    }
                    if (!System.IO.File.Exists(localFileLocation)) CopyFileAsync(img, localFileLocation);

                }

                var message = "";
                if (downloadedFiles > 0)
                {
                    message = $"Started downloading {downloadedFiles} images...";
                }
                if (alreadyExistingFiles == totalFiles)
                {
                    message = $"All images are already available on disk, no action required";
                }

                return message;
            }
            catch (Exception e)
            {
                // ignored
                return "";

            }
        }

        public static async Task CopyFileAsync(string sourceFile, string destinationFile)
        {
            if (sourceFile.StartsWith("http"))
            {
                using (var client = new WebClient())
                {
                    try
                    {
                        client.DownloadFile(sourceFile, destinationFile);
                    }
                    catch (Exception)
                    {
                        //
                    }

                }
            }
            else
            {
                using (var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan))
                using (var destinationStream = new FileStream(destinationFile, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan))
                    await sourceStream.CopyToAsync(destinationStream);
            }
        }

        public static bool ClearCache(string folder = null)
        {
            if(folder == null)
            {
                folder = "temp";
            }
            string size = "";
            var totalBytes = GetTotalFileSize(folder);
            if (totalBytes == 0)
            {
                ShowWinUIMessageBox("The cache is already empty", "Clear cache", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            if (totalBytes > 1024)
            {
                size = (totalBytes / 1024).ToString() + " KB";
            }
            if (totalBytes > 1024 * 1024)
            {
                size = (totalBytes / (1024 * 1024)).ToString() + " MB";
            }
            if (totalBytes > 1024 * 1024 * 1024)
            {
                size = (totalBytes / (1024 * 1024 * 1024)).ToString() + " GB";
            }
            
            var result = ShowWinUIDialog($"The cache contains {size} on image data, do you want to clear it?", "Clear cache", MessageBoxImage.Question);

            if(result == MessageBoxResult.Yes)
            {
                try
                {
                    Directory.Delete(folder, true);
                    Directory.CreateDirectory("temp");
                    return true;
                }
                catch (Exception e)
                {
                    //Log("errors.txt", "Error clearing cache", e);
                    return true;
                }
            }
            return false;
        }

        public static long GetTotalFileSize(string folderPath)
        {
            long totalSize = 0;
            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
            if(directoryInfo.Exists == false)
            {
                return 0;
            }
            FileInfo[] files = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (FileInfo file in files)
            {
                totalSize += file.Length;
            }
            return totalSize;
        }

        public static bool IsUserVisible(FrameworkElement element, FrameworkElement container)
        {
            if (!element.IsVisible)
                return false;

            try
            {
                Rect bounds = element.TransformToAncestor(container).TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));
                Rect rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);
                return rect.Contains(bounds.TopLeft) || rect.Contains(bounds.BottomRight);
            }
            catch (Exception)
            {
                return true;
            }
            
        }
    }
}

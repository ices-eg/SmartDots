﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Xpf.WindowsUI;

namespace SmartDots.Helpers
{
    public static class Helper
    {
        
        public static void ShowWinUIMessageBox(string message, string caption, MessageBoxButton msgBoxButton, MessageBoxImage img, Exception e = null)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {

                new WinUIMessageBoxService().Show(message, caption, msgBoxButton, img);

            });
            

            //try
            //{
            //    using (StreamWriter writer = new StreamWriter(@"\\clo.be\dfs\Data\data_d1\software\smartdots\errorlogs.txt", true))
            //    {
            //        writer.WriteLine(DateTime.Now + Environment.NewLine + "Computer: " + Environment.MachineName + Environment.NewLine +
            //            "User: " + System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString() + Environment.NewLine +
            //            "Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + Environment.NewLine +
            //            message + Environment.NewLine + e?.ToString() + Environment.NewLine);
            //    }
            //}
            //catch (Exception)
            //{

            //}
        }

        public static object ConvertType(object sourceObject, Type targetType)
        {
            var obj = Activator.CreateInstance(targetType);

            var sourceProps = sourceObject.GetType().GetProperties().Where(x => x.CanRead).ToList();
            var targetProps = targetType.GetProperties().Where(x => x.CanWrite).ToList();
            foreach (var prop in targetProps)
            {
                var propMatch = sourceProps.FirstOrDefault(x => x.Name == prop.Name);
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
                var propMatch = sourceFields.FirstOrDefault(x => x.Name == prop.Name);
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

        public static string WindowsSecurityConnectionString;
        public static string SecurityConnectionString;
        public static string SmartDotConnectionString;
    }
}
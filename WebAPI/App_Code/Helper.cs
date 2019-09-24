using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.App_Code
{
    public static class Helper
    {
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
    }
}
using Accelerate.Foundations.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Helpers
{
    public static class ObjectHelper
    {
        public class ObjectConvertInfo
        {
            public string Base { get; set; }
            public string Prefix { get; set; }
            public string Postfix { get; set; }
            public object ConvertObject { set; get; }
            public IList<Type> IgnoreTypes { set; get; }
            public IList<string> IgnoreProperties { set; get; }
            public int MaxDeep { set; get; } = 3;
        }
        //https://stackoverflow.com/questions/31314649/convert-nested-class-to-dictionary
        public static Dictionary<string, string> ConvertObjectToDictionary(object data)
        {
            var objectConvertInto = new ObjectConvertInfo
            {
                Base = null,
                Prefix = "$",
                Postfix = "$",
                ConvertObject = data,
                IgnoreProperties = new List<string> { "IgnorePropertyA" },
                IgnoreTypes = new List<Type> { typeof(Array), typeof(IntPtr), typeof(Delegate), typeof(Type) },
                MaxDeep = 4
            };
            return ConvertObjectToDictionary(objectConvertInto);
        }
        public static Dictionary<string, T> ToPropertyDictionary<T>(this object obj)
        {
            var dictionary = new Dictionary<string, T>();
            foreach (var propertyInfo in obj.GetType().GetProperties())
                if (propertyInfo.CanRead && propertyInfo.GetIndexParameters().Length == 0)
                    dictionary[propertyInfo.Name] = (T)propertyInfo.GetValue(obj, null);
            return dictionary;
        }
        public static Dictionary<string, string> ConvertObjectToDictionary(ObjectConvertInfo objectConvertInfo)
        {
            try
            {
                var dictionary = new Dictionary<string, string>();
                MapToDictionaryInternal(dictionary, objectConvertInfo, objectConvertInfo.Base, 0);
                return dictionary;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private static void MapToDictionaryInternal(IDictionary<string, string> dictionary, ObjectConvertInfo objectConvertInfo, string name, int deep)
        {
            try
            {
                if (deep > objectConvertInfo.MaxDeep)
                    return;

                var properties = objectConvertInfo.ConvertObject.GetType().GetProperties();
                foreach (var propertyInfo in properties)
                {
                    if (objectConvertInfo.IgnoreProperties.ContainIgnoreCase(propertyInfo.Name))
                        continue;

                    var key = string.IsNullOrEmpty(name) ? propertyInfo.Name.ToLower() : $"{name.ToLower()}.{propertyInfo.Name.ToLower()}";
                    var value = propertyInfo.GetValue(objectConvertInfo.ConvertObject, null);
                    if (value == null)
                        continue;

                    var valueType = value.GetType();

                    if (objectConvertInfo.IgnoreTypes.Contains(valueType))
                        continue;

                    if (valueType.IsPrimitive || valueType == typeof(string))
                    {
                        dictionary[$"{objectConvertInfo.Prefix}{key}{objectConvertInfo.Postfix}"] = value.ToString();
                    }
                    else if (value is IEnumerable<object>)
                    {
                        var i = 0;
                        foreach (var data in (IEnumerable<object>)value)
                        {
                            MapToDictionaryInternal(dictionary, new ObjectConvertInfo
                            {
                                Prefix = objectConvertInfo.Prefix,
                                Postfix = objectConvertInfo.Postfix,
                                Base = objectConvertInfo.Base,
                                ConvertObject = data,
                                IgnoreTypes = objectConvertInfo.IgnoreTypes,
                                IgnoreProperties = objectConvertInfo.IgnoreProperties,
                                MaxDeep = objectConvertInfo.MaxDeep
                            }, $"{key}.[{i}]", deep + 1);
                            i++;
                        }
                    }
                    else
                    {
                        MapToDictionaryInternal(dictionary, new ObjectConvertInfo
                        {
                            Prefix = objectConvertInfo.Prefix,
                            Postfix = objectConvertInfo.Postfix,
                            Base = objectConvertInfo.Base,
                            ConvertObject = value,
                            IgnoreTypes = objectConvertInfo.IgnoreTypes,
                            IgnoreProperties = objectConvertInfo.IgnoreProperties,
                            MaxDeep = objectConvertInfo.MaxDeep
                        }, key, deep + 1);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}

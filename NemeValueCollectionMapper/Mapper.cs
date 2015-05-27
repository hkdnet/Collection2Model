using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using System.Reflection;

namespace Collection2Model.Mapper
{
    public static class Mapper
    {
        public static T MappingFromNameValueCollection<T>(NameValueCollection c)
            where T : class, new()
        {
            var ignoring = new List<string>();
            return MappingFromNameValueCollection<T>(c, ignoring);
        }
        public static T MappingFromNameValueCollection<T>(NameValueCollection c, List<String> ignoring)
            where T : class, new()
        {
            var ret = new T();
            var properties = from p in GetTargetProps(typeof(T))
                             where c[p.Name] != null
                                && !ignoring.Contains(p.Name, StringComparer.OrdinalIgnoreCase)
                             select p;
            foreach (var p in properties)
            {
                var strVal = c[p.Name];
                var val = Convert.ChangeType(strVal, p.PropertyType);
                p.SetValue(ret, val, null);
            }
            return ret;
        }
        /// <summary>
        /// return properties to be mapped based on static analysis
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static IEnumerable<PropertyInfo> GetTargetProps(Type t)
        {
            return from p in t.GetProperties()
                   where !HasIgnoreAttribute(p)
                      && p.CanWrite
                   select p;
        }
        private static bool HasIgnoreAttribute(PropertyInfo p)
        {
            var attrs = Attribute.GetCustomAttributes(p, typeof(IgnorePropertyAttribute));
            var attr = attrs.FirstOrDefault() as IgnorePropertyAttribute;
            return attr != null;
        }
    }


}

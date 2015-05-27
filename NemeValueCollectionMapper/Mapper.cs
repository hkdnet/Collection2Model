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
            var properties = typeof(T).GetProperties();
            foreach (var p in properties)
            {
                if (HasIgnoreAttribute(p))
                {
                    continue;
                }
                var strVal = c[p.Name];
                var isIgnored = ignoring.Contains(p.Name, StringComparer.OrdinalIgnoreCase);
                if (!p.CanWrite || strVal == null || isIgnored)
                {
                    continue;
                }
                var val = Convert.ChangeType(strVal, p.PropertyType);
                p.SetValue(ret, val, null);
            }
            return ret;
        }
        private static bool HasIgnoreAttribute(PropertyInfo p)
        {
            var attrs = Attribute.GetCustomAttributes(p, typeof(IgnorePropertyAttribute));
            var attr = attrs.FirstOrDefault() as IgnorePropertyAttribute;
            return attr != null;
        }
    }


}

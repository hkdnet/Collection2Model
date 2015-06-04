using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace Collection2Model.Mapper
{
    public static class Mapper
    {
        public static T MappingFromNameValueCollection<T>(NameValueCollection c)
            where T : class, new()
        {
            var ret = new T();
            var properties = from p in GetTargetProps(typeof(T))
                             select p;
            foreach (var p in properties)
            {
                ret = Validate<T>(ret, p, c[p.Name]);
            }
            return ret;
        }


        private static T Validate<T>(T ret, PropertyInfo p, string strVal)
            where T : class, new()
        {
            // is required?
            RequireValidate(p, strVal);
            if (strVal == null)
            {
                return ret;
            }

            // format ok?
            var val = Convert.ChangeType(strVal, p.PropertyType);
            p.SetValue(ret, val, null);

            // valid to meta-data?
            ValueValidate(p, val);
            return ret;
        }


        private static void RequireValidate(PropertyInfo p, string strVal)
        {
            var attr = (RequiredAttribute)Attribute.GetCustomAttributes(p, typeof(RequiredAttribute)).FirstOrDefault();
            if (attr != null)
                attr.Validate(strVal, attr.ErrorMessage);
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
        private static void ValueValidate(PropertyInfo p, Object val)
        {
            var attrs = from attr in Attribute.GetCustomAttributes(p, typeof(ValidationAttribute))
                        select (ValidationAttribute)attr;
            foreach (var attr in attrs)
            {
                attr.Validate(val, attr.ErrorMessage);
            }
        }
    }
}

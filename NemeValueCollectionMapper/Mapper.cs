using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NemeValueCollectionMapper
{
    public static class Mapper
    {
        public static T Mapping<T>(System.Collections.Specialized.NameValueCollection c)
            where T : class, new()
        {
            var ret = new T();
            var properties = typeof(T).GetProperties();
            foreach (var p in properties)
            {
                var strVal = c[p.Name];
                if (!p.CanWrite || strVal == null)
                {
                    continue;
                }
                if (p.PropertyType.FullName.Equals("System.String"))
                {
                    p.SetValue(ret, strVal, null);
                }
                var val = Convert.ChangeType(strVal, p.PropertyType);
                p.SetValue(ret, val, null);
            }
            return ret;
        }
    }
}

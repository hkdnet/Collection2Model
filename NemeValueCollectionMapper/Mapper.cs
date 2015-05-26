using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection2Model.Mapper
{
    public static class Mapper
    {
        public static T MappingFromNameValueCollection<T>(System.Collections.Specialized.NameValueCollection c)
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
                var val = Convert.ChangeType(strVal, p.PropertyType);
                p.SetValue(ret, val, null);
            }
            return ret;
        }
    }
}

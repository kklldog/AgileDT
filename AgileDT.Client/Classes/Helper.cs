using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace AgileDT.Client.Classes
{
    public class Helper
    {
        internal static List<Type> ScanAll()
        {
            var ieventT = typeof(IEventService);
            var ass =  AppDomain.CurrentDomain.GetAssemblies();

            var list = new List<Type>();
            foreach (var ab in ass)
            {
                foreach (var t in ab.GetTypes())
                {
                    var method = GetBizMethod(t);
                    if (method != null && t.IsClass && ieventT.IsAssignableFrom(t))
                    {
                        list.Add(t);
                    }
                }
            }

            return list;
        }

        internal static string GetEventName(Type t)
        {
            return t.Name;
        }

        public static MethodInfo GetBizMethod(Type t)
        {
            var methods = t.GetMethods();
            foreach (var item in methods)
            {
                var attr = item.GetCustomAttribute<DtEventBizMethodAttribute>();
                if (attr != null)
                {
                    return item;
                }
            }

            return null;
        }

        public static DtEventBizMethodAttribute GetDtEventBizMethodAttribute(Type t)
        {
            var method = GetBizMethod(t);
            var attr = method.GetCustomAttribute<DtEventBizMethodAttribute>();

            return attr;
        }
    }
}

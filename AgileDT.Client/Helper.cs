using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace AgileDT.Client
{
    public class Helper
    {
        public static List<Type> ScanAll()
        {
            var ieventT = typeof(IEvent);
            var ass =  AppDomain.CurrentDomain.GetAssemblies();

            var list = new List<Type>();
            foreach (var ab in ass)
            {
                foreach (var t in ab.GetTypes())
                {
                    if (t.IsClass && ieventT.IsAssignableFrom(t))
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

        internal static MethodInfo GetBizMethod(Type t)
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
    }
}

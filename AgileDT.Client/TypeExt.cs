using AgileDT.Client.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgileDT.Client
{
    static class  TypeExt
    {
        public static string GetEventName(this Type t)
        {
            var attr = Helper.GetDtEventNameAttribute(t);

            if (attr != null)
            {
                if (!string.IsNullOrEmpty(attr.EventName))
                {
                    return attr.EventName;
                }
            }

            return t.Name.Replace("_agiledt_proxy","");
        }
    }
}

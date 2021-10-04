using System;
using System.Collections.Generic;
using System.Text;

namespace AgileDT.Client
{
    class ClassTemplate
    {
        public static string ClassTemp =
            @"
        @using 
        using System;
        namespace @ns {
            public class @newClassName : @sourceClassName {

                @ctrs

                @methodName {
                    Console.WriteLine(""before"");

                    var ret = base.@bizMethodName(@bizMethodCallParams);

                    Console.WriteLine(""after"");

                    return ret;
                }
            }
        }
        ";
    }
}

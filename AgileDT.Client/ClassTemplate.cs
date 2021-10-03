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
            public class @newClassName : @sourceClassNAme {

                @ctrs

                public override bool @bizMethodName (Order order) {
                    Console.WriteLine(""before"");

                    var ret = base.@bizMethodName(order);

                    Console.WriteLine(""after"");

                    return ret;
                }
            }
        }
        ";
    }
}

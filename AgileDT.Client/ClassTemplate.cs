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
        using AgileDT.Client;
        namespace @ns {
            public class @newClassName : @sourceClassName {

                @ctrs

               public override @returnType @methodName {
                    var context = new DtEventContext(this);
                    var atr = AgileDT.Client.Helper.GetDtEventBizMethodAttribute(typeof(@sourceClassName));
                    atr.SetContext(context);
                    atr.Before();
                    @returnType ret;
                    try
                    {
                        ret = base.@bizMethodName(@bizMethodCallParams);
                    }
                    catch
                    {
                        if(Guid.TryParse(EventId,out Guid id)) {
                             string sql = ""delete from event_message where event_id = '""+EventId+""' "";
                                                    FREESQL.Instance.Ado.ExecuteNonQuery(sql, new
                                                    {
                                                    });
                        }
                       

                        throw;
                    }


                    atr.After();

                    return ret;
                }
           
            }
        }
        ";
    }
}

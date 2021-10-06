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
                    var atr = AgileDT.Client.Helper.GetDtEventBizMethodAttribute(typeof(@sourceClassName));
                    atr.SetService(this);
                    atr.Before();
                    this.EventId = atr.GetEventId();
                    @returnType ret;
                    try
                    {
                        ret = base.@bizMethodName(@bizMethodCallParams);
                    }
                    catch
                    {
                        const string sql = ""update[EVENT_MESSAGE] set[STATUS] = @status where event_id = @id "";
                        FREESQL.Instance.Ado.ExecuteNonQuery(sql, new
                        {
                            id = EventId,
                            status = AgileDT.Client.MessageStatus.Cancel
                        });

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


//using System;
//using System;

//using System;
//using AgileDT.Client;

//namespace AgileDT.Client.Tests
//{
//    public class Test1_agiledt_proxy : Test1
//    {

//        public Test1_agiledt_proxy() : base() { }


//        public override void Add(String str)
//        {
//            var context = new DtEventContext(this);
//            var atr = AgileDT.Client.Classes.Helper.GetDtEventBizMethodAttribute(typeof(Test1));
//            atr.SetContext(context);
//            atr.Before();

//            try
//            {
//                base.Add(str);
//            }
//            catch
//            {
//                if (Guid.TryParse(EventId, out Guid id))
//                {
//                    string sql = "delete from event_message where event_id = '" + EventId + "' ";
//                    AgileDT.Client.Data.FREESQL.Instance.Ado.ExecuteNonQuery(sql, new
//                    {
//                    });
//                }


//                throw;
//            }


//            atr.After();


//        }

//    }
//}
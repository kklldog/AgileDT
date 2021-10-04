using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgileDT.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgileDT.Client.Tests
{
    [TestClass()]
    public class ClassProxyCreatorTests
    {
        public class Test1:IEvent
        {
            public string EventId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            [DtEventBizMethod]
            public bool Add(string str)
            {
                return true;
            }

            public MessageStatus QueryEventStatus(string eventId)
            {
                throw new NotImplementedException();
            }
        }


        [TestMethod()]
        public void CreateStringClassTest()
        {
            var creator = new ClassProxyCreator();
            var classStr = creator.CreateStringClass(typeof(Test1));

            Console.WriteLine(classStr);
        }
    }
}
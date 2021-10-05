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

        public class Test2: IEvent
        {
            public string EventId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            [DtEventBizMethod]
            public bool Add(List<string> str)
            {
                return true;
            }

            public MessageStatus QueryEventStatus(string eventId)
            {
                throw new NotImplementedException();
            }
        }

        public class Test3 : IEvent
        {
            public Test3(string str)
            {

            }

            public string EventId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            [DtEventBizMethod]
            public bool Add(List<string> str)
            {
                return true;
            }

            public MessageStatus QueryEventStatus(string eventId)
            {
                throw new NotImplementedException();
            }
        }

        public class Test4 : IEvent
        {
            public Test4(List<string> str)
            {

            }

            public string EventId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            [DtEventBizMethod]
            public bool Add(List<string> str)
            {
                return true;
            }

            public MessageStatus QueryEventStatus(string eventId)
            {
                throw new NotImplementedException();
            }
        }

        public class Test5 : IEvent
        {
            public Test5(List<string> str,int a)
            {

            }

            public string EventId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            [DtEventBizMethod]
            public bool Add(List<string> str)
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

            var classStr2 = creator.CreateStringClass(typeof(Test2));
            Console.WriteLine(classStr2);

            var classStr3 = creator.CreateStringClass(typeof(Test3));
            Console.WriteLine(classStr3);

            var classStr4 = creator.CreateStringClass(typeof(Test4));
            Console.WriteLine(classStr4);

            var classStr5 = creator.CreateStringClass(typeof(Test5));
            Console.WriteLine(classStr5);
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using AgileDT.Client.Data;
using AgileDT.Client.Classes;

namespace AgileDT.Client.Tests
{
    public class Test1 : IEventService
    {
        public string EventId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        [DtEventBizMethod]
        public virtual void Add(string str)
        {
        }

        public string GetBizMsg()
        {
            throw new NotImplementedException();
        }

        public MessageStatus QueryEventStatus(string eventId)
        {
            throw new NotImplementedException();
        }
    }

    public class Test2 : IEventService
    {
        public string EventId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        [DtEventBizMethod]
        public virtual int Add(List<string> str)
        {
            return 0;
        }

        public string GetBizMsg()
        {
            throw new NotImplementedException();
        }

        public MessageStatus QueryEventStatus(string eventId)
        {
            throw new NotImplementedException();
        }
    }

    public class Test3 : IEventService
    {
        public Test3(string str)
        {

        }

        public string EventId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        [DtEventBizMethod]
        public virtual object Add(List<string> str)
        {
            return true;
        }

        public string GetBizMsg()
        {
            throw new NotImplementedException();
        }

        public MessageStatus QueryEventStatus(string eventId)
        {
            throw new NotImplementedException();
        }
    }

    public class Test4 : IEventService
    {
        public Test4(List<string> str)
        {

        }

        public string EventId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        [DtEventBizMethod]
        public virtual string Add(List<string> str)
        {
            return "";
        }

        public string GetBizMsg()
        {
            throw new NotImplementedException();
        }

        public MessageStatus QueryEventStatus(string eventId)
        {
            throw new NotImplementedException();
        }
    }

    public class Test5 : IEventService
    {
        public Test5(List<string> str, int a)
        {

        }

        public string EventId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        [DtEventBizMethod]
        public virtual Test1 Add(List<string> str)
        {
            return null;
        }

        public string GetBizMsg()
        {
            throw new NotImplementedException();
        }

        public MessageStatus QueryEventStatus(string eventId)
        {
            throw new NotImplementedException();
        }
    }

    public class Test6 : IEventService
    {
        public Test6(List<string> str, int a)
        {

        }

        public string EventId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        [DtEventBizMethod]
        public virtual Test2 Add(List<string> str, int a, List<int> b)
        {
            return null;
        }

        public string GetBizMsg()
        {
            throw new NotImplementedException();
        }

        public MessageStatus QueryEventStatus(string eventId)
        {
            throw new NotImplementedException();
        }
    }

    public class Test7 : IEventService
    {
        public Test7(List<List<string>> str)
        {
        }

        public string EventId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        [DtEventBizMethod]
        public virtual List<string> Add(List<string> str)
        {
            return null;
        }

        public string GetBizMsg()
        {
            throw new NotImplementedException();
        }

        public MessageStatus QueryEventStatus(string eventId)
        {
            throw new NotImplementedException();
        }
    }

    public class Test8 : IEventService
    {
        public Test8(List<List<string>> str, List<int> a, Dictionary<string, int> b, string c)
        {
        }
        public string EventId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        [DtEventBizMethod]
        public virtual Action<Test1, Test2> Add(List<string> str)
        {
            return null
                ;
        }

        public string GetBizMsg()
        {
            throw new NotImplementedException();
        }

        public MessageStatus QueryEventStatus(string eventId)
        {
            throw new NotImplementedException();
        }
    }

    public class Test9 : IEventService
    {
        public Test9(List<List<string>> str, List<int> a, Dictionary<string, int> b, string c)
        {
        }
        public string EventId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        [DtEventBizMethod]
        public virtual bool Add(List<List<string>> str, List<int> a, Dictionary<string, int> b, string c)
        {
            return true;
        }

        public string GetBizMsg()
        {
            throw new NotImplementedException();
        }

        public MessageStatus QueryEventStatus(string eventId)
        {
            throw new NotImplementedException();
        }
    }
    [TestClass()]
    public class ClassProxyCreatorTests
    {
       


        [TestMethod()]
        public void CreateStringClassTest()
        {
            var creator = new ClassProxyCreator("testlib");
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

            var classStr6 = creator.CreateStringClass(typeof(Test6));
            Console.WriteLine(classStr6);

            var classStr7 = creator.CreateStringClass(typeof(Test7));
            Console.WriteLine(classStr7);

            var classStr8 = creator.CreateStringClass(typeof(Test8));
            Console.WriteLine(classStr8);

            var classStr9 = creator.CreateStringClass(typeof(Test9));
            Console.WriteLine(classStr9);
        }

        [TestMethod()]
        public void CreateProxyAssemblyTest()
        {
            var creator = new ClassProxyCreator("testlib");
            var ass = creator.CreateProxyAssembly(new List<Type> { 
                typeof(Test1),
                typeof(Test2),typeof(Test3),typeof(Test4),typeof(Test5),typeof(Test6),typeof(Test7),typeof(Test8),typeof(Test9),
            });

            Assert.IsNotNull(ass);
        }
    }
}
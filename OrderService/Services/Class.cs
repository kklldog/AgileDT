
//using Microsoft.Extensions.Logging;
//using OrderService.Data.entities;

//using System;
//namespace OrderService.Services
//{
//    public class AddOrderEventService_Proxy : AddOrderEventService
//    {

//        public AddOrderEventService_Proxy(ILogger<AddOrderEventService> logger) : base(logger) { }


//        public override bool AddOrder(Order order)
//        {
//            Console.WriteLine("before");

//            var ret = base.AddOrder(order);

//            Console.WriteLine("after");

//            return ret;
//        }
//    }
//}

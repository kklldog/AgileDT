using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgileDT.Client.Sgr
{
    class RetryPolicy : IRetryPolicy
    {
        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            var count = retryContext.PreviousRetryCount / 50;
            if (count < 1)//重试次数<50,间隔1s
            {
                return new TimeSpan(0, 0, 1);
            }
            else if (count < 5)//重试次数<250:间隔30s
            {
                return new TimeSpan(0, 0, 30);
            }
            else //重试次数>250:间隔1m
            {
                return new TimeSpan(0, 1, 0);
            }
        }
    }
}

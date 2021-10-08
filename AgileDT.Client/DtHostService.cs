using AgileDT.Client.Sgr;
using Microsoft.CodeAnalysis.Host;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AgileDT.Client
{
    public class DtHostedService : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await SgrClient.Instance.ConnectAsync();
            SgrClient.Instance.AddMessageHandler(new QueryStatusMessageHandler());
            var types = Helper.ScanAll();
            types.ForEach(x =>
            {
                SgrClient.Instance.SendMessageToHub("RegisterEvent", x.Name);
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

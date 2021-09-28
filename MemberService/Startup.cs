using AgileHttp;
using MemberService.MessageQueue;
using MemberService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemberService
{
    public class Startup
    {
        class ApiResult
        {
            public bool success { get; set; }
        }

        public Startup(IConfiguration configuration)
        {
            Config.Instance = configuration;
            Configuration = configuration;

            MQ.Receive((m, args) => {
                var message = Encoding.UTF8.GetString(args.Body.ToArray());
                Console.WriteLine($"收到消息： {message}");
                var service = new PointHistoryService();
                var ret = service.HanldeMessage(message);
                if (ret)
                {
                    dynamic obj = JsonConvert.DeserializeObject<dynamic>(message);
                    string eventId = obj.EventId;
                    var url = "http://localhost:5000/api/Message";
                    using var resp = url.AsHttpClient()
                    .Config(new RequestOptions { 
                        ContentType ="application/json"
                    })
                    .Post(new
                    {
                        eventId = eventId,
                        status = 4
                    }) ;
                    var result = resp.Deserialize<ApiResult>();
                    if (result.success)
                    {
                        m.BasicAck(args.DeliveryTag, false);
                    }
                }
            });
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

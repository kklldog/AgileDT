using AgileDT.Client.Data;
using AgileHttp;
using System;

namespace AgileDT.Client
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DtEventBizMethodAttribute : Attribute
    {
        private DtEventContext _dtEventContext;

        public void SetContext(DtEventContext context)
        {
            _dtEventContext = context;
        }

        private string ServerBaseUrl
        {
            get
            {
                var agiledtServer = Config.Instance["agiledt:server"];
                agiledtServer = agiledtServer.TrimEnd('/');

                return agiledtServer;
            }
        }

        public virtual void Before()
        {
            Console.WriteLine("DtEventBizMethodAttribute Before");

            var eventMsg = new EventMessage
            {
                EventId = _dtEventContext.EventId,
                Status = MessageStatus.Prepare,
                BizMsg = "",
                CreateTime = DateTime.Now,
                EventName = _dtEventContext.Service.GetType().GetEventName()
            };
            try
            {
                FREESQL.Instance.Ado.Transaction(() =>
                {
                    //1. 往event_message表写 Prepare 数据
                    FREESQL.Instance.Insert(eventMsg).ExecuteAffrows();

                    //2. 调用可靠消息服务的接口把 prepare 状态传递过去
                    using var resp = (ServerBaseUrl + "/api/message")
                        .AsHttpClient()
                        .Config(new RequestOptions
                        {
                            ContentType = "application/json"
                        })
                        .Post(eventMsg);
                    if (resp.Exception != null)
                    {
                        throw resp.Exception;
                    }
                    if (resp.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        throw new Exception("send Prepare message to agile_dt_server fail .");
                    }
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual  void After()
        {
            Console.WriteLine("DtEventBizMethodAttribute After");

            //4. 业务执行成功，发送 done 消息
            var doneMsg = new EventMessage
            {
                EventId = _dtEventContext.EventId,
                Status = MessageStatus.Done,
                BizMsg = _dtEventContext.Service.GetBizMsg()
            };

            using var resp = (ServerBaseUrl + "/api/message")
                     .AsHttpClient()
                     .Config(new RequestOptions
                     {
                         ContentType = "application/json"
                     })
                     .Post(doneMsg);
            if (resp.Exception != null)
            {
                throw resp.Exception;
            }
            if (resp.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception("send done message to agile_dt_server fail .");
            }
        }
    }

}

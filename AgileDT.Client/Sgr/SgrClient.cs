using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AgileDT.Client.Sgr
{
    public class SgrClient
    {
        public static SgrClient Instance {
            get;
        }
        static SgrClient()
        {
            Instance = new SgrClient();
        }

        private HubConnection _connection;
        private SgrClient()
        {
            _connection = new HubConnectionBuilder()
            .WithUrl(new Uri($"{ServerBaseUrl}/sgr/hub/message"))
            .WithAutomaticReconnect()
            .Build();

            _connection.Closed += async (error) =>
            {
                Console.WriteLine("signalR connect occur an error {0} .", error);
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await _connection.StartAsync();
            };

            _connection.Reconnected += async (str) => {
                Console.WriteLine("signalR Reconnected .");
                await Task.CompletedTask;
            };

            _connection.Reconnecting += async (error) => {
                Console.WriteLine("signalR Reconnecting , error {0} .", error);
                await Task.CompletedTask;
            };

        }

        public Task ConnectAsync()
        {
            return _connection?.StartAsync();
        }

        public void AddMessageHandler(MessageHandler handler)
        {
            _connection.On<string>(handler.Type, ( message) =>
            {
                handler.Handle(message);
            });
        }

        public Task SendMessageToHub(string method, object message)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                return _connection.InvokeAsync(method, message);
            }

            return Task.CompletedTask;
        }

        public static string ServerBaseUrl
        {
            get
            {
                var agiledtServer = Config.Instance["agiledt:server"];
                agiledtServer = agiledtServer.TrimEnd('/');

                return agiledtServer;
            }
        }
    }
}

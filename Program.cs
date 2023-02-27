using System;
using System.Buffers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SuperSocket.ClientEngine;
using System.Net.Sockets;

namespace testSocketClient
{
    public partial class Program
    {
        static async Task Main(string[] args)
        {

            var host = Host.CreateDefaultBuilder()
              .ConfigureServices((_, services) => {
                  services.AddSingleton((_) => new EasyClient());
              })
              .ConfigureWebHostDefaults(webBuilder => {
                  webBuilder.Configure(app => {
                      app.Run(async context => {
                          if (context.Request.Method == "POST")
                          {
                              var input = await JsonSerializer.DeserializeAsync<InputModel>(context.Request.Body);
                              if (input?.Message != null)
                              {
                                  var myPackage = new MyPackage(input.Message);
                                  var client = context.RequestServices.GetService<EasyClient>();

                                  if (client != null)
                                  {
                                      var sendData = myPackage.ToByteFormat();
                                      await client.Socket.SendAsync(sendData, SocketFlags.None);
                                      await context.Response.WriteAsync("Message sent to server.");
                                  }
                              }
                          }
                      });
                  });
              })
              .Build();

            await host.StartAsync();

            var client = host.Services.GetRequiredService<EasyClient>();

            client.Initialize(new MyPackageFilter(3), async (request) => {
                var newSendPacket = new MyPackage((PACKET_TYPE)request.OpType, "type complte");
                var sendData = newSendPacket.ToByteFormat();
                await client.Socket.SendAsync(sendData, SocketFlags.None);
            });

            string address = "127.0.0.1";
            IPAddress serverIP = IPAddress.Parse(address);

            bool connected = await client.ConnectAsync(new IPEndPoint(serverIP, 6000));
            if (connected)
            {
                Console.WriteLine("Connected to server.");
            }
            else
            {
                Console.WriteLine("Failed to connect to server.");
                return;
            }

            while (true)
            {
                // Do nothing, just wait for incoming packets from the server
                await Task.Delay(1000);
            }
        }

        class InputModel
        {
            public string Message { get; set; }

            public InputModel()
            {

                Message = "";
            }
        }
    }
}
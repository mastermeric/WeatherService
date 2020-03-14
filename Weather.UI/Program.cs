using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Weather.Entities;
using Microsoft.AspNetCore.SignalR.Client;
using Weather.UI;

namespace BigDataTeknoloji.UI
{
    class Program
    {
        static HubConnection myHubConnection;
        static WeatherOperations operations;

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Monitoring App started. OK.");
                Task.Delay(1000);

                //Stage-1:  Get existing records -----------------------------------------------
                operations = new WeatherOperations();
                Task ExistingRecords = Task.Factory.StartNew(() => operations.GetExistingRecords());
                Task.Delay(1000);
                //------------------------------------------------------------------------------


                //Stage-2: SignalR bildrimleri bekle -------------------------------------------
                myHubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:50217/NotificationHub")
                .Build();
                myHubConnection.Closed += MyHubConnection_Closed;
                myHubConnection.On<string>("ReceiveMessageFromServerHub", DataRecieved);
                myHubConnection.StartAsync();
                //------------------------------------------------------------------------------

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Main ERROR !");
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task MyHubConnection_Closed(Exception arg)
        {
            Console.WriteLine("Server Conenction is Closed !");
            await Task.Delay(1000);
            await myHubConnection.StartAsync();
        }

        private static void DataRecieved(string obj)
        {
            try
            {
                Console.WriteLine(obj);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR at HubConnection-DataRecieved:" + ex.Message);
            }
        }

      

    }
}

using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace SbRealTimePush
{
    class Program
    {
        static void Main(string[] args)
        {
            Random r = new Random(25);
            Random rhum = new Random(40);

            while (true)
            {
                DeviceClient deviceClient1 = DeviceClient.CreateFromConnectionString("HostName=XXXXXXXXXXX.azure-devices.net;DeviceId=XXXXX;SharedAccessKey=XXXXXXXXXXXXXXXXXXXXXX==", TransportType.Http1);
                
              
                double rDouble = r.Next(25,50); //for doubles
                double rDoubleHum = rhum.Next(40,100); //for doubles

                var telemetryDataPoint = new
                {
                    DeviceId = 1,
                    temperature = rDouble,
                    unitOfTemp = "C",
                    humidity = rDoubleHum,
                    unitOfHumidity = "%",
                    TimeFlag = Convert.ToString(DateTime.Now)
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                Console.WriteLine(messageString);
                var message = new Message(System.Text.Encoding.ASCII.GetBytes(messageString));
                deviceClient1.SendEventAsync(message);
                Thread.Sleep(1000);
            }
        }
    }
}

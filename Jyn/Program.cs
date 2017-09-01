using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Jint;
using System.Net.Sockets;
using System.Net;
using System.Globalization;

namespace Jyn
{

    public class Program
    {
        public static void WakeOnLan(string mac2)
        {
            //Log.Information("Waking on MAC: {mac}", BitConverter.ToString(mac));

            byte[] mac = new byte[6] { 0x00, 0x11, 0x32, 0x58, 0x70, 0x75 };

            int counter = 0;

            byte[] bytes = new byte[6 * 17];
            for (var i = 0; i < 6; i++)
            {
                bytes[counter++] = 0xFF;
            }

            //16x MAC
            for (var i = 0; i < 16; i++)
            {
                mac.CopyTo(bytes, 6 + i * 6);
            }

            try
            {
                using (var client = new UdpClient())
                {
                    client.EnableBroadcast = true;
                    var test = client.SendAsync(bytes, bytes.Length, new IPEndPoint(new IPAddress(0xffffffff), 0)).Result;
                }
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "Exception during wake on lan request.");
                throw;
            }
        }

        public static void Main(string[] args)
        {

            var engine = new Engine()
                .SetValue("log", new Action<object>(Console.WriteLine))
                .SetValue("wakeOnLan", new Action<string>(WakeOnLan));

            engine.Execute(@"
      function hello() { 
        log('Hello World');
      };

    wakeOnLan('00-11-32-58-70-75');
      
      hello();
    ");

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}

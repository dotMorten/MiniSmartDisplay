using Iot.Device.Spi;
using System.Device.Gpio;
using System.Device.Spi;
using EPaper;
using SkiaSharp;
using System.Device.I2c;
using dotMorten.HomeAssistent;
using System.Runtime.CompilerServices;

namespace EPaperApp
{
	  internal static class Configuration
    {
        public const string UnifiUserName = "StatusDisplay";
        public const string UnifiPassword = "DisplayStatus1";
        public const string UnifiHost = "192.168.1.1";
        public const string UnifiCameraSnap = "http://192.168.1.157/snap.jpeg";

        public const string HomeAssistentUrl = "http://192.168.1.138:8123";
        public const string HomeAssistentAccessToken = "HOMEASSISTANT_ACCESS_TOKEN_HERE";
    }


    internal class Program
    {
		internal static HAClient HomeClient { get; } = new HAClient(Configuration.HomeAssistentAccessToken, Configuration.HomeAssistentUrl);


        static async Task Main(string[] args)
        {
            System.AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException; ;

            using SmartDisplay display = new SmartDisplay();
            try
            {
                display.Initialize();
                await display.Run();
            }
            catch(System.Exception ex)
            {
                Console.WriteLine("ERROR\n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            
        }
    }
}

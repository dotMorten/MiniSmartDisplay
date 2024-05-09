using Iot.Device.Spi;
using System.Device.Spi;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.I2c;

namespace EPaper
{
    public class TouchController : IDisposable
    {
        const int GT1151_ADDRESS = (0x14);
        const int GT1151_COMMAND = (0x8040);
        const int GT1151_CONFIG = (0x8050);
        const int GT9XX_PRODUCT_ID = (0x8140);
        const int GT1151_READ_STATUS = (0x814E);
        const int GT1151_POINT1_REG = (0x814F);
        const int GT1151_CHECK_SUM = (0X813C);

        private I2cDevice i2CDevice;
        
        private void ReadID(I2cDevice i2CDevice)
        {
            Span<byte> register = stackalloc byte[2] { 0x81, 0x40 };
            Span<byte> data = stackalloc byte[4];
            i2CDevice.WriteRead(register, data);
            System.Console.WriteLine($"ID: {data[0]:X2} {data[1]:X2} {data[2]:X2} {data[3]:X2}");
        }
        
        public TouchController(GpioController gpioController, int pinId = 27)
        {
            //i2CDevice = I2cDevice.Create(new I2cConnectionSettings(1, 0x48));
            //ReadID(i2CDevice);
            int _interruptPin = pinId; // Pin 13 on the Raspberry Pi
            gpioController.OpenPin(_interruptPin, PinMode.InputPullUp);
            gpioController.RegisterCallbackForPinValueChangedEvent(_interruptPin, PinEventTypes.Rising | PinEventTypes.Falling, OnInterrupt);
        }

        private void OnInterrupt(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {
            if (pinValueChangedEventArgs.ChangeType != PinEventTypes.Falling)
                return;
            /*
            var buf = i2CDevice.ReadByte();
            var bf = new byte[1];
            Span<byte> register = stackalloc byte[2] { 0x81, 0x4E }; // GT1151_READ_STATUS
            Span<byte> data = stackalloc byte[1];
            i2CDevice.WriteRead(register, data);
            if (buf == 0xcc)
            {
                Console.WriteLine("gesture mode exiting");
            }
            register = stackalloc byte[2] { 0x81, 0x4C };
            Span<byte> data2 = stackalloc byte[1];
            i2CDevice.WriteRead(register, data2);

            Console.WriteLine($"Interrupt! #{pinValueChangedEventArgs.PinNumber} = {pinValueChangedEventArgs.ChangeType} - {data[0]} - {data2[0]}");
            */
            if ((DateTime.Now - lastEvent).TotalSeconds < 3)
                return;
            lastEvent = DateTime.Now;
            Touched?.Invoke(this, EventArgs.Empty);
        }
        DateTime lastEvent = DateTime.MinValue;
        public event EventHandler Touched;

        public void Dispose()
        {
            i2CDevice?.Dispose();
        }
    }
}

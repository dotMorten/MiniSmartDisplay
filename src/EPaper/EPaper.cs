using System.Buffers;
using System.Device.Gpio;
using System.Device.Spi;
using System.Diagnostics;
using Iot.Device.Spi;

namespace EPaper
{
    public class EPaper : IDisposable
    {
        public const int DefaultSpiClockFrequency = 20_000_000;
        public const SpiMode DefaultSpiMode = SpiMode.Mode0;
        private readonly int _dcPinId;
        private readonly int _resetPinId;
        private readonly int _busyPinId;
        private SpiDevice _spiDevice;
        private GpioController _gpioDevice;
        public short Height { get; }
        public short Width { get; }
        private readonly int BufferSize;
        bool _shouldDispose;

        public static EPaper Create2in9HatDisplay(GpioController? controller = null)
        {
            SoftwareSpi device = new SoftwareSpi(11, -1, 10, 8, new SpiConnectionSettings(0)
            {
                ClockFrequency = 20_000_000,
                Mode = SpiMode.Mode0,
            });
            return new EPaper(device, dataCommandPin: 25, resetPin: 17, busyPin: 24, width: 128, height: 296, gpioController: controller);
        }

        public EPaper(SpiDevice spiDevice, int dataCommandPin, int resetPin, int busyPin, short width, short height, GpioController? gpioController = null, bool shouldDispose = true)
        {
            Width = width;
            Height = height;
            _spiDevice = spiDevice;
            _dcPinId = dataCommandPin;
            _resetPinId = resetPin;
            _gpioDevice = gpioController ?? new GpioController();
            _shouldDispose = shouldDispose || gpioController is null;
            _busyPinId = busyPin;
            _gpioDevice.OpenPin(_dcPinId, PinMode.Output);
            _gpioDevice.OpenPin(_resetPinId, PinMode.Output);
            _gpioDevice.OpenPin(_busyPinId, PinMode.Input);
            BufferSize = (int)(Width / 8 + (Width % 8 == 0 ? 0 : 1)) * Height;
            Initialize();
        }

        /// <summary>
        /// Initialize the e-Paper register
        /// </summary>
        private void Initialize()
        {
            Reset();
            Thread.Sleep(100);
            WaitUntilIdle();
            SendCommand(0x12); // soft reset
            WaitUntilIdle();
            SendCommand(Command.DRIVER_OUTPUT_CONTROL);
            SendData(0x27);
            SendData(0x01);
            SendData(0x00);
            SendCommand(Command.DATA_ENTRY_MODE_SETTING);
            SendData(0x03);
            SetWindows(0, 0, (short)(Width - 1), (short)(Height - 1));

            SendCommand(Command.DISPLAY_UPDATE_CONTROL_1);
            SendData(0x00);
            SendData(0x80);

            SetCursor(0, 0);
            WaitUntilIdle();
        }

        private void TurnOnDisplay()
        {
            SendCommand(Command.DISPLAY_UPDATE_CONTROL_2); //Display Update Control
            SendData(0xF7);
            SendCommand(0x20); //Activate Display Update Sequence
            WaitUntilIdle();
        }

        private void TurnOnDisplay_Partial(bool wait = false)
        {
            SendCommand(0x22); //Display Update Control
            SendData(0x0F);
            SendCommand(0x20); //Activate Display Update Sequence
            if (wait)
                WaitUntilIdle();
        }

        /// <summary>
        /// Sends the image buffer in RAM to e-Paper and displays
        /// </summary>
        /// <param name="image"></param>
        /// <param name="partial"></param>
        /// <exception cref="ArgumentException"></exception>
        public void DisplayImage(ReadOnlySpan<byte> image, bool partial = false)
        {
            if (image.Length != BufferSize)
                throw new ArgumentException($"Image size {image.Length} does not match the display size buffer {BufferSize}", nameof(image));
           
            if (partial)
            {
                DisplayImage_Partial(image, true);
            }
            else
            {
                SendCommand(Command.WRITE_RAM);
                SendData(image);
                SendCommand(0x26);
                SendData(image);
                TurnOnDisplay();
            }
        }

        public void Clear(bool white = true)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(BufferSize); 
            for (int i = 0; i < BufferSize; i++)
            {
                buffer[i] = white ? (byte)0xFF : (byte)0x00;
            }
            DisplayImage(new ReadOnlySpan<byte>(buffer, 0, BufferSize));
            ArrayPool<byte>.Shared.Return(buffer);
            //EPD_2IN9_V2_TurnOnDisplay();
        }
      
        private void DisplayImage_Partial(ReadOnlySpan<byte> Image, bool wait = false)
        {
            // UWORD i;

            if (wait)
            {
                _gpioDevice.Write(_resetPinId, PinValue.Low);
                Thread.Sleep(200);
                _gpioDevice.Write(_resetPinId, PinValue.High);
                
                //Reset
                // DEV_Digital_Write(EPD_RST_PIN, 0);
                // DEV_Delay_ms(5);
                // DEV_Digital_Write(EPD_RST_PIN, 1);
                // DEV_Delay_ms(10);
            }

            SendLUT(wait);
            SendCommand(0x37);
            SendData(0x00);
            SendData(0x00);
            SendData(0x00);
            SendData(0x00);
            SendData(0x00);
            SendData(0x40);
            SendData(0x00);
            SendData(0x00);
            SendData(0x00);
            SendData(0x00);

            SendCommand(0x3C); //BorderWavefrom
            SendData(0x80);

            SendCommand(0x22);
            SendData(0xC0);
            SendCommand(0x20);
            WaitUntilIdle();

            SetWindows(0, 0, (short)(Width - 1), (short)(Height - 1));
            SetCursor(0, 0);

            SendCommand(0x24);   //Write Black and White image to RAM
            SendData(Image);
            TurnOnDisplay_Partial(wait);
        }

        private void SendLUT(bool wait)
        {
            SendCommand(0x32);
            if (!wait)
                SendData(_WF_PARTIAL_2IN9);
            else
                SendData(_WF_PARTIAL_2IN9_Wait);
            WaitUntilIdle();
        }

        /// <summary>
        /// Set display to sleep mode
        /// </summary>
        public void Sleep()
        {
            SendCommand(Command.DEEP_SLEEP_MODE);
            SendData(0x01);
            Thread.Sleep(100);
        }

        /// <summary>
        /// Wait until the display is idle (not busy)
        /// </summary>
        protected virtual void WaitUntilIdle()
        {
            int count = 0;
            Debug.Write("Waiting for idle...");
            while (_gpioDevice.Read(_busyPinId) != PinValue.Low)
            {
                Thread.Sleep(10);
                count++;
            }
            Debug.WriteLine(count*10);
        }

        private void SetWindows(short xstart, short ystart, short xend, short yend)
        {
            SendCommand(Command.SET_RAM_X_ADDRESS_START_END_POSITION);
            // x point must be the multiple of 8 or the last 3 bits will be ignored 
            SendData((xstart >> 3) & 0xFF);
            SendData((xend >> 3) & 0xFF);

            SendCommand(Command.SET_RAM_Y_ADDRESS_START_END_POSITION);
            SendData(ystart & 0xFF);
            SendData((ystart >> 8) & 0xFF);
            SendData(yend & 0xFF);
            SendData((yend >> 8) & 0xFF);
        }

        private void SetCursor(int Xstart, int Ystart)
        {
            SendCommand(0x4E); // SET_RAM_X_ADDRESS_COUNTER
            SendData(Xstart & 0xFF);

            SendCommand(0x4F); // SET_RAM_Y_ADDRESS_COUNTER
            SendData(Ystart & 0xFF);
            SendData((Ystart >> 8) & 0xFF);
        }

        private protected void SendCommand(Command command) => SendCommand((byte)command);

        protected void SendCommand(byte command)
        {
            _gpioDevice.Write(_dcPinId, CommandState);
            _spiDevice.Write(new byte[] { command });
        }

        protected void SendData(int data)
        {
            if (data > 255 || data < 0)
                throw new ArgumentOutOfRangeException(nameof(data));
            SendData((byte)data);
        }

        private PinValue DataState = PinValue.High;
        private PinValue CommandState = PinValue.Low;

        protected void SendData(byte data)
        {
            _gpioDevice.Write(_dcPinId, DataState);
            _spiDevice.Write(new ReadOnlySpan<byte>(ref data));
        }

        protected void SendData(ReadOnlySpan<byte> data)
        {
            _gpioDevice.Write(_dcPinId, DataState);
            _spiDevice.Write(data);
        }

        byte[] _WF_PARTIAL_2IN9 =
        {
        0x0,0x40,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,
        0x80,0x80,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,
        0x40,0x40,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,
        0x0,0x80,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,
        0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,
        0x0A,0x0,0x0,0x0,0x0,0x0,0x0,
        0x1,0x0,0x0,0x0,0x0,0x0,0x0,
        0x1,0x0,0x0,0x0,0x0,0x0,0x0,
        0x0,0x0,0x0,0x0,0x0,0x0,0x0,
        0x0,0x0,0x0,0x0,0x0,0x0,0x0,
        0x0,0x0,0x0,0x0,0x0,0x0,0x0,
        0x0,0x0,0x0,0x0,0x0,0x0,0x0,
        0x0,0x0,0x0,0x0,0x0,0x0,0x0,
        0x0,0x0,0x0,0x0,0x0,0x0,0x0,
        0x0,0x0,0x0,0x0,0x0,0x0,0x0,
        0x0,0x0,0x0,0x0,0x0,0x0,0x0,
        0x0,0x0,0x0,0x0,0x0,0x0,0x0,
        0x22,0x22,0x22,0x22,0x22,0x22,0x0,0x0,0x0,
        0x22,0x17,0x41,0xB0,0x32,0x36,
        };

        byte[] _WF_PARTIAL_2IN9_Wait =
        {
            0x0,0x40,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,
            0x80,0x80,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,
            0x40,0x40,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,
            0x0,0x80,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,
            0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,
            0x0A,0x0,0x0,0x0,0x0,0x0,0x2,
            0x1,0x0,0x0,0x0,0x0,0x0,0x0,
            0x1,0x0,0x0,0x0,0x0,0x0,0x0,
            0x0,0x0,0x0,0x0,0x0,0x0,0x0,
            0x0,0x0,0x0,0x0,0x0,0x0,0x0,
            0x0,0x0,0x0,0x0,0x0,0x0,0x0,
            0x0,0x0,0x0,0x0,0x0,0x0,0x0,
            0x0,0x0,0x0,0x0,0x0,0x0,0x0,
            0x0,0x0,0x0,0x0,0x0,0x0,0x0,
            0x0,0x0,0x0,0x0,0x0,0x0,0x0,
            0x0,0x0,0x0,0x0,0x0,0x0,0x0,
            0x0,0x0,0x0,0x0,0x0,0x0,0x0,
            0x22,0x22,0x22,0x22,0x22,0x22,0x0,0x0,0x0,
            0x22,0x17,0x41,0xB0,0x32,0x36,
            };

        protected virtual void Reset()
        {
            _gpioDevice.Write(_resetPinId, PinValue.High);
            Thread.Sleep(200);
            _gpioDevice.Write(_resetPinId, PinValue.Low);
            Thread.Sleep(10);
            _gpioDevice.Write(_resetPinId, PinValue.High);
            Thread.Sleep(200);
        }

        public void Dispose()
        {
            try
            {
                Sleep();
            }
            catch { }
            if (_shouldDispose)
            {
                _spiDevice?.Dispose();
                _gpioDevice?.Dispose();
            }
        }
    }
}
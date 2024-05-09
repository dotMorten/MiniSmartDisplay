using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPaper
{
    internal enum Command : byte
    {
        /// <summary>
        /// Driver output control
        /// </summary>
        DRIVER_OUTPUT_CONTROL = 0x01,
        /// <summary>
        /// Booster soft start control
        /// </summary>
        BOOSTER_SOFT_START_CONTROL = 0x0C,
        /// <summary>
        /// Gate scan start position
        /// </summary>
        GATE_SCAN_START_POSITION = 0x0F,
        /// <summary>
        /// Deep sleep mode
        /// </summary>
        DEEP_SLEEP_MODE = 0x10,
        /// <summary>
        /// Data entry mode setting
        /// </summary>
        DATA_ENTRY_MODE_SETTING = 0x11,
        /// <summary>
        /// Software reset
        /// </summary>
        SW_RESET = 0x12,
        /// <summary>
        /// Temperature sensor control
        /// </summary>
        TEMPERATURE_SENSOR_CONTROL = 0x1A,
        /// <summary>
        /// Master activation
        /// </summary>
        MASTER_ACTIVATION = 0x20,
        /// <summary>
        /// Display update control 1
        /// </summary>
        DISPLAY_UPDATE_CONTROL_1 = 0x21,
        /// <summary>
        /// Display update control 2
        /// </summary>
        DISPLAY_UPDATE_CONTROL_2 = 0x22,
        /// <summary>
        /// Write ram
        /// </summary>
        WRITE_RAM = 0x24,
        /// <summary>
        /// Write VCOM register
        /// </summary>
        WRITE_VCOM_REGISTER = 0x2C,
        /// <summary>
        /// Write look up table register
        /// </summary>
        WRITE_LUT_REGISTER = 0x32,
        /// <summary>
        /// Set dummy line period
        /// </summary>
        SET_DUMMY_LINE_PERIOD = 0x3A,
        /// <summary>
        /// Set gate time
        /// </summary>
        SET_GATE_TIME = 0x3B,
        /// <summary>
        /// Border waveform control
        /// </summary>
        BORDER_WAVEFORM_CONTROL = 0x3C,
        /// <summary>
        /// Set ram address x start and end position
        /// </summary>
        SET_RAM_X_ADDRESS_START_END_POSITION = 0x44,
        /// <summary>
        /// Set ram address y start and end position
        /// </summary>
        SET_RAM_Y_ADDRESS_START_END_POSITION = 0x45,
        /// <summary>
        /// Set ram x address counter
        /// </summary>
        SET_RAM_X_ADDRESS_COUNTER = 0x4E,
        /// <summary>
        /// Set ram y address counter
        /// </summary>
        SET_RAM_Y_ADDRESS_COUNTER = 0x4F,
        /// <summary>
        /// Terminate frame read and write
        /// </summary>
        TERMINATE_FRAME_READ_WRITE = 0xFF,
    }
}

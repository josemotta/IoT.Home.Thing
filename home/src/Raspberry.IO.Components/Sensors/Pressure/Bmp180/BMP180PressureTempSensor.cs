/*
Copyright (c) 2014 Adafruit Industries
Original Author: Tony DiCola
https://github.com/adafruit/Adafruit_Python_BMP/blob/master/Adafruit_BMP/BMP085.py
Ported to C#/Raspberry.IO by Ionut Nechita

# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the "Software"), to deal
# in the Software without restriction, including without limitation the rights
# to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:
#
# The above copyright notice and this permission notice shall be included in
# all copies or substantial portions of the Software.
#
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
# THE SOFTWARE.
 * 
*/

/*
Extracted from:
http://djonexx.netimage.ro/2015/04/10/using-bosch-bmp180-sensor-with-raspberry-pi-and-monoc/
*/

using Raspberry.IO.InterIntegratedCircuit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raspberry.IO.Components.Sensors.Pressure.Bmp180
{
    /// <summary>
    /// Represents a BMP180 Pressure Temp Sensor device
    /// </summary>
    public class BMP180PressureTempSensor
    {
        I2cDeviceConnection _device;
        BMP085Mode _mode = BMP085Mode.Standard;

        #region Device Registers
        // Registers
        const byte BMP085_CAL_AC1 = 0xAA;  // R   Calibration data (16 bits)
        const byte BMP085_CAL_AC2 = 0xAC;  // R   Calibration data (16 bits)
        const byte BMP085_CAL_AC3 = 0xAE;  // R   Calibration data (16 bits)
        const byte BMP085_CAL_AC4 = 0xB0;  // R   Calibration data (16 bits)
        const byte BMP085_CAL_AC5 = 0xB2;  // R   Calibration data (16 bits)
        const byte BMP085_CAL_AC6 = 0xB4;  // R   Calibration data (16 bits)
        const byte BMP085_CAL_B1 = 0xB6;  // R   Calibration data (16 bits)
        const byte BMP085_CAL_B2 = 0xB8;  // R   Calibration data (16 bits)
        const byte BMP085_CAL_MB = 0xBA;  // R   Calibration data (16 bits)
        const byte BMP085_CAL_MC = 0xBC;  // R   Calibration data (16 bits)
        const byte BMP085_CAL_MD = 0xBE;  // R   Calibration data (16 bits)
        const byte BMP085_CONTROL = 0xF4;
        const byte BMP085_TEMPDATA = 0xF6;
        const byte BMP085_PRESSUREDATA = 0xF6;

        // Commands
        const byte BMP085_READTEMPCMD = 0x2E;
        const byte BMP085_READPRESSURECMD = 0x34;
        const byte BMP085_ECHO = 0xD0;
        // (reading from this register should yield 0x55)
        const byte BMP085_RESET = 0xE0;  // write 0xB6
        #endregion


        #region Sensor Calibration
        // calibration variables
        short cal_AC1 = 408;
        short cal_AC2 = -72;
        short cal_AC3 = -14383;
        ushort cal_AC4 = 32741;
        ushort cal_AC5 = 32757;
        ushort cal_AC6 = 23153;
        short cal_B1 = 6190;
        short cal_B2 = 4;
        short cal_MB = -32767;
        short cal_MC = -8711;
        short cal_MD = 2868;

        void LoadCalibration()
        {
            cal_AC1 = ReadS16(BMP085_CAL_AC1); //   # INT16
            cal_AC2 = ReadS16(BMP085_CAL_AC2); //   # INT16
            cal_AC3 = ReadS16(BMP085_CAL_AC3); //  # INT16
            cal_AC4 = ReadU16(BMP085_CAL_AC4); //  # UINT16
            cal_AC5 = ReadU16(BMP085_CAL_AC5); //  # UINT16
            cal_AC6 = ReadU16(BMP085_CAL_AC6); // # UINT16
            cal_B1 = ReadS16(BMP085_CAL_B1);  //   # INT16
            cal_B2 = ReadS16(BMP085_CAL_B2);  //   # INT16
            cal_MB = ReadS16(BMP085_CAL_MB);  //   # INT16
            cal_MC = ReadS16(BMP085_CAL_MC);  //   # INT16
            cal_MD = ReadS16(BMP085_CAL_MD);  //   # INT16

            if (false)
            {
                Console.WriteLine("Dumping calibration data");
                DumpParameter("AC1", cal_AC1);
                DumpParameter("AC2", cal_AC2);
                DumpParameter("AC3", cal_AC3);
                DumpParameter("AC4", cal_AC4);
                DumpParameter("AC5", cal_AC5);
                DumpParameter("AC6", cal_AC6);
                DumpParameter("B1", cal_B1);
                DumpParameter("B2", cal_B2);
                DumpParameter("MB", cal_MB);
                DumpParameter("MC", cal_MC);
                DumpParameter("MD", cal_MD);
            }
        }

        #endregion


        #region Utility Methods

        private void DumpParameter(string p, int par)
        {
            Console.WriteLine("{0}: {1}", p, par);
        }


        void WriteByte(byte register, byte value)
        {
            _device.Write(register, value);
        }

        byte ReadByte(byte register)
        {
            _device.WriteByte(register);
            return _device.ReadByte();
        }

        ushort ReadU16(byte register)
        {
            _device.WriteByte(register);
            byte msb = _device.ReadByte();
            _device.WriteByte((byte)(register + 1));
            byte lsb = _device.ReadByte();
            return (ushort)((msb << 8) + lsb);
        }

        short ReadS16(byte register)
        {
            _device.WriteByte(register);
            byte msb = _device.ReadByte();
            _device.WriteByte((byte)(register + 1));
            byte lsb = _device.ReadByte();
            return (short)((msb << 8) + lsb);
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="driver">The I2cDriver instance</param>
        /// <param name="deviceAddress">The device address on the i2c bus (usually 0x77)</param>
        /// <param name="mode">The sensor resolution mode</param>
        public BMP180PressureTempSensor(I2cDriver driver, int deviceAddress, BMP085Mode mode = BMP085Mode.UltraLowPower)
        {
            _device = driver.Connect(deviceAddress);
            _mode = mode;
            TestConnection();
            LoadCalibration();
        }

        /// <summary>
        /// Tests the connection to the sensor. Writing to the echo register should always return 0x55.
        /// </summary>
        void TestConnection()
        {
            WriteByte(BMP085_ECHO, 0x00);
            byte echo = ReadByte(BMP085_ECHO);
            //Console.WriteLine(String.Format("Echo: {0:x}", echo));
            if (echo != 0x55)
                throw new ApplicationException("Unexpected value from echo command");
        }


        ushort ReadRawTemp()
        {
            WriteByte(BMP085_CONTROL, BMP085_READTEMPCMD);
            WaitForDevice();
            return ReadU16(BMP085_TEMPDATA);
        }

        long ReadRawPressure()
        {
            WriteByte(BMP085_CONTROL, (byte)(BMP085_READPRESSURECMD | ((byte)_mode << 6)));
            WaitForDevice();

            byte msb = ReadByte(BMP085_PRESSUREDATA);
            byte lsb = ReadByte(BMP085_PRESSUREDATA + 1);
            byte xlsb = ReadByte(BMP085_PRESSUREDATA + 2);

            long raw = ((msb << 16) + (lsb << 8) + xlsb) >> (8 - (byte)_mode);
            return raw;
        }

        /// <summary>
        /// Waits for the device to execute the command. This depends on the desired resolution mode
        /// </summary>
        void WaitForDevice()
        {
            switch (_mode)
            {
                case BMP085Mode.UltraLowPower:
                    Raspberry.Timers.Timer.Sleep(TimeSpan.FromSeconds(0.005));
                    break;
                case BMP085Mode.HighRes:
                    Raspberry.Timers.Timer.Sleep(TimeSpan.FromSeconds(0.014));
                    break;
                case BMP085Mode.UltraHighRes:
                    Raspberry.Timers.Timer.Sleep(TimeSpan.FromSeconds(0.026));
                    break;
                default:
                    Raspberry.Timers.Timer.Sleep(TimeSpan.FromSeconds(0.008));
                    break;
            }
        }


        /// <summary>
        /// Gets the sea-level pressure in Pa
        /// </summary>
        /// <returns></returns>
        public double ReadSealevelPressure()
        {
            double altitudeM = 0.0f;
            double pressure = this.ReadPressure();
            double ret = pressure / Math.Pow(1.0 - altitudeM / 44330.0, 5.255);
            return ret;
        }

        /// <summary>
        /// Gets the altitude in meters.
        /// </summary>
        /// <returns></returns>
        public double ReadAltitude()
        {
            double sealevelPA = 101325.0f;
            double pressure = this.ReadPressure();
            double altitude = 44330 * (1.0 - Math.Pow(pressure / sealevelPA, (1.0 / 5.255)));
            return altitude;
        }

        /// <summary>
        /// Reads the current pressure
        /// </summary>
        /// <returns>Pressure in Pa</returns>
        public long ReadPressure()
        {
            long ut = this.ReadRawTemp();
            long up = this.ReadRawPressure();
            //ut = 27898;
            //up = 23843;

            long x1 = ((ut - cal_AC6) * cal_AC5) >> 15;
            long x2 = (cal_MC << 11) / (x1 + cal_MD);
            long b5 = x1 + x2;
            //Console.WriteLine("B5 = " + b5);

            long b6 = b5 - 4000;

            //Console.WriteLine("B6 = " + b6);
            x1 = (cal_B2 * ((b6 * b6) >> 12)) >> 11;
            x2 = (cal_AC2 * b6) >> 11;
            var x3 = x1 + x2;

            long b3 = (((cal_AC1 * 4 + x3) << (byte)_mode) + 2) >> 2;
            //Console.WriteLine("B3 = " + b3);

            x1 = (cal_AC3 * b6) >> 13;
            x2 = (cal_B1 * ((b6 * b6) >> 12)) >> 16;
            x3 = ((x1 + x2) + 2) >> 2;

            ulong b4 = (cal_AC4 * (ulong)(x3 + 32768)) >> 15;
            //Console.WriteLine("B4 = " + b4);

            ulong b7 = (ulong)((up - b3) * (50000 >> (byte)_mode));
            //Console.WriteLine("B7 = " + b7);

            long p = 0;
            if (b7 < 0x80000000)
                p = (long)((b7 * 2) / b4);
            else
                p = (long)((b7 / b4) * 2);

            //Console.WriteLine("P = " + p);

            x1 = (p >> 8) * (p >> 8);
            x1 = (x1 * 3038) >> 16;
            x2 = (-7357 * p) >> 16;
            p = p + ((x1 + x2 + 3791) >> 4);

            //Console.WriteLine("Pfin = " + p);
            return p;
        }

        /// <summary>
        /// Reads the compensated temperature in degrees celsius.
        /// </summary>
        /// <returns>The temperature, in &deg;C</returns>
        public double ReadTemperature()
        {
            ushort UT = ReadRawTemp();
            //DumpParameter("UT", UT);
            // Datasheet value for debugging:
            //UT = 27898;
            // Calculations below are taken straight from section 3.5 of the datasheet.
            long X1 = ((UT - cal_AC6) * cal_AC5) >> 15;
            long X2 = (cal_MC << 11) / (X1 + cal_MD);
            long B5 = X1 + X2;
            double temp = ((B5 + 8) >> 4) / 10;
            return temp;
        }


    }

    /// <summary>
    /// BMP085 Mode
    /// </summary>
    public enum BMP085Mode : byte
    {
        UltraLowPower = 0,
        Standard = 1,
        HighRes = 2,
        UltraHighRes = 3
    }
}

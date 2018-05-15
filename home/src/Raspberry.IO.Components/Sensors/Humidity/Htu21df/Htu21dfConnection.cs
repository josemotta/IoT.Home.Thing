using System;
using System.Threading;
using Raspberry.IO.Components.Sensors.Temperature.Dht;
using Raspberry.IO.InterIntegratedCircuit;

namespace Raspberry.IO.Components.Sensors.Humidity.Htu21df
{
    /// <summary>
    /// Connection to the HTU21D-F Adafruit humidity and temperature breakout board.
    /// </summary>
    /// <remarks>See <see cref="https://www.adafruit.com/datasheets/1899_HTU21D.pdf"/> for more information.</remarks>/// 
    public class Htu21dfConnection
    {
        #region Helpers

        public enum I2cDefs : byte
        {
            HTU21DF_I2CADDR = 0x40,
            HTU21DF_READTEMP = 0xE3,
            HTU21DF_READHUM = 0xE5,
            HTU21DF_READTEMP_NOHOLDMASTER = 0xF3,
            HTU21DF_READHUM_NOHOLDMASTER = 0xF5,
            HTU21DF_WRITEREG = 0xE6,
            HTU21DF_READREG = 0xE7,
            HTU21DF_RESET = 0xFE,
        }

        public enum I2cReadMode
        {
            /// <summary>
            /// Default. In this mode, the SCK line is blocked during the measurement process. When measurement is complete
            /// the sensor indicates by releasing SCK and the read can continue.
            /// </summary>
            HoldMaster,

            /// <summary>
            /// The sensor does not hold SCK so other I2C comms can take place on the bus. The MCU (R-Pi) has to then
            /// poll for the data. NOT CURRENTLY SUPPORTED.
            /// </summary>
            NoHoldMaster,
        }

        #endregion

        #region Fields

        private readonly I2cDeviceConnection connection;

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="Htu21dfConnection"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public Htu21dfConnection(I2cDeviceConnection connection)
        {
            this.connection = connection;
            ReadMode = I2cReadMode.HoldMaster;
        }

        #endregion

        #region Methods

        public I2cReadMode ReadMode { get; set; }

        /// <summary>
        /// Init's the sensor and checks its status is OK.
        /// </summary>
        public void Begin()
        {
            Reset();

            connection.WriteByte((byte)I2cDefs.HTU21DF_READREG);
            var status = connection.ReadByte();
            if (status != 0x02)
            {
                throw new Exception($"Status following reset should be 0x02. Have {status}");
            }
        }

        /// <summary>
        /// Resets the sensor by power cycling. Takes about 15ms.
        /// </summary>
        private void Reset()
        {
            connection.WriteByte((byte)I2cDefs.HTU21DF_RESET);
            Thread.Sleep(15);
        }

        /// <summary>
        /// Reads the temperature value from the sensor.
        /// </summary>
        /// <returns>Temperature in degrees Centigrade.</returns>
        public double ReadTemperature()
        {
            if (ReadMode == I2cReadMode.NoHoldMaster)
            {
                throw new NotSupportedException("No-Hold-Master read mode not supported.");
            }

            connection.WriteByte((byte)I2cDefs.HTU21DF_READTEMP);

            // Add delay between request and actual read
            Thread.Sleep(50);

            // Read 3 bytes; 2 bytes temp data and one byte checksum.
            var readBytes = connection.Read(3);
            CheckCrc(readBytes);

            // Get data value from bytes 0 and 1.
            var tVal = (readBytes[0] << 8) + readBytes[1];

            // Compute temp using formula from datasheet.
            return ((tVal * 175.72) / 65536) - 46.85;
        }

        /// <summary>
        /// Reads the humidity value from the sensor.
        /// </summary>
        /// <returns>The relative humidity value as a percentage.</returns>
        public double ReadHumidity()
        {
            if (ReadMode == I2cReadMode.NoHoldMaster)
            {
                throw new NotSupportedException("No-Hold-Master read mode not supported.");
            }

            connection.WriteByte((byte)I2cDefs.HTU21DF_READHUM);

            // Add delay between request and actual read
            Thread.Sleep(50);

            // Read 3 bytes; 2 bytes temp data and one byte checksum.
            var readBytes = connection.Read(3);
            CheckCrc(readBytes);

            // Get data value from bytes 0 and 1.
            var hVal = (readBytes[0] << 8) + readBytes[1];

            // Compute temp using formula from datasheet.
            return (((double)hVal * 125) / 65536) - 6;
        }

        /// <summary>
        /// Calculate the CRC checksum. The result should be zero.
        /// Thanks to the IoT-Playground - https://github.com/iot-playground.
        /// </summary>
        public static void CheckCrc(byte[] readData)
        {
            // Test cases from datasheet:
            // sensor value = 0xDC, checkvalue is 0x79. readData = 0x00DC79.
            // message = 0x683A, checkvalue is 0x7C. readData = 0x683A7C
            // message = 0x4E85, checkvalue is 0x6B. readData = 0x45856B

            // Create a 32-bit value with bytes of {0, val-msb, val-lsb, checksum}
            var remainder = (uint)((readData[0] << 16) + (readData[1] << 8) + readData[2]);

            // This is the x^8 + x^5 + x^4 + 1 polynomial (100110001), but shifted up to start at the left-hand
            // side of the 24-bit value. 
            uint divisor = 0x988000;

            for (var i = 0; i < 16; i++)
            {
                if ((remainder & (1 << (23 - i))) != 0)
                {
                    remainder ^= divisor;
                }

                divisor >>= 1;
            }

            if (remainder != 0)
            {
                throw new InvalidChecksumException(0x00, remainder);
            }
        }

        #endregion
    }
}

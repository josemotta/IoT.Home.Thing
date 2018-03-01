namespace Raspberry
{
    /// <summary>
    /// The Raspberry Pi processor.
    /// </summary>
    public enum Processor
    {
        /// <summary>
        /// Processor is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// Processor is a BCM2708.
        /// </summary>
        Bcm2708,

        /// <summary>
        /// Processor is a BCM2709.
        /// </summary>
        Bcm2709,

        /// <summary>
        /// Processor is a BCM2835.
        /// Support for RPi3/BCM2835 on Raspbian 4.9
        /// https://github.com/raspberry-sharp/raspberry-sharp-io/issues/88
        /// </summary>
        Bcm2835
    }
}
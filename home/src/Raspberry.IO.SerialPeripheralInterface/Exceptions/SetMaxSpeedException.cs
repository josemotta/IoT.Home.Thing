using System;
using System.Runtime.Serialization;

namespace Raspberry.IO.SerialPeripheralInterface
{
#pragma warning disable 1591
    [Serializable]
    public class SetMaxSpeedException : Exception {
        public SetMaxSpeedException() {}
        public SetMaxSpeedException(string message) : base(message) {}
        public SetMaxSpeedException(string message, Exception innerException) : base(message, innerException) {}
    }
#pragma warning restore 1591
}
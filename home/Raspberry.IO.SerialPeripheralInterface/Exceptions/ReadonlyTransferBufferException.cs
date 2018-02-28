﻿using System;
using System.Runtime.Serialization;

namespace Raspberry.IO.SerialPeripheralInterface
{
#pragma warning disable 1591
    [Serializable]
    public class ReadOnlyTransferBufferException : Exception
    {
        public ReadOnlyTransferBufferException() {}
        public ReadOnlyTransferBufferException(string message) : base(message) {}
        public ReadOnlyTransferBufferException(string message, Exception innerException) : base(message, innerException) {}
    }
#pragma warning restore 1591
}
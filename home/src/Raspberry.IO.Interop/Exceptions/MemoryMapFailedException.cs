using System;
using System.Runtime.Serialization;

namespace Raspberry.IO.Interop
{
    [Serializable]
    public class MemoryMapFailedException : Exception {
        public MemoryMapFailedException() {}
        public MemoryMapFailedException(string message) : base(message) {}
        public MemoryMapFailedException(string message, Exception innerException) : base(message, innerException) {}
      
    }
}
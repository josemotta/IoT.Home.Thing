using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Helper to save files
    /// </summary>
    public static class HelperFileStream
    {
        /// <summary>
        /// Save stream to file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        public static void SaveToFile(string path, Object value)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                using (StreamWriter str = new StreamWriter(fs))
                {
                    str.Write(value);
                    str.Flush();
                }
            }
        }
    }
}

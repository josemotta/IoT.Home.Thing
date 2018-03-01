using Microsoft.Extensions.Configuration;
using Raspberry.IO.GeneralPurpose.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Helpers to config
    /// </summary>
    public static class HelperConfiguration
    {
        /// <summary>
        /// Get Config From JSON
        /// </summary>
        /// <param name="file"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static GpioConnectionConfigurationSection GetConfigurationFromJSON(string file, string section)
        {
            string runFolder = AppContext.BaseDirectory;
            string configFile = Path.Combine(runFolder, file);
            if (!File.Exists(configFile))
                throw new FileLoadException($"Can't open config file: {configFile}");
            IConfigurationBuilder conf = new ConfigurationBuilder().AddJsonFile(configFile);
            return conf.Build().GetSection(section) as GpioConnectionConfigurationSection;
        }

    }
}

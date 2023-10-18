using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventConsumer
{
    public static class AppConfiguration
    {
        public static string signalRHubUrl { get; set; }
        public static void Load(IConfiguration configuration)
        {
            signalRHubUrl = configuration[$"{ConfigurationItem.SignalRConfig}:SignalRUrl"];
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCSHMD
{
    public class Settings
    {
        public string IP;
        public int Port;
        public string TelemetryOuptut;

        public Settings()
        {
            IP = "127.0.0.1";
            Port = 8532;
            TelemetryOuptut = Environment.CurrentDirectory + "\\FlightData.log";
        }
    }
}

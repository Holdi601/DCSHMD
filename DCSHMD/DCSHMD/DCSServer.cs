using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace DCSHMD
{
    public class DCSServer
    {
        public Dictionary<string, string> CurrentValue = null;
        public bool TelemetryOutput = false;
        public bool quit = false;
        public void StartDCSListener()
        {
            try
            {
                if (quit) return;
                TcpListener listener = new TcpListener(IPAddress.Parse(General.Settings.IP), General.Settings.Port);
                listener.Start();
                General.Write("DCS Listener started");
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    General.Write("Connected " +client.Client.LocalEndPoint.ToString());
                    StreamReader reader = new StreamReader(client.GetStream());
                    StreamWriter writer = new StreamWriter(client.GetStream());
                    string s = string.Empty;
                    while (true)
                    {
                        if (quit) return;
                        s = reader.ReadLine();
                        if (s == "exit") break;
                        else
                        {
                            if (TelemetryOutput) File.AppendAllText(General.Settings.TelemetryOuptut,"[" + DateTime.UtcNow + "] " + s);
                            CurrentValue = JsonConvert.DeserializeObject<Dictionary<string, string>>(s);
                            //General.Write(stopWatch.ElapsedMilliseconds.ToString());
                            stopWatch.Stop();
                            CurrentValue.Add("FRAMETIME", stopWatch.ElapsedMilliseconds.ToString());
                            stopWatch.Restart();
                        }
                    }
                    reader.Close();
                    writer.Close();
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                General.NoteError(ex);
                Thread.Sleep(500);
                if (quit) return;
            }
            StartDCSListener();
        }
    }
}

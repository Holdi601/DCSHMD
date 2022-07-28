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
        public dynamic CurrentValue = null;
        public void StartDCSListener()
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Parse(General.Settings.IP), General.Settings.Port);
                listener.Start();
                General.Write("DCS Listener started");
                Stopwatch stopWatch = new Stopwatch();
                
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    General.Write("Connected " +client.Client.LocalEndPoint.ToString());
                    StreamReader reader = new StreamReader(client.GetStream());
                    StreamWriter writer = new StreamWriter(client.GetStream());
                    string s = string.Empty;
                    while (true)
                    {
                        s = reader.ReadLine();
                        if (s == "exit") break;
                        else
                        {
                            CurrentValue = JsonConvert.DeserializeObject<dynamic>(s);
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
            }
            StartDCSListener();
        }
    }
}

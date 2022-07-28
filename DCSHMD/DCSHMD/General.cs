using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Windows;
using System.Threading;

namespace DCSHMD
{
    public enum FlightUnits { Knots, Meters, Kilometers}
    public enum CoordUnits { MGRS, MinuteDecimal, SecondDecimal, XY}
    public static class General
    {
        public static MetaInfo Current;
        public static MainWindow MainWindow;
        const string MYDOCFOLDER = "\\DCSHMD";
        static string metaFileName = "\\meta.xml";
        public static Thread DCSServerThread;
        public static Settings Settings;
        public static DCSServer DCSServer;
        public static Thread UIAssigningTextThread;
        static string LogFile = "\\log";
        static string callScript = "dofile(lfs.writedir()..[[Scripts\\Export_DCSHMD.lua]])";

        public static void SaveMetaInfo(MainWindow mw)
        {
            Current = new MetaInfo(mw);
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+ MYDOCFOLDER;
            if(!Directory.Exists(path))Directory.CreateDirectory(path);
            string xml = ToXML(Current);
            Write(xml);
            File.WriteAllText(path + metaFileName, xml);
        }

        public static void Init()
        {
            try
            {
                if (File.Exists(Environment.CurrentDirectory + LogFile)) File.Delete(Environment.CurrentDirectory + LogFile);
            }
            catch(Exception ex)
            {
                NoteError(ex);
            }
            InitSettings();
            InitThreads();
        }

        static void InitThreads()
        {
            DCSServer = new DCSServer();
            DCSServerThread = new Thread(DCSServer.StartDCSListener);
            DCSServerThread.Start();
        }

        public static List<string> GetDCSUserFolder()
        {
            string savedGames = KnownFolders.GetPath(KnownFolder.SavedGames);
            DirectoryInfo dirInfo = new DirectoryInfo(savedGames);
            DirectoryInfo[] subDirs = dirInfo.GetDirectories();
            List<string> list = new List<string>();
            foreach(DirectoryInfo dir in subDirs)
            {
                if (dir.Name.ToLower().StartsWith("dcs")) list.Add(dir.FullName);
            }
            return list;
        }

        public static void UninstallScript()
        {
            foreach (string path in GetDCSUserFolder())
            {
                string luaContent = "";
                if (File.Exists(path + "\\Scripts\\Export.lua"))
                {
                    StreamReader srExp = new StreamReader(path + "\\Scripts\\Export.lua");
                    luaContent = srExp.ReadToEnd();
                    srExp.Close();
                    srExp.Dispose();
                }
                if (luaContent.Contains(callScript))
                {
                    try
                    {
                        luaContent = luaContent.Replace(callScript, "");
                        StreamWriter sw = new StreamWriter(path + "\\Scripts\\Export.lua");
                        sw.WriteLine(luaContent);
                        sw.Close();
                        sw.Dispose();
                        System.Windows.MessageBox.Show("Script was installed successfully");
                    }
                    catch (Exception ex)
                    {
                        NoteError(ex);
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Script was already uninstalled");
                }
            }
        }

        public static void InstallScript()
        {
            StreamReader sr = new StreamReader(Environment.CurrentDirectory + "\\Lua\\Addative.lua");
            string contentAddative = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
            foreach (string pth in GetDCSUserFolder())
            {
                string luaContent = "";
                if (File.Exists(pth + "\\Scripts\\Export.lua"))
                {
                    StreamReader srExp = new StreamReader(pth + "\\Scripts\\Export.lua");
                    luaContent = srExp.ReadToEnd();
                    srExp.Close();
                    srExp.Dispose();
                }
                if (!luaContent.Contains(callScript))
                {
                    try
                    {
                        luaContent = luaContent + "\r\n" + callScript;
                        StreamWriter sw = new StreamWriter(pth + "\\Scripts\\Export.lua");
                        sw.WriteLine(luaContent);
                        sw.Close();
                        sw.Dispose();
                        StreamWriter sw2 = new StreamWriter(pth + "\\Scripts\\Export_DCSHMD.lua");
                        sw2.WriteLine(contentAddative);
                        sw2.Close();
                        sw2.Dispose();
                        System.Windows.MessageBox.Show("Script was installed successfully");
                    }
                    catch (Exception ex)
                    {
                        NoteError(ex);
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Script was already installed");
                }
            }
        }
        static void InitSettings()
        {
            string confPath = Environment.CurrentDirectory + "\\Config";
            if (!Directory.Exists(confPath)) Directory.CreateDirectory(confPath);
            confPath += "\\Settings.xml";
            if (!File.Exists(confPath))
            {
                CreateNewSettings(confPath);
            }
            else
            {
                try
                {
                    string xml = File.ReadAllText(confPath);
                    Settings = LoadFromjsonstring<Settings>(xml);
                    Write("Settings XML Loaded");
                }
                catch (Exception e)
                {
                    NoteError(e);
                    CreateNewSettings(confPath);
                }
            }
        }
        
        public static void NoteError(Exception e)
        {
            Write(e.ToString());
            Write(e.Message);
            Write(e.Source);
            Write(e.StackTrace);
        }
        static void CreateNewSettings(string path)
        {
            Settings = new Settings();
            string xml = ToXML(Settings);
            File.WriteAllText(path, xml);
            Write("New Settings created");
        }

        public static void LoadMetaLast()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + MYDOCFOLDER;
            if (!Directory.Exists(path)) return;
            if (!File.Exists(path + metaFileName)) return;
            string xml = File.ReadAllText(path + metaFileName);
            Current= LoadFromjsonstring<MetaInfo>(xml);
        }

        public static void ApplyWindow(Window w)
        {
            WindowMeta wm= new WindowMeta();
            if (w is MainWindow) wm = Current.main;
            w.Top = wm.Y;
            w.Left = wm.X;
            w.Width = wm.Width;
            w.Height = wm.Height;
        }

        public static void Write(string msg)
        {
            msg ="["+ DateTime.UtcNow.ToString()+"]: " + "\t" + msg;
            System.Diagnostics.Debug.WriteLine(msg);
            File.AppendAllText(Environment.CurrentDirectory + LogFile, msg+"\r\n");
        }

        public static string ToXML<T>(T value)
        {
            using (var stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(value.GetType());
                serializer.Serialize(stringwriter, value);
                return stringwriter.ToString();
            }
        }

        public static T LoadFromjsonstring<T>(string xmlText) where T : class
        {
            using (var stringReader = new System.IO.StringReader(xmlText))
            {
                var serializer = new XmlSerializer(typeof(T));
                return serializer.Deserialize(stringReader) as T;
            }
        }

    }
}

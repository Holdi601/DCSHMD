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
using System.Windows.Controls;
using org.mariuszgromada.math.mxparser;

namespace DCSHMD
{
    public enum FlightUnits { Knots, Meters, Kilometers}
    public enum CoordUnits { MGRS, MinuteDecimal, SecondDecimal, XY}
    public static class General
    {
        public static MetaInfo Current;
        public static MainWindow MainWindow;
        public static Overlay overlay;
        const string MYDOCFOLDER = "\\DCSHMD";
        static string metaFileName = "\\meta.xml";
        public static Thread DCSServerThread;
        public static Thread AssignThread;
        public static ValueAssignmentThread ValueAssignment;
        public static Settings Settings;
        public static DCSServer DCSServer;
        public static Thread UIAssigningTextThread;
        static string LogFile = "\\log";
        static string callScript = "dofile(lfs.writedir()..[[Scripts\\Export_DCSHMD.lua]])";
        public static Dictionary<string, DP_Windows> SourceElements = new Dictionary<string, DP_Windows>();
        public static Dictionary<string, string> SourceElementTypes = new Dictionary<string, string>();
        public static Dictionary<string, Label> ElementLabels = new Dictionary<string, Label>();
        public static Dictionary<string, Function> ElementsCalc = new Dictionary<string, Function>();


        public static void StopThreads(object sender, EventArgs e)
        {
            if (DCSServer != null) DCSServer.quit = true;

        }

        public static void SaveMetaInfo(Window mw)
        {
            if(mw is MainWindow)
            {
                Current = new MetaInfo(mw);
            }else if(mw is Overlay)
            {
                Current.Overlay = new WindowMeta(mw);
            }
            
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
            string contentAddative="";
            while (!sr.EndOfStream)
            {
                string current = sr.ReadLine()+ "\r\n";
                if (!current.Contains("%%INSERTHERE%%"))
                {
                    contentAddative += current;
                }
                else
                {
                    for(int i = 0; i < SourceElements.Count; i++)
                    {
                        if (i == SourceElements.Count - 1) contentAddative += SourceElements.ElementAt(i).Value.ToLuaLine(true);
                        else contentAddative += SourceElements.ElementAt(i).Value.ToLuaLine(false);
                    }
                }
            }
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
                    Settings = LoadFromXMLstring<Settings>(xml);
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
            Current= LoadFromXMLstring<MetaInfo>(xml);
            LoadElements(null, null);
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

        public static T LoadFromXMLstring<T>(string xmlText) where T : class
        {
            using (var stringReader = new System.IO.StringReader(xmlText))
            {
                var serializer = new XmlSerializer(typeof(T));
                return serializer.Deserialize(stringReader) as T;
            }
        }

        public static void SaveElements(object sender, EventArgs e)
        {
            string path = Environment.CurrentDirectory + "\\Elements\\";
            if(!Directory.Exists(path))Directory.CreateDirectory(path);
            foreach(KeyValuePair<string, DP_Windows> kvp in SourceElements)
            {
                string xml = ToXML(kvp.Value);
                string final = "";
                if(kvp.Value is DP_Windows_Text)
                {
                    final = path+"text_" + kvp.Value.Name + ".xml";
                }
                else
                {
                    final= path + kvp.Value.Name + ".xml";
                }
                File.WriteAllText(final, xml);
            }
        }

        public static void LoadElements(object sender, EventArgs e)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + "\\Elements\\")) return;
            DirectoryInfo dirinf = new DirectoryInfo(Environment.CurrentDirectory+"\\Elements\\");
            FileInfo[] files = dirinf.GetFiles();
            foreach (FileInfo file in files)
            {
                if(file.Name.EndsWith(".xml"))
                {
                    string ngl = File.ReadAllText(file.FullName);
                    if (file.Name.StartsWith("text_"))
                    {
                        DP_Windows_Text dpt = LoadFromXMLstring<DP_Windows_Text>(ngl);
                        SourceElements.Add(dpt.Name, dpt);
                        dpt.CreateUpdateElements();
                    }
                    else
                    {
                        DP_Windows dp = LoadFromXMLstring<DP_Windows>(ngl);
                        SourceElements.Add(dp.Name, dp);
                    }
                }
            }
        }

    }
}

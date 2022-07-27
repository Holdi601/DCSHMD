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

namespace DCSHMD
{
    public static class General
    {
        public static MetaInfo Current;
        public static MainWindow MainWindow;
        const string MYDOCFOLDER = "\\DCSHMD";
        static string metaFileName = "\\meta.xml";

        public static void SaveMetaInfo(MainWindow mw)
        {
            Current = new MetaInfo(mw);
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+ MYDOCFOLDER;
            if(!Directory.Exists(path))Directory.CreateDirectory(path);
            string xml = ToXML(Current);
            Write(xml);
            File.WriteAllText(path + metaFileName, xml);
        }

        public static void LoadMetaLast()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + MYDOCFOLDER;
            if (!Directory.Exists(path)) return;
            if (!File.Exists(path + metaFileName)) return;
            string xml = File.ReadAllText(path + metaFileName);
            Current= LoadFromXMLString<MetaInfo>(xml);
           

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
            msg = DateTime.UtcNow.ToString() + "\t" + msg;
            Console.WriteLine(msg);
            File.AppendAllText(Environment.CurrentDirectory + "\\log", msg);
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

        public static T LoadFromXMLString<T>(string xmlText) where T : class
        {
            using (var stringReader = new System.IO.StringReader(xmlText))
            {
                var serializer = new XmlSerializer(typeof(T));
                return serializer.Deserialize(stringReader) as T;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DCSHMD
{
    public struct WindowMeta
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public WindowMeta(double _x, double _y, double _width, double _height)
        {
            X= _x;
            Y= _y;
            Width= _width;
            Height= _height;
        }

        public WindowMeta()
        {
            X = 0;
            Y = 0;
            Width = 0;
            Height = 0;
        }
        public WindowMeta(Window w)
        {
            X = w.Left;
            Y = w.Top;
            Width = w.Width;
            Height = w.Height;
        }
    }

    public class MetaInfo
    {
        public WindowMeta main {get;set;}
        public WindowMeta Overlay { get; set; }

        public MetaInfo(Window mainWindow)
        {
            main=new WindowMeta(mainWindow);
        }

        public MetaInfo()
        {
            main = new WindowMeta();
        }
    }
}

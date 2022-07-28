using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DCSHMD
{
    public class DP_Windows
    {
        public double X;
        public double Y;
        public double Size;
        public System.Windows.HorizontalAlignment Anchor_Horizon;
        public System.Windows.VerticalAlignment Anchor_Vertical;
        private byte r = 255, g = 255, b = 255, a = 255;
        public SolidColorBrush ColorFG()
        {
            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }
        public void ToColorFG(SolidColorBrush col)
        {
            a = col.Color.A;
            r = col.Color.R;
            g = col.Color.G;
            b = col.Color.B;
        }

        public DP_Windows()
        {
            X = 0;
            Y = 0;
            Size = 20;
            Anchor_Vertical = System.Windows.VerticalAlignment.Center;
            Anchor_Horizon = System.Windows.HorizontalAlignment.Center;
            ToColorFG(Brushes.White);
        }

        public DP_Windows(double x, double y, double size, System.Windows.HorizontalAlignment anchor_Horizon, System.Windows.VerticalAlignment anchor_Vertical, SolidColorBrush colorFG)
        {
            X=x;
            Y=y;
            Size = size;
            Anchor_Horizon = anchor_Horizon;
            Anchor_Vertical = anchor_Vertical;
            ToColorFG(colorFG);
        }
    }

    public class DP_Windows_Text : DP_Windows
    {
        public string Text;
        public bool Boxed;
        public DP_Windows_Text()
        {
            Text = "";
            Boxed = false;
        }

        public DP_Windows_Text(string text, bool boxed, double x, double y, double size, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment, SolidColorBrush colorFG) : base(x,y, size, horizontalAlignment, verticalAlignment, colorFG)
        {
            Text = text;
            Boxed = boxed;
        }
    }

    public class DisplayElement
    {
        public string Type { get; set; }
        public string Reference { get; set; }
        public DP_Windows Disp { get; set; }
        public bool Show { get; set; }

    }
}

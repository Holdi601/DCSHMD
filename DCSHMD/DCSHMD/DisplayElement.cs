using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DCSHMD
{
    public struct DP_Windows
    {
        public double X;
        public double Y;
        public double Size;
        public System.Windows.HorizontalAlignment Anchor_Horizon;
        public System.Windows.VerticalAlignment Anchor_Vertical;
        public System.Windows.Media.Brush Color;
        public DP_Windows()
        {
            X = 0;
            Y = 0;
            Size = 20;
            Anchor_Vertical = System.Windows.VerticalAlignment.Center;
            Anchor_Horizon = System.Windows.HorizontalAlignment.Center;
            Color = System.Windows.Media.Brushes.White;
        }
    }

    public class DisplayElement
    {
        public string Type { get; set; }
        public string Reference { get; set; }
        public DP_Windows Disp { get; set; }

    }
}

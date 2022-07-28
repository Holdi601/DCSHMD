using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DCSHMD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FillUnits();
            General.Init();
            General.LoadMetaLast();
            SetupListeners();
            General.ApplyWindow(this);
            General.Write("Init Success");
        }

        public void SetupListeners()
        {
            General.Write("Listeners Setup");
            CloseOverlayBtn.Click += CloseOverlay;
            OpenOverlayBtn.Click += OpenOverlay;
            InstallScriptBtn.Click += InstallScript;
            UninstallScriptBtn.Click += UninstallScript;
            this.SizeChanged += SaveCurrentMetaState;
            this.LocationChanged += SaveCurrentMetaState;
        }

        void SaveCurrentMetaState(object sender, EventArgs e)
        {
            General.Write("Current State Saved");
            General.SaveMetaInfo(this);
        }

        void OpenOverlay(object sender, EventArgs e)
        {
            General.Write("Open Overlay pressed");
            DP_Windows wp = new DP_Windows();
            wp.Anchor_Horizon = HorizontalAlignment.Right;
            wp.Anchor_Vertical= VerticalAlignment.Bottom;
            wp.X = -1111;
            wp.Y = -232323;
            wp.Size = 2323;
            string xml = General.ToXML(wp);
            General.Write(xml);
        }

        void CloseOverlay(object sender, EventArgs e)
        {
            General.Write("Close Overlay pressed");
        }

        void InstallScript(object sender, EventArgs e)
        {
            General.InstallScript();
            General.Write("Script installed");
        }
        void UninstallScript(object sender, EventArgs e)
        {
            General.UninstallScript();
            General.Write("Script uninstalled");
        }

        void FillUnits()
        {
            General.Write("Dropdown filling");
            CoordinatesDD.Items.Clear();
            CoordinatesDD.Items.Add("N00'00\"00.0 E000'00\"00.00");
            CoordinatesDD.Items.Add("N00'00.000 E000'00.000");
            CoordinatesDD.Items.Add("37T BN 0000 0000");
            CoordinatesDD.Items.Add("X0.000000000 Y0.00000000");
            FlightUnitsDD.Items.Clear();
            FlightUnitsDD.Items.Add("Knts Ft");
            FlightUnitsDD.Items.Add("Meters");
            FlightUnitsDD.Items.Add("Kilometers");
        }
    }
}

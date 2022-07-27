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
            SetupListeners();
            General.LoadMetaLast();
            General.ApplyWindow(this);
        }

        public void SetupListeners()
        {
            CloseOverlayBtn.Click += CloseOverlay;
            OpenOverlayBtn.Click += OpenOverlay;
            InstallScriptBtn.Click += InstallScript;
            UninstallScriptBtn.Click += UninstallScript;
            this.SizeChanged += SaveCurrentMetaState;
            this.LocationChanged += SaveCurrentMetaState;
        }

        void SaveCurrentMetaState(object sender, EventArgs e)
        {
            General.SaveMetaInfo(this);
        }

        void OpenOverlay(object sender, EventArgs e)
        {
            
        }

        void CloseOverlay(object sender, EventArgs e)
        {

        }

        void InstallScript(object sender, EventArgs e)
        {

        }
        void UninstallScript(object sender, EventArgs e)
        {

        }

        void FillUnits()
        {
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

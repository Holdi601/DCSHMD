using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DCSHMD
{
    /// <summary>
    /// Interaction logic for Overlay.xaml
    /// </summary>
    public partial class Overlay : Window
    {
        public Overlay()
        {
            InitializeComponent();
            if (General.Current != null)
            {
                if (General.Current.Overlay.Y > 0) this.Top = General.Current.Overlay.Y;
                if (General.Current.Overlay.X > 0) this.Left = General.Current.Overlay.X;
                if (General.Current.Overlay.Width > 0) this.Width = General.Current.Overlay.Width;
                if (General.Current.Overlay.Height > 0) this.Height = General.Current.Overlay.Height;
            }
            else
            {
                General.Current = new MetaInfo();
            }
            this.SizeChanged += SaveCurrentMetaState;
            this.LocationChanged += SaveCurrentMetaState;
            this.Deactivated += Window_Deactivated;
            MoveLabel.MouseLeftButtonDown += LMBDown;
            MouseLeftButtonDown +=  LMBDown;
            SetupLabel();
            MainGrid.ShowGridLines = true;
            CloseBtn.Click += CloseThis;
        }

        void SetupLabel()
        {
            foreach(KeyValuePair<string, Label> kvp in General.ElementLabels)
            {
                Grid.SetColumn(kvp.Value, 0);
                Grid.SetRow(kvp.Value, 0);
                MainGrid.Children.Add(kvp.Value);
            }
        }

        void LMBDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (Exception ex)
            {
                General.NoteError(ex);
            }
        }

        void CloseThis(object sender, EventArgs e)
        {
            Close();
        }

        void SaveCurrentMetaState(object sender, EventArgs e)
        {
            General.Write("Current State Saved");
            General.SaveMetaInfo(this);
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
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
            General.Init();
            General.LoadMetaLast();
            SetupListeners();
            General.ApplyWindow(this);
            RefreshView(null, null);
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
            AddBtn.Click+=Add;
            AddBtn.Name = "AddBtn";
            SaveBtn.Click += General.SaveElements;
            this.Closing += General.StopThreads;
        }

        void SaveCurrentMetaState(object sender, EventArgs e)
        {
            General.Write("Current State Saved");
            General.SaveMetaInfo(this);
        }

        void OpenOverlay(object sender, EventArgs e)
        {
            General.Write("Open Overlay pressed");
            General.overlay = new Overlay();
            General.overlay.Show();
            General.ValueAssignment = new ValueAssignmentThread();
            General.AssignThread = new System.Threading.Thread(General.ValueAssignment.RefresherGo);
            General.AssignThread.Start();
            
            //Thread Start
        }

        void CloseOverlay(object sender, EventArgs e)
        {
            General.Write("Close Overlay pressed");
            if(General.ValueAssignment!=null)
            {
                General.ValueAssignment.quit = true;
            }
            if(General.overlay != null)
            {
                General.overlay.Close();
                //Thread Stop
            }
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

        void RefreshView(object sender, EventArgs e)
        {
            Grid g = getGrid();
            for(int i=0; i<General.SourceElements.Count; ++i)
            {
                Label l = new Label();
                l.Content = General.SourceElements.ElementAt(i).Key;
                l.Foreground = Brushes.White;
                l.HorizontalAlignment = HorizontalAlignment.Left;
                l.VerticalAlignment = VerticalAlignment.Top;
                l.Margin = new Thickness(0, 0, 0, 0);
                Grid.SetColumn(l, 0);
                Grid.SetRow(l, i);
                g.Children.Add(l);

                Button editBtn = new Button();
                editBtn.Name = "editBtn" + i.ToString();
                editBtn.Content = "Edit";
                editBtn.HorizontalAlignment = HorizontalAlignment.Left;
                editBtn.VerticalAlignment = VerticalAlignment.Top;
                editBtn.Margin = new Thickness(100, 0, 0, 0);
                editBtn.Click += Add;
                editBtn.Width = 50;
                Grid.SetColumn(editBtn, 0);
                Grid.SetRow(editBtn, i);
                g.Children.Add(editBtn);

                Button deleteBtn = new Button();
                deleteBtn.Name = "deleteBtn" + i.ToString();
                deleteBtn.Content = "Delete";
                deleteBtn.HorizontalAlignment = HorizontalAlignment.Left;
                deleteBtn.VerticalAlignment = VerticalAlignment.Top;
                deleteBtn.Margin = new Thickness(200, 0, 0, 0);
                deleteBtn.Click += Delete;
                deleteBtn.Width = 50;
                Grid.SetColumn(deleteBtn, 0);
                Grid.SetRow(deleteBtn, i);
                g.Children.Add(deleteBtn);
            }
            SV.Content = g;
        }

        Grid getGrid()
        {
            Grid grid = new Grid();
            for(int i=0; i<General.SourceElements.Count; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }

            return grid;
        }

        void Add(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            if(b.Name== "AddBtn")
            {
                AddElement ae = new AddElement(Type.TEXT_TEXT);
                ae.Closing += RefreshView;
                ae.Show();
            }
            else
            {
                string nbr = b.Name.Replace("editBtn", "");
                int indx = -1;
                int.TryParse(nbr, out indx);
                AddElement ae = new AddElement(General.SourceElements.ElementAt(indx).Value);
                ae.Show();
            }
        }

        void Delete(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            string nbr = b.Name.Replace("deleteBtn", "");
            int indx = -1;
            int.TryParse(nbr, out indx);
            KeyValuePair<string, DP_Windows> kv = General.SourceElements.ElementAt(indx);
            General.SourceElements.Remove(kv.Key);
            string file = Environment.CurrentDirectory + "\\Elements\\";
            if(kv.Value is DP_Windows_Text)
            {
                file += "text_" + kv.Key + ".xml";
            }
            else
            {
                file +=  kv.Key + ".xml";
            }
            if(File.Exists(file))File.Delete(file);
            RefreshView(null, null);
        }

    }
}

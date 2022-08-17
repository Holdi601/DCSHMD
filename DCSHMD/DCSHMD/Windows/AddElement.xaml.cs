using System;
using System.Collections.Generic;
using System.Drawing.Text;
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
    /// Interaction logic for AddElement.xaml
    /// </summary>
    public partial class AddElement : Window
    {
        DP_Windows element;
        SolidColorBrush? color=null;
        Type type;
        string font;
        bool edit;
        public AddElement(Type t)
        {
            InitializeComponent();
            if(t== Type.TEXT_TEXT||t== Type.TEXT_NUMBER)
            {
                element = new DP_Windows_Text();
            }else
            {
                element=new DP_Windows();
                MessageBox.Show("Not yet defined");
            }
            edit = false;
            Init();
        }

        public AddElement(DP_Windows el)
        {
            InitializeComponent();
            element = el;
            edit = true;
            Init();
            FillValuesFromElement();
        }

        void Init()
        {
            FillCB();
            ColorBtn.Click += OpenColorPicker;
            OKBtn.Click += okclose;
            CancelBtn.Click += CloseBTN;
            if (element is DP_Windows_Text)
            {

            }
            else
            {
                FontCB.Visibility = Visibility.Hidden;
                FontLbl.Visibility = Visibility.Hidden;
                PreTB.Visibility = Visibility.Hidden;
                PostTB.Visibility = Visibility.Hidden;
                PreTxtLbl.Visibility = Visibility.Hidden;
                PostTxtLbl.Visibility = Visibility.Hidden;
                RoundLbl.Visibility = Visibility.Hidden;
                RoundingTB.Visibility = Visibility.Hidden;
            }
        }

        void FillCB()
        {
            if(element is DP_Windows_Text)
            {
                TypeCB.Items.Add("TEXT");
                TypeCB.Items.Add("NUMBER");
            }
            foreach (var item in Enum.GetValues(typeof(HorizontalAlignment)))
            {
                HorAnchCB.Items.Add(item);
            }
            foreach (var item in Enum.GetValues(typeof(VerticalAlignment)))
            {
                VerAnchCB.Items.Add(item);
            }
            List<string> fonts = new List<string>();
            InstalledFontCollection installedFonts = new InstalledFontCollection();
            foreach (System.Drawing.FontFamily font in installedFonts.Families)
            {
                fonts.Add(font.Name);
            }
            FontCB.ItemsSource = fonts;
        }

        void FillValuesFromElement()
        {
            NameTB.Text = element.Name;
            ShowCB.IsChecked = element.Show;
            int foundIndex = -1;
            if (element is DP_Windows_Text)
            {
                foundIndex = -1;
                for(int i=0; i<FontCB.Items.Count; ++i)
                {
                    if ((string)FontCB.Items[i]==((DP_Windows_Text)element).Font)
                    {
                        foundIndex = i;
                        break;
                    }
                }
                if(foundIndex >= 0)
                {
                    FontCB.SelectedIndex = foundIndex;
                }
                if (element.Type == Type.TEXT_TEXT) TypeCB.SelectedIndex = 0;
                else if (element.Type == Type.TEXT_NUMBER) TypeCB.SelectedIndex = 1;
                BoxedCheck.IsChecked = ((DP_Windows_Text)element).Boxed;
                PostTB.Text = ((DP_Windows_Text)element).PostText;
                PreTB.Text = ((DP_Windows_Text)element).PreText;
                BorderThickTB.Text = ((DP_Windows_Text)element).BorderThickness.ToString();
                RoundingTB.Text = ((DP_Windows_Text)element).Rounding.ToString();
            }
            AlphaTB.Text = element.a.ToString();
            CalcTB.Text = element.Calc;
            XTB.Text = element.X.ToString();
            YTB.Text = element.Y.ToString();
            SizeTB.Text = element.Size.ToString();
            LuaTB.Text = element.LuaCommand;
            
            foundIndex = -1;
            for (int i = 0; i < HorAnchCB.Items.Count; ++i)
            {
                if ((HorizontalAlignment)HorAnchCB.Items[i] == element.Anchor_Horizon)
                {
                    foundIndex = i;
                    break;
                }
            }
            HorAnchCB.SelectedIndex = foundIndex;
            foundIndex = -1;
            for (int i = 0; i < VerAnchCB.Items.Count; ++i)
            {
                if ((VerticalAlignment)VerAnchCB.Items[i] == element.Anchor_Vertical)
                {
                    foundIndex = i;
                    break;
                }
            }
            VerAnchCB.SelectedIndex = foundIndex;
            color = element.ColorFG();
        }


        void OpenColorPicker(object sender, EventArgs e)
        {
            System.Windows.Forms.ColorDialog dig = new System.Windows.Forms.ColorDialog();
            if (dig.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                color = new SolidColorBrush(Color.FromArgb(dig.Color.A, dig.Color.R, dig.Color.G, dig.Color.B));
            }
        }
        void CloseBTN(object sender, EventArgs e)
        {
            Close();
        }
        void okclose(object sender, EventArgs e)
        {
            string name = NameTB.Text.Trim().Replace(" ", "");
            if(element is DP_Windows_Text)
            {
                if (element is DP_Windows_Text && (TypeCB.SelectedIndex < 0 || FontCB.SelectedIndex < 0))
                {
                    MessageBox.Show("TextValues Not Valued");
                    return;
                }
                if(!int.TryParse(RoundingTB.Text, out ((DP_Windows_Text)element).Rounding))
                {
                    MessageBox.Show("Rounding val not valid");
                }
            }
            if(name.Length < 3)
            {
                MessageBox.Show("Name not properly filled");
                return;
            }
            if (!double.TryParse(XTB.Text, out element.X))
            {
                MessageBox.Show("X not properly filled");
                return;
            }
            if (!double.TryParse(YTB.Text, out element.Y))
            {
                MessageBox.Show("Y not properly filled");
                return;
            }
            if (!double.TryParse(SizeTB.Text, out element.Size))
            {
                MessageBox.Show("Size not properly filled");
                return;
            }
            if (LuaTB.Text.Length < 3)
            {
                MessageBox.Show("Lua not properly filled");
                return;
            }
            if (color == null)
            {
                MessageBox.Show("color not properly filled");
                return;
            }
            if (HorAnchCB.SelectedIndex < 0)
            {
                MessageBox.Show("Horizon Anchor not properly filled");
                return;
            }
            if (VerAnchCB.SelectedIndex < 0)
            {
                MessageBox.Show("Vertical Anchor not properly filled");
                return;
            }
            
            if(name.Length < 3)
            {
                return;
            }
            if (edit)
            {
                if (name != element.Name)
                {
                    General.SourceElements.Remove(element.Name);
                    General.ElementsCalc.Remove(element.Name);
                    element.Name = name;
                    General.SourceElements.Add(name, element);
                    if (element is DP_Windows_Text)
                    {
                        General.ElementLabels.Remove(element.Name);
                    }
                }
            }else
            {
                element.Name = name;
                if(General.SourceElements.ContainsKey(name))
                {
                    MessageBox.Show("Name with element already exist");
                    return;
                }
                General.SourceElements.Add(name, element);
                
            }
            element.Show= ShowCB.IsEnabled==true?true:false;
            element.Calc = CalcTB.Text;
            element.LuaCommand = LuaTB.Text;
            element.Anchor_Horizon = (HorizontalAlignment)HorAnchCB.SelectedItem;
            element.Anchor_Vertical = (VerticalAlignment)HorAnchCB.SelectedItem;
            element.ToColorFG(color);
            byte.TryParse(AlphaTB.Text, out element.a);
            if (element is DP_Windows_Text)
            {
                DP_Windows_Text dwt = (DP_Windows_Text)element;
                dwt.Type = (string)TypeCB.SelectedItem == "TEXT" ? Type.TEXT_TEXT : Type.TEXT_NUMBER;
                dwt.PreText = PreTB.Text;
                dwt.PostText = PostTB.Text;
                dwt.Font = (string)FontCB.SelectedItem;
                if (BorderThickTB.Text.Length > 0) double.TryParse(BorderThickTB.Text, out dwt.BorderThickness);
                dwt.CreateUpdateElements();
            }
            Close();
        }
    }
}

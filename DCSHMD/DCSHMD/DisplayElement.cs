using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DCSHMD
{
    public enum Type { TEXT_TEXT, TEXT_NUMBER, ERROR }
    public class DP_Windows
    {
        public string Name;
        public bool Show;
        public Type Type;
        public string Calc;
        public double X;
        public double Y;
        public double Size;
        public string LuaCommand;
        public System.Windows.HorizontalAlignment Anchor_Horizon;
        public System.Windows.VerticalAlignment Anchor_Vertical;
        public byte r = 255, g = 255, b = 255, a = 255;
        public string[] Variables;
        const string luapre = "    jsonstring=jsonstring..";
        protected Stopwatch sw;
        public bool bench = false;
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
            Type = Type.ERROR;
            Show = true;
            Name = "DEBUG";
            LuaCommand = "";
            Variables = new string[0];
            Calc = "";
            X = 0;
            Y = 0;
            Size = 20;
            Anchor_Vertical = System.Windows.VerticalAlignment.Center;
            Anchor_Horizon = System.Windows.HorizontalAlignment.Center;
            ToColorFG(Brushes.White);
            sw = new Stopwatch();
        }

        public DP_Windows(string name,string calc, string lua, double x, double y, double size, System.Windows.HorizontalAlignment anchor_Horizon, System.Windows.VerticalAlignment anchor_Vertical, SolidColorBrush colorFG, bool show)
        {
            Type type = Type.ERROR;
            Name = name;
            Calc = calc;
            LuaCommand= lua;
            if (calc.Contains("="))
            {
                string vars = calc.Substring(0, calc.IndexOf("="));
                vars = vars.Substring(vars.IndexOf("(") + 1);
                vars = vars.Substring(0, vars.IndexOf(")"));
                string[] varsRaw = vars.Split(',');
                Variables = new string[varsRaw.Length];
                for(int i = 0; i < varsRaw.Length; i++)Variables[i] = varsRaw[i].Trim();
            }
            else
            {
                Variables = new string[0];
            }
            X=x;
            Y=y;
            Size = size;
            Anchor_Horizon = anchor_Horizon;
            Anchor_Vertical = anchor_Vertical;
            ToColorFG(colorFG);
            Show=show;
            sw = new Stopwatch();
        }

        public string ToLuaLine(bool lastElement)
        {
            string result = luapre + "'\"" + Name + "\":\"'.." + LuaCommand + "..'\"";
            if (lastElement) result += "}'\r\n";
            else result += ",'\r\n";
            return result;
        }
    }

    public class DP_Windows_Text : DP_Windows
    {
        public bool Boxed;
        public string PreText;
        public string PostText;
        public string Font;
        public double BorderThickness;
        public int Rounding;
        public DP_Windows_Text()
        {
            Boxed = false;
            Type = Type.TEXT_TEXT;
            Font = "Arial";
            PreText = "";
            PostText = "";
            Rounding = 0;
        }
        public DP_Windows_Text(string name,Type type, string pretext, string posttext, string calc, string lua, bool boxed, double x, double y, double size, string font, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment, SolidColorBrush colorFG, bool show, double borderThickness, int rounding) : base(name, calc, lua, x, y, size, horizontalAlignment, verticalAlignment, colorFG, show)
        {
            Boxed = boxed;
            Font = font;
            Type = type;
            PreText = pretext;
            PostText = posttext;
            BorderThickness = borderThickness;
            Rounding = rounding;
        }

        public void CreateUpdateElements()
        {
            if (General.ElementLabels == null) General.ElementLabels = new Dictionary<string, System.Windows.Controls.Label>();
            string IDLbl = Name + "_LBL";
            Label TextElement;
            if(General.ElementLabels.ContainsKey(IDLbl))
            {
                TextElement = General.ElementLabels[IDLbl];
            }
            else
            {
                TextElement= new Label();
                General.ElementLabels.Add(IDLbl, TextElement);
            }
            TextElement.BorderBrush = ColorFG();
            if (Boxed)
            {
                TextElement.BorderThickness = new Thickness(BorderThickness);
            }
            else
            {
                TextElement.BorderThickness = new Thickness(0);
            }
            TextElement.Name = IDLbl;
            TextElement.FontSize = Size;
            TextElement.FontFamily = new FontFamily(Font);
            TextElement.Content = PreText + "VALUE" + PostText;
            TextElement.Foreground = ColorFG();
            Thickness t = new Thickness();
            if(Anchor_Horizon== HorizontalAlignment.Left||Anchor_Horizon== HorizontalAlignment.Center)
            {
                t.Right = 0;
                t.Left = X;
            }
            else
            {
                t.Left = 0;
                t.Right = X;
            }
            if(Anchor_Vertical== VerticalAlignment.Top||Anchor_Vertical== VerticalAlignment.Center)
            {
                t.Top = Y;
                t.Bottom = 0;
            }
            else
            {
                t.Bottom = Y;
                t.Top = 0;
            }
            
            TextElement.Margin = t;
            if(Type!=Type.TEXT_TEXT&&Calc.Length>2)
            {
                Function f = new Function(Calc);
                if(General.ElementsCalc.ContainsKey(Name))General.ElementsCalc[Name] = f;
                else General.ElementsCalc.Add(Name, f);
                if (Calc.Contains("="))
                {
                    string vars = Calc.Substring(0, Calc.IndexOf("="));
                    vars = vars.Substring(vars.IndexOf("(") + 1);
                    vars = vars.Substring(0, vars.IndexOf(")"));
                    string[] varsRaw = vars.Split(',');
                    Variables = new string[varsRaw.Length];
                    for (int i = 0; i < varsRaw.Length; i++) Variables[i] = varsRaw[i].Trim();
                }
                else
                {
                    Variables = new string[0];
                }
            }
        }

        public void UpdateValue()
        {
            sw.Start();
            string IDLbl = Name + "_LBL";
            if (General.DCSServer == null || General.DCSServer.CurrentValue == null || !General.DCSServer.CurrentValue.ContainsKey(Name))
                return;
            if (Type==Type.TEXT_TEXT||(Type==Type.TEXT_NUMBER&&(!Calc.Contains("=")||Variables.Length<1)))
            {
                General.ElementLabels[IDLbl].Content = PreText + General.DCSServer.CurrentValue[Name] + PostText;
            }else
            {
                string expr = "f(" + General.DCSServer.CurrentValue[Variables[0]].Replace(",", ".");
                for(int i=1; i<Variables.Length; i++)
                {
                    expr += "," + General.DCSServer.CurrentValue[Variables[i]].Replace(",", ".");
                }
                expr += ")";
                org.mariuszgromada.math.mxparser.Expression ex1 = new org.mariuszgromada.math.mxparser.Expression(expr, General.ElementsCalc[Name]);
                string calcRes = ex1.calculate().ToString().Replace(",",".");
                int indxComma = calcRes.IndexOf(".");
                if(indxComma>=0)
                {
                    if(Rounding==0)calcRes= calcRes.Substring(0,indxComma);
                    else
                    {
                        int length = indxComma + 1 + Rounding;
                        if(length<calcRes.Length)
                        {
                            calcRes = calcRes.Substring(0, indxComma + 1 + Rounding);
                        }
                    }
                }
                General.ElementLabels[IDLbl].Content = PreText + calcRes + PostText;
            }
            sw.Stop();
            //General.Write(Name + " " + sw.ElapsedMilliseconds.ToString());
            sw.Reset();
        }
    }

}

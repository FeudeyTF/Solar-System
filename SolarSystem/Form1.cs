using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SolarSystem
{
    public partial class Form1 : Form
    {
        SolarSystem Sys = new SolarSystem(20);
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED       
                return handleParam;
            }
        }
        int Quant = 50;
        int Repeat = 48;
        double Time = 1800;
        double Ellapsed = 0;
        int cx, cy;
        bool isMouseDown = false; // Показвыает нажата ли мышь (для события MouseMove)
        int x1, y1; // Коордиаты нажатия мыши
        public Form1()
        {
            InitializeComponent();
          
            textBoxTime.Text = Time.ToString();
            numericUpDownQuant.Value = Quant;
            numericUpDownRepeat.Value = Repeat;
            textBoxScale.Text = (Body.scale/1E9).ToString();
            textBoxPeriod.Text = (Time * Repeat).ToString();
            cx = panel1.Width / 2; cy = panel1.Height / 2;
            label7.Text = DateTime.Now.ToString("G");

            this.MouseWheel  += new MouseEventHandler(Wheel);
        }

        private void Wheel(object Sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                Body.scale /= 1.1;
                panel1.Invalidate();
            }
            else
            {
                Body.scale *= 1.1;
                panel1.Invalidate();

            }
            textBoxScale.Text = (Body.scale / 1E9).ToString();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            int n = 0;
            Graphics g = e.Graphics;
            g.TranslateTransform(cx,cy);
            for (int i = -1000; i <= 1000; i += 50)
            {
                int diff1 = (int)(i / double.Parse(textBoxScale.Text));
                int diff2 = (int)(1000 / double.Parse(textBoxScale.Text));
                g.DrawLine(new Pen(Color.FromArgb(20, 255, 255, 255)), new Point(diff1, -diff2), new Point(diff1, diff2));
            }
            for (int i = -1000; i <= 1000; i += 50)
            {
                int diff1 = (int)(i / double.Parse(textBoxScale.Text));
                int diff2 = (int)(1000 / double.Parse(textBoxScale.Text));
                g.DrawLine(new Pen(Color.FromArgb(20, 255, 255, 255)), new Point(-diff2, diff1), new Point(diff2, diff1));
            }
            Sys.Paint(g, true);

            g.Dispose();
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            panel1.Invalidate();
        }

        private void textBoxTime_TextChanged(object sender, EventArgs e)
        {
            double NewTime;
            if (double.TryParse(textBoxTime.Text, out NewTime))
                Time = NewTime;
            else
                textBoxTime.Text = Time.ToString();
            textBoxPeriod.Text = (Time * Repeat).ToString();
        }

        private void numericUpDownQuant_ValueChanged(object sender, EventArgs e)
        {
            Quant = (int)numericUpDownQuant.Value;
            timer1.Interval = Quant;
            
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            timer1.Interval = Quant;
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < Repeat; i++)
            {
                Sys.Step(Time);
                Ellapsed+=Time;

            }
            panel1.Invalidate(false);
            label7.Text = DateTime.Now.AddSeconds(Ellapsed).ToString("G");
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            cx = e.X; cy = e.Y;
            panel1.Invalidate();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
            Cursor = Cursors.Default;
            panel1.Invalidate();
        }
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
       
            if (isMouseDown)
            {
                x1 = e.X;
                y1 = e.Y;
                Cursor = Cursors.SizeAll;
            }
        }
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                cx += e.X - x1;
                cy += e.Y - y1;
                panel1.Invalidate();
                x1 = e.X;
                y1 = e.Y;
            }
           
            Cords.Text = e.X + "X " + e.Y+"Y";

            Body f = Sys.Info(e.X, e.Y);
            if (f != null)
            {
                textBoxX.Text = f.name;
                textBoxY.Text = (f.y/Body.scale).ToString();
                textBoxC.Text = "R: " + f.C.R + " G: " + f.C.G + " B: " + f.C.B;

                panelAbout.Location = new Point(e.X + 10, e.Y + 10);
                panelAbout.Visible = true;

            }
            else
            {
                textBoxX.Text = "None";
                textBoxY.Text = "None";
                textBoxC.Text = "None";
                panelAbout.Visible = false;
            }

        }

        private void CreateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreatePlanet newSystem = new CreatePlanet(Sys, panel1);
            newSystem.Show();
        }

        private void ClearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sys.Clear();
            panel1.Invalidate();
        }

        private void ClearLastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sys.ClearLast();
            panel1.Invalidate();
        }

        private void createSolarSystemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sys.Add("Sun", 2E30, 0, 0, 0, 0, 10, Color.Yellow);
            Sys.Add("Mercury", 3.26E23, 57.9E9, 0, 0, 47.8E3, 3, Color.Red);
            Sys.Add("Venus", 4.88E24, 108.2E9, 0, 0, 35E3, 7, Color.Orange);
            Sys.Add("Earth", 6E24, 149.6E9, 0, 0, 29.8E3, 8, Color.Blue);
            Sys.Add("Moon", 7.35E22, 149.984E9, 0, 0, 29E3, 2, Color.White);
            Sys.Add("Mars", 6.43E23, 227.9E9, 0, 0, 24.1E3, 4, Color.Red);
            Sys.Add("Jupiter", 1.9E27, 778.3E9, 0, 0, 13E3, 10, Color.RosyBrown);
            Sys.Add("Saturn", 5.69E26, 1426E9, 0, 0, 9.6E3, 9, Color.Gray);
            Sys.Add("Uranus", 8.69E25, 2871E9, 0, 0, 6.8E3, 9, Color.Green);
            Sys.Add("Neptune", 1.04E26, 4497E9, 0, 0, 5.4E3, 8, Color.Blue);
            panel1.Invalidate();
        }

       

        private void numericUpDownRepeat_ValueChanged(object sender, EventArgs e)
        {
            Repeat = (int)numericUpDownRepeat.Value;
            textBoxPeriod.Text = (Time * Repeat).ToString();
        }
    }
}

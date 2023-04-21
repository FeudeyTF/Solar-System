using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SolarSystem
{
    partial class CreatePlanet : Form
    {
        SolarSystem system;
        Panel panel;
        public CreatePlanet(SolarSystem system, Panel panel)
        {
            InitializeComponent();
            this.system = system;
            this.panel = panel;
        }

        private void buttonCreatePlanet_Click(object sender, EventArgs e)
        {
            double m,x,y,vx,vy;
            int r;
            Color c;
            if (colorDialog1.Color != null)
                c = colorDialog1.Color;
            else c = Color.White;
            double.TryParse(textBoxMass.Text, out m);
            double.TryParse(textBoxCordX.Text, out x);
            double.TryParse(textBoxCordY.Text, out y);
            double.TryParse(textBoxVelY.Text, out vy);
            double.TryParse(textBoxVelX.Text, out vx);
            int.TryParse(textBoxDiametr.Text, out r);
            system.Add(textBoxName.Text, m, x, y, vx, vy, r, c);
            panel.Invalidate();
            this.Close();

        }

        private void buttonColor_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            this.BackColor = colorDialog1.Color;
        }
    }
}

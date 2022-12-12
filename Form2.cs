using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace raspredMaksvell
{
    public partial class Form2 : Form
    {
        Bitmap Bmp;
        Graphics Gr;
        double Kh, Kv;
        Pen P1 = new Pen(Color.Black,2);

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            Resunok();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            Resunok();
        }

        Pen P2 = new Pen(Color.Yellow, 2);
        SolidBrush Br = new SolidBrush(Color.Red);

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form1.Ff1.посмотретьToolStripMenuItem.Enabled = true;
            Form1.Ff1.button1.Enabled = true;
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            Bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Gr = Graphics.FromImage(Bmp);
            Resunok();
        }
        private void Resunok()
        {
            Gr.Clear(pictureBox1.BackColor);
            //X = Kh * x;
            //Yscr = [BM.Height - Kv * y];
            Kh = Math.Pow(1.1, trackBar1.Value) * (Bmp.Width / (Form1.dv * Form1.Vmax));//если написано статик, то так, если нет - Ff2.Form1.dv
            Kv = Math.Pow(1.1, trackBar2.Value) * (10 * Bmp.Height);
            if (Form1.Nans == 0) return;
            for ( int i = 0; i < 500; i++)
            {
                Gr.DrawRectangle(P1, (int)(Kh * i * Form1.dv),(int)(Bmp.Height - Kv * Form1.distrV[i] / (Form1.N * Form1.Nans)),(int)(Kh * Form1.dv), (int)(Kv * Form1.distrV[i] / (Form1.N * Form1.Nans)));
                Gr.FillRectangle(Br, (int)(Kh * i * Form1.dv),(int)(Bmp.Height - Kv * Form1.distrV[i] / (Form1.N * Form1.Nans)),(int)(Kh * Form1.dv), (int)(Kv * Form1.distrV[i] / (Form1.N * Form1.Nans)));
            }
            pictureBox1.Image = Bmp;
        }
    }
}

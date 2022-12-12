using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;//подключили для работы с файломи

namespace raspredMaksvell
{
    public partial class Form1 : Form
    {
        public static Form1 Ff1;
        public static Form2 Ff2;
        public Form1()
        {
            InitializeComponent();
        }
        double Px, Py, Psq, VP1x, VP1y, VN1x, VN1y;
        double VP2x, VP2y, VN2x, VN2y;

        //задание переменных
        public static int N;
        double R;
        public static double Vmax;
        double Xmax; 
        double Ymax;
        double dt;
        double K;

        //задание массива
        double[] X;
        double[] Y;
        double[] Vx;
        double[] Vy;

        int t;//счетчик времени
        double[] Cs;//пройденный путь
        int[] Cn;//счетчик столкновений
        public static double dv = 5;
        public static int[] distrV = new int[500];//Цифровое значение для второй формы
        public static int Nans;
        int time = 1000;

        bool NewModel = true;//переменная логического типа
        Random rnd = new Random();

        Bitmap Bmp1, Bmp2;//создание битмапа(области для рисования)
        Graphics Gr1, Gr2;//создание графика(тут будем рисовать)

        private void посмотретьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Ff2 = new Form2();
            Ff2.Visible = true;
            посмотретьToolStripMenuItem.Enabled = false;
            button1.Enabled = false;
            double Vsr = 0;//Скорость 2 форма
            double Lsr = 0;//Длина пробега 2 форма
            double Nsr = 0;//Ср Частота Соудорений 2 форма
            
            for (int i=0; i<N; i++)
            {
                Vsr += Cs[i];
                Lsr += Cs[i] / (Cn[i] + 1);
                Nsr += (double)Cn[i]; 
            }
            Vsr = Vsr / (N * t * dt);
            Lsr = Lsr / N;
            Nsr = Nsr / (N * t * dt);

            //Вывод итоговых значений в поля текстбокса
            Ff2.textBox1.Text = Vsr.ToString();
            Ff2.textBox2.Text = Lsr.ToString();
            Ff2.textBox3.Text = Nsr.ToString();
            //
            Ff2.dataGridView1.RowCount = 500;//кол-во строк
            for(int i = 0;i < 500; i++)
            {
                Ff2.dataGridView1[0, i].Value = String.Format("[ {0:f2}; {1:f2})", i * dv, (i + 1) * dv); //i-номер строки.заголовок -1, 1 сточка - 0;f2- 2 знака после запятой
                Ff2.dataGridView1[1, i].Value = distrV[i];

            }
        }

        Pen pen1 = new Pen(Color.Red, 2);//создание ручки
        Pen pen2 = new Pen(Color.Green, 2);//создание ручки
        SolidBrush Br = new SolidBrush(Color.Yellow);//создание кисточки

         private void timer1_Tick(object sender, EventArgs e)
         {
            t++;
            textBox8.Text = Convert.ToString(t * dt);
            //движение частиц и рикошет от стенок
            for (int i = 0; i < N; i++)
            {
                X[i] += Vx[i] * dt;
                Y[i] += Vy[i] * dt;
                Cs[i] += Math.Sqrt(Vx[i]*Vx[i] + Vy[i]*Vy[i])*dt;
                //Движение после соприкосновения в обратную сторону
                if ((X[i] >= Xmax - R) && (Vx[i] > 0)) Vx[i] = -Vx[i];
                if ((X[i] <= -Xmax + R)&& (Vx[i] < 0)) Vx[i] = -Vx[i];
                if ((Y[i] >= Ymax - R) && (Vy[i]> 0)) Vy[i] = -Vy[i];
                if ((Y[i] <= -Ymax + R)&& (Vy[i] < 0)) Vy[i] = -Vy[i];
            }

            //соударение частиц между собой
            for (int i = 0; i< N - 1; i++)
            {
                for (int j = i + 1; j < N; j++)
                {
                    Px = X[j] - X[i];//расстояние по игрику между частицами
                    Py = Y[j] - Y[i];//растояние по иксу между частицами
                    Psq = Px * Px + Py * Py;//расстояние между ними
                    if (Psq <= 4 * R * R)
                    {
                        VP1x = ((Vx[i] * Px + Vy[i] * Py) * Px / Psq);
                        VP1y = ((Vx[i] * Px + Vy[i] * Py) * Py / Psq);
                        VP2x = ((Vx[j] * Px + Vy[j] * Py) * Px / Psq);
                        VP2y = ((Vx[j] * Px + Vy[j] * Py) * Py / Psq);
                        VN1x = Vx[i] - VP1x;
                        VN1y = Vy[i] - VP1y;
                        VN2x = Vx[j] - VP2x;
                        VN2y = Vy[j] - VP2y;
                        if ((VP2x - VP1x) * Px + (VP2y - VP1y) * Py < 0)
                        {
                            Vx[i] = VP2x + VN1x;
                            Vy[i] = VP2y + VN1y;
                            Vx[i] = VP1x + VN2x;
                            Vy[j] = VP1y + VN2y;
                            Cn[i]++;
                            Cn[j]++;
                        }
                    }
                }
            }
            if (t % time == 0)
            {
                Nans++;
                for(int i = 0; i < N-1; i++)
                {
                    int index = (int)(Math.Sqrt(Vx[i] * Vx[i] + Vy[i] * Vy[i]) / dv);
                    distrV[index]++;
                }
            }


            Gr2.DrawImage(Bmp1, 0, 0);
            for (int i = 0; i < N; i++)
            {
                Gr2.DrawEllipse(pen1, (int)((Bmp1.Width / 2) + K * (X[i] - R)), (int)((Bmp1.Height / 2) - K * (Y[i] + R)), (int)(2 * K * R), (int)(2 * K * R));
                Gr2.FillEllipse(Br, (int)((Bmp1.Width / 2) + K * (X[i] - R)), (int)((Bmp1.Height / 2) - K * (Y[i] + R)), (int)(2 * K * R), (int)(2 * K * R));
            }
            pictureBox1.Image = Bmp2;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Ff1 = this;
            Bmp1 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Gr1 = Graphics.FromImage(Bmp1);
            Bmp2 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Gr2 = Graphics.FromImage(Bmp2);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (NewModel)
            {
                //забираем переменные из меню
                t = 0;
                N = int.Parse(textBox1.Text);
                R = double.Parse(textBox2.Text);
                Vmax = double.Parse(textBox3.Text);
                Xmax = double.Parse(textBox4.Text) / 2;
                Ymax = double.Parse(textBox5.Text) / 2;
                dt = double.Parse(textBox6.Text);
                K = double.Parse(textBox7.Text);

                //ввод массива с N переменными
                X = new double[N];
                Y = new double[N];
                Vx = new double[N];
                Vy = new double[N];
                Cs = new double[N];
                Cn = new int[N];

                for (int i = 0; i < N; i++)
                {
                    //задается случайное значение скорости и начальное положение
                    X[i] = (2 * rnd.NextDouble() - 1) * (Xmax - R);
                    Y[i] = (2 * rnd.NextDouble() - 1) * (Ymax - R);
                    Vx[i] = (2 * rnd.NextDouble() - 1) * Vmax;
                    Vy[i] = (2 * rnd.NextDouble() - 1) * Vmax;
                    Cs[i] = 0;
                    Cn[i] = 0;
                }

                for (int i = 0; i < 500; i++)
                {
                    distrV[i] = 0;
                }
                Nans = 0;


                Gr1.Clear(pictureBox1.BackColor);//чистит поле
                Gr1.DrawRectangle(pen2, (int)((Bmp1.Width / 2) - K * Xmax), (int)((Bmp1.Height / 2) - K * Ymax), (int)(2 * K * Xmax), (int)(2 * K * Ymax));//рисуется область
                Gr2.DrawImage(Bmp1, 0, 0);
                for (int i = 0; i < N; i++)
                {
                    Gr2.DrawEllipse(pen1, (int)((Bmp1.Width / 2) + K * (X[i] - R)), (int)((Bmp1.Height / 2) - K * (Y[i] + R)), (int)(2 * K * R), (int)(2 * K * R));
                    Gr2.FillEllipse(Br, (int)((Bmp1.Width / 2) + K * (X[i] - R)), (int)((Bmp1.Height / 2) - K * (Y[i] + R)), (int)(2 * K * R), (int)(2 * K * R));
                }// рисуются молекулы
                pictureBox1.Image = Bmp2;
                NewModel = false;
            }

                посмотретьToolStripMenuItem.Enabled = false;//кнопку "посмотреть" больше не нажать

                button1.Enabled = false;//нельзя нажать первую кнопку
                button2.Enabled = true;//можно нажать вторую кнопку
                timer1.Enabled = true;//таймер включается
                                      //Xscr = (int)((Bmp1.Width / 2) + K*X);Yscr = (int)((Bmp1.Width / 2) + K*Y);
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;//включается первая кнопка
            button2.Enabled = false;//выключается вторая кнопка
            timer1.Enabled = false;//выключается таймер
            посмотретьToolStripMenuItem.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();// по нажатию 3 кнопки прога закрывается
        }
    }
}

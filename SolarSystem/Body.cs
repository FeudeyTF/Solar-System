using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SolarSystem
{
    class Body
    {
        public readonly string name;
        private const int HistLen = 100;
        private bool useHistory = false;
        public readonly double m; // Масса [кг]
        public double x, y; // Координаты  [м]
        public double vx, vy; // Скорость [м/с]
        public double ax, ay; // Ускорения в [м/с]
        public readonly int r; // Радиус в пикселях на картинке
        public readonly Brush b; // Цвет на картинке
        public readonly Color C; // Цвет на картинке

        public static double scale = 10E+8; // Масштаб [м/пикс]

        private int[] HistX, HistY;

        public Body(string name, double m, double x, double y, double vx, double vy, int r, Color c)
        {
            this.name = name; this.m = m; 
            this.x = x; this.y = y;
            this.vx = vx; this.vy = vy;
            this.r = r; this.b = new SolidBrush(c);
            this.C = c;
            if (useHistory)
            {
                HistX = new int[HistLen];
                HistY = new int[HistLen];
                for (int i = 0; i < HistX.Length; i++)
                    HistX[i] = HistY[i] = 0;
            }
        }

        public static void ChangeScale(double scale)
        {
            Body.scale=scale;
        }


        public void Paint(Graphics g, bool showname)
        {
            int i = (int) Math.Round(+x / scale);
            int j = (int) Math.Round(-y / scale);

            g.FillEllipse(b, i - r, j - r, 2 * r, 2 * r);

            if (useHistory)
            {
                for (int k = HistX.Length - 1; k > 0; k--)
                {
                    HistX[k] = HistX[k - 1];
                    HistY[k] = HistY[k - 1];
                    if (HistX[k] != 0)
                        g.FillEllipse(b, HistX[k] - 1, HistY[k] - 1, 2, 2);
                }
                HistX[0] = i; HistY[0] = j;
            }
            if (showname)
            {
                g.DrawString(name, new Font("Arial", 8), b, i + 5, j + 2);
                g.DrawString(Math.Round(vx).ToString(), new Font("Arial", 8), b, i + 12, j - 9);
            }
            
        }

        public void Step(double dt)
        {
            double vx2 = vx + ax * dt;
            double vy2 = vy + ay * dt;
            x += (vx + vx2) * 0.5 * dt;
            y += (vy + vy2) * 0.5 * dt;
            vx = vx2;
            vy = vy2;
        }

    }

    class SolarSystem
    {
        public const double G = 6.6743E-11;
        Body[] Bodies;
        private int n; // Число добавленных объектов

        double sqr(double n) { return n * n; }
        public SolarSystem(int N) // Максимальное число объектов
        {
            Bodies = new Body[N];  // Не забыть Солнце
        }

        public void Paint(Graphics g, bool showname)
        {
            for (int i = 0; i < n; i++)
                Bodies[i].Paint(g, showname);
        }

        public bool Add(string name, double m, double x, double y, double vx, double vy, int r, Color c)
        {
            if (n < Bodies.Length)
            {
                Bodies[n++] = new Body(name, m, x, y, vx, vy, r, c);
                return true;
            }
            else
                return false;
        }

        
        public bool Clear()
        {
            if (n != 0)
            {
                for (int i = 0; i < n; i++)
                {
                    Bodies[i] = null;
                }
                n=0;
                return true;
            }
            return false;
        }

        public void ClearLast()
        {
            int amount = 0;
            for(int i =1; i<Bodies.Length; i++)
            {
                if (Bodies[i] != null)
                {
                    amount++;
                }
            }

            Bodies[amount] = null;
            n--;
        }
        public void Step(double dt)
        {
            // Вычисление взаимных ускорений
            for (int i = 0; i < n; i++)
            {
                Bodies[i].ax = 0; Bodies[i].ay = 0;
                for (int j = 0; j < n; j++)
                    if (i != j)
                    {
                        double dx = Bodies[j].x - Bodies[i].x;
                        double dy = Bodies[j].y - Bodies[i].y;
                        double r2 = dx * dx + dy * dy;
                        double a = G * Bodies[j].m / r2;
                        double r = Math.Sqrt(r2);
                        Bodies[i].ax += a * dx / r;
                        Bodies[i].ay += a * dy / r;
                    }
            }

            for (int i = 0; i < n; i++) Bodies[i].Step(dt);
        }
        public Body Info(int mx, int my)
        {
            for (int i = 0; i < n; i++)
                if (sqr(mx*Body.scale - Bodies[i].x) + sqr(my*Body.scale - Bodies[i].y) < sqr(Bodies[i].r))
                    return Bodies[i];
            return null;
        }

    }
}

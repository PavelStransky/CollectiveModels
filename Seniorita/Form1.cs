using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PavelStransky.GCM;
using PavelStransky.Math;

namespace Seniorita
{
    public partial class Form1 : Form
    {
        LHOQuantumGCMA5D gcm;
        int w, h;

        public Form1()
        {
            InitializeComponent();
        }

        private void putPixel(int x, int y, Color c)
        {
            Bitmap bmp = (Bitmap)p.Image;

            if (x < 0 || x >= w || y < 0 || y >= h) return;

            int r = 2;

            for (int xx = Math.Max(x - r, 0); xx <= Math.Min(x + r, w - 1); xx++)
                for (int yy = Math.Max(y - r, 0); yy <= Math.Min(y + r, h - 1); yy++)
                    bmp.SetPixel(xx, yy, c);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            int maxE = 150;
            int numSteps = 100;
            double hbar = 0.03;

            double a0;
            double a0min = 0.1;
            double summin = 1E10;

            for (a0 = 0.1; a0 <= 1; a0 += 0.01)
            {
                gcm = new LHOQuantumGCMA5D(-1, 1.09, 1, 1, a0, hbar);
                double sum = gcm.HamiltonianMatrixTrace(maxE, numSteps, null);
                if (summin > sum)
                {
                    a0min = a0;
                    summin = sum;
                }
            }
            a0 = a0min / 2.0;

            gcm = new LHOQuantumGCMA5D(-1, 1.09, 1, 1, a0, hbar);
            gcm.Compute(maxE, numSteps, true, 800, null, LHOQuantumGCM.ComputeMethod.LAPACKBand);
            Vector v = gcm.GetEigenValues();

            int cnt = v.Length / 3;
            PointD[] points = new PointD[cnt];
            LHO5DIndex index = gcm.index;

            for (int i = 0; i < cnt; i++)
            {
                Vector eig = gcm.GetEigenVector(i);
                double s = 0;

                for (int j = 0; j < eig.Length; j++)
                {
                    int lambda = 3 * index.Mu[j];
                    s += eig[j] * eig[j] * lambda * (lambda + 3);
                }
                points[i] = new PointD(v[i], -Math.Sqrt(s));
            }


            double minX = points[0].X;
            double maxX = minX;
            double minY = points[0].Y;
            double maxY = minY;

            for (int i = 1; i < cnt; i++) {
                PointD P = points[i];
                if (P.X < minX) minX = P.X;
                if (P.X > maxX) maxX = P.X;
                if (P.Y < minY) minY = P.Y;
                if (P.Y > maxY) maxY = P.Y;
            }


            w = p.ClientRectangle.Width;
            h = p.ClientRectangle.Height;

            Bitmap bmp = new Bitmap(w, h);
            p.Image = bmp;

            double sX = w / (maxX - minX + 1);
            double sY = h / (maxY - minY + 1);

            for (int i = 0; i < cnt; i++)
            {
                PointD P = points[i];
                int x = (int) (((P.X - minX) * sX));
                int y = (int) (((P.Y - minY) * sY));
                putPixel(x, y, Color.Red);
            }

        }
    }
}
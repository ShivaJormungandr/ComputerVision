using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace ComputerVision
{
    public partial class MainForm : Form
    {
        private string sSourceFileName = "";
        private FastImage workImage;
        private Bitmap originalImage = null;

        public MainForm()
        {
            InitializeComponent();
            cbGrayscale.SelectedIndex = 0;
            cbReflexion.SelectedIndex = 0;
            tbWeight.Text = "3";
        }

        private void SafeExecute(Action action)
        {
            if (workImage == null) return;

            workImage.Lock();
            action();
            workImage.Unlock();
        }

        private void RefreshImage(Bitmap img)
        {
            if (workImage == null) return;

            panelDestination.BackgroundImage = null;
            panelDestination.BackgroundImage = workImage.Image;
        }

        private void ApplyGrayscale()
        {

            Color color;

            for (int i = 0; i < workImage.Width; i++)
            {
                for (int j = 0; j < workImage.Height; j++)
                {
                    color = workImage.GetPixel(i, j);
                    byte R = color.R;
                    byte G = color.G;
                    byte B = color.B;

                    byte average;

                    if (cbGrayscale.SelectedItem.ToString() == "AVG")
                    {
                        average = (byte)((R + G + B) / 3);
                    }
                    else
                    {
                        average = (byte)((0.299 * R + 0.587 * G + 0.114 * B) / 3);
                    }

                    color = Color.FromArgb(average, average, average);

                    workImage.SetPixel(i, j, color);
                }
            }
        }

        private void ApplyNegative()
        {
            Color color;

            for (int i = 0; i < workImage.Width; i++)
            {
                for (int j = 0; j < workImage.Height; j++)
                {
                    color = workImage.GetPixel(i, j);
                    byte R = color.R;
                    byte G = color.G;
                    byte B = color.B;

                    byte negativeR = (byte)(255 - R);
                    byte negativeG = (byte)(255 - G);
                    byte negativeB = (byte)(255 - B);


                    color = Color.FromArgb(negativeR, negativeG, negativeB);

                    workImage.SetPixel(i, j, color);
                }
            }
        }

        private void ApplyBrightnessChange()
        {
            Color color;
            int delta = tbBrightness.Value;

            for (int i = 0; i < workImage.Width; i++)
            {
                for (int j = 0; j < workImage.Height; j++)
                {
                    color = workImage.GetPixel(i, j);
                    byte R = color.R;
                    byte G = color.G;
                    byte B = color.B;


                    int newR = delta + R;
                    int newG = delta + G;
                    int newB = delta + B;

                    color = Color.FromArgb(newR < 0 ? 0 : newR > 255 ? 255 : newR,
                                            newG < 0 ? 0 : newG > 255 ? 255 : newG,
                                            newB < 0 ? 0 : newB > 255 ? 255 : newB);

                    workImage.SetPixel(i, j, color);
                }
            }
        }

        private void ApplyContrastChange()
        {
            int minR = 255;
            int minG = 255;
            int minB = 255;
            int maxR = 0;
            int maxG = 0;
            int maxB = 0;

            for (int i = 0; i < workImage.Width; i++)
            {
                for (int j = 0; j < workImage.Height; j++)
                {
                    var color = workImage.GetPixel(i, j);

                    int R = color.R;
                    int G = color.G;
                    int B = color.B;

                    if (minR > R) minR = R;
                    if (minG > G) minG = G;
                    if (minB > B) minB = B;
                    if (maxR < R) maxR = R;
                    if (maxG < G) maxG = G;
                    if (maxB < B) maxB = B;

                }
            }

            int delta = tbContrast.Value;
            int Ra = minR - delta;
            int Ga = minG - delta;
            int Ba = minB - delta;
            int Rb = maxR + delta;
            int Gb = maxG + delta;
            int Bb = maxB + delta;

            for (int i = 0; i < workImage.Width; i++)
            {
                for (int j = 0; j < workImage.Height; j++)
                {
                    var color = workImage.GetPixel(i, j);
                    int R = color.R;
                    int G = color.G;
                    int B = color.B;


                    int newR = (Rb - Ra) * (R - minR) / (maxR - minR) + Ra;
                    int newG = (Gb - Ga) * (G - minG) / (maxG - minG) + Ga;
                    int newB = (Bb - Ba) * (B - minB) / (maxB - minB) + Ba;

                    color = Color.FromArgb(newR < 0 ? 0 : newR > 255 ? 255 : newR,
                                            newG < 0 ? 0 : newG > 255 ? 255 : newG,
                                            newB < 0 ? 0 : newB > 255 ? 255 : newB);

                    workImage.SetPixel(i, j, color);
                }
            }
        }

        private void ApplyHistogramEqualisationGrayscale()
        {
            int[] histo = new int[256];

            for (int i = 0; i < workImage.Width; i++)
            {
                for (int j = 0; j < workImage.Height; j++)
                {
                    Color color = workImage.GetPixel(i, j);
                    byte R = color.R;
                    byte G = color.G;
                    byte B = color.B;

                    byte average;

                    if (cbGrayscale.SelectedItem.ToString() == "AVG")
                    {
                        average = (byte)((R + G + B) / 3);
                    }
                    else
                    {
                        average = (byte)((0.299 * R + 0.587 * G + 0.114 * B) / 3);
                    }

                    histo[average] += 1;
                }
            }

            int[] histoC = new int[256];

            for (int i = 1; i < 255; i++)
            {
                histoC[i] = histoC[i - 1] + histo[i];
            }

            int[] transf = new int[256];
            for (int i = 0; i < histo.Length; i++)
            {
                transf[i] = (histoC[i] * 255) / (workImage.Width * workImage.Height);
            }

            for (int i = 0; i < workImage.Width; i++)
            {
                for (int j = 0; j < workImage.Height; j++)
                {
                    var color = workImage.GetPixel(i, j);

                    byte R = color.R;
                    byte G = color.G;
                    byte B = color.B;

                    byte average;

                    if (cbGrayscale.SelectedItem.ToString() == "AVG")
                    {
                        average = (byte)((R + G + B) / 3);
                    }
                    else
                    {
                        average = (byte)((0.299 * R + 0.587 * G + 0.114 * B) / 3);
                    }


                    color = Color.FromArgb(transf[average], transf[average], transf[average]);

                    workImage.SetPixel(i, j, color);

                }
            }
        }

        private void ApplyReflexion()
        {
            workImage.Unlock();
            Bitmap originalBit = workImage.Image.Clone(new Rectangle(0, 0, workImage.Width, workImage.Height), workImage.Image.PixelFormat);
            FastImage original = new FastImage(originalBit);
            workImage.Lock();
            original.Lock();

            if (cbReflexion.SelectedItem.ToString() == "Horizontal")
            {
                var yLine = workImage.Height / 2;
                for (int i = 0; i < workImage.Width; i++)
                {
                    for (int j = 0; j < workImage.Height; j++)
                    {
                        var color = original.GetPixel(i, j);
                        var newY = 2 * yLine - j;
                        if (newY >= workImage.Height || newY < 0) continue;
                        workImage.SetPixel(i, newY, color);
                    }
                }
            }
            else if (cbReflexion.SelectedItem.ToString() == "Vertical")
            {
                var xLine = workImage.Width / 2;
                for (int i = 0; i < workImage.Width; i++)
                {
                    for (int j = 0; j < workImage.Height; j++)
                    {
                        var color = original.GetPixel(i, j);
                        var newX = 2 * xLine - i;
                        if (newX >= workImage.Width || newX < 0) continue;
                        workImage.SetPixel(newX, j, color);
                    }
                }
            }
            else
            {
                var theta = 45;
                int x0 = workImage.Width / 2;
                int y0 = workImage.Height / 2;

                for (int i = 0; i < workImage.Width; i++)
                {
                    for (int j = 0; j < workImage.Height; j++)
                    {
                        var color = original.GetPixel(i, j);
                        var delta = (i - x0) * Math.Sin(theta) - (j - y0) * Math.Cos(theta);
                        var newX = i - 2 * delta * Math.Sin(theta);
                        var newY = j - 2 * delta * Math.Cos(theta);
                        if (newX == workImage.Width || newX < 0) continue;
                        if (newY == workImage.Height || newY < 0) continue;
                        //workImage.SetPixel(newX, newY, color);
                    }
                }
            }
        }

        private void ApplayLowPassFilter()
        {

            var weightString = tbWeight.Text;
            int weight;
            var habemusNumar = int.TryParse(weightString, out weight);
            if (!habemusNumar) return;

            double[,] h = new double[3, 3];

            h[0, 0] = 1;
            h[0, 1] = weight;
            h[0, 2] = 1;
            h[1, 0] = weight;
            h[1, 1] = weight * weight;
            h[1, 2] = weight;
            h[2, 0] = 1;
            h[2, 1] = weight;
            h[2, 2] = 1;

            for (int r = 1; r < workImage.Height - 1; r++)
            {
                for (int c = 1; c < workImage.Width - 1; c++)
                {
                    double sumR = 0;
                    double sumG = 0;
                    double sumB = 0;
                    for (int row = r - 1; row <= r + 1; row++)
                    {
                        for (int col = c - 1; col <= c + 1; col++)
                        {
                            var color = workImage.GetPixel(col, row);

                            sumR += color.R * h[row - r + 1, col - c + 1];
                            sumG += color.G * h[row - r + 1, col - c + 1];
                            sumB += color.B * h[row - r + 1, col - c + 1];
                        }
                    }

                    sumR /= ((weight + 2) * (weight + 2));
                    sumG /= ((weight + 2) * (weight + 2));
                    sumB /= ((weight + 2) * (weight + 2));
                    var cl = Color.FromArgb((int)sumR, (int)sumG, (int)sumB);
                    workImage.SetPixel(c, r, cl);
                }
            }
        }

        private void ApplayHighPassFilter()
        {
            workImage.Unlock();
            Bitmap originalBit = workImage.Image.Clone(new Rectangle(0, 0, workImage.Width, workImage.Height), workImage.Image.PixelFormat);
            FastImage original = new FastImage(originalBit);
            workImage.Lock();
            original.Lock();

            var choiseString = tbWeight.Text;
            int choise;
            var habemusNumar = int.TryParse(choiseString, out choise);
            if (!habemusNumar) return;

            double[,] h = new double[3, 3];

            switch (choise)
            {
                case 1:
                    h[0, 0] = 0;
                    h[0, 1] = -1;
                    h[0, 2] = 0;
                    h[1, 0] = -1;
                    h[1, 1] = 5;
                    h[1, 2] = -1;
                    h[2, 0] = 0;
                    h[2, 1] = -1;
                    h[2, 2] = 0;
                    break;
                case 2:
                    h[0, 0] = -1;
                    h[0, 1] = -1;
                    h[0, 2] = -1;
                    h[1, 0] = -1;
                    h[1, 1] = 9;
                    h[1, 2] = -1;
                    h[2, 0] = -1;
                    h[2, 1] = -1;
                    h[2, 2] = -1;
                    break;
                case 3:
                    h[0, 0] = 1;
                    h[0, 1] = -2;
                    h[0, 2] = 1;
                    h[1, 0] = -2;
                    h[1, 1] = 5;
                    h[1, 2] = -2;
                    h[2, 0] = 1;
                    h[2, 1] = -2;
                    h[2, 2] = 1;
                    break;
            }

            for (int r = 1; r < workImage.Height - 1; r++)
            {
                for (int c = 1; c < workImage.Width - 1; c++)
                {
                    double sumR = 0;
                    double sumG = 0;
                    double sumB = 0;
                    for (int row = r - 1; row <= r + 1; row++)
                    {
                        for (int col = c - 1; col <= c + 1; col++)
                        {
                            var color = original.GetPixel(col, row);

                            sumR += color.R * h[row - r + 1, col - c + 1];
                            sumG += color.G * h[row - r + 1, col - c + 1];
                            sumB += color.B * h[row - r + 1, col - c + 1];
                        }
                    }
                    sumR = sumR > 255 ? 255 : sumR;
                    sumR = sumR < 0 ? 0 : sumR;
                    sumG = sumG > 255 ? 255 : sumG;
                    sumG = sumG < 0 ? 0 : sumG;
                    sumB = sumB > 255 ? 255 : sumB;
                    sumB = sumB < 0 ? 0 : sumB;

                    var cl = Color.FromArgb((int)sumR, (int)sumG, (int)sumB);
                    workImage.SetPixel(c, r, cl);
                }
            }
        }

        private void ApplayUnsharpMask()
        {
            workImage.Unlock();
            Bitmap originalBit = workImage.Image.Clone(new Rectangle(0, 0, workImage.Width, workImage.Height), workImage.Image.PixelFormat);
            FastImage original = new FastImage(originalBit);
            workImage.Lock();
            original.Lock();

            var c = 0.6;

            var weightString = tbWeight.Text;
            int weight;
            var habemusNumar = int.TryParse(weightString, out weight);
            if (!habemusNumar) return;

            double[,] h = new double[3, 3];

            h[0, 0] = 1;
            h[0, 1] = weight;
            h[0, 2] = 1;
            h[1, 0] = weight;
            h[1, 1] = weight * weight;
            h[1, 2] = weight;
            h[2, 0] = 1;
            h[2, 1] = weight;
            h[2, 2] = 1;

            for (int row = 1; row < workImage.Height - 1; row++)
            {
                for (int col = 1; col < workImage.Width - 1; col++)
                {
                    var color = original.GetPixel(col, row);

                    double sumR = 0;
                    double sumG = 0;
                    double sumB = 0;

                    for (int rowLPF = row - 1; rowLPF <= row + 1; rowLPF++)
                    {
                        for (int colLPF = col - 1; colLPF <= col + 1; colLPF++)
                        {
                            var colorLPF = workImage.GetPixel(colLPF, rowLPF);

                            sumR += colorLPF.R * h[rowLPF - row + 1, colLPF - col + 1];
                            sumG += colorLPF.G * h[rowLPF - row + 1, colLPF - col + 1];
                            sumB += colorLPF.B * h[rowLPF - row + 1, colLPF - col + 1];
                        }
                    }

                    sumR /= ((weight + 2) * (weight + 2));
                    sumG /= ((weight + 2) * (weight + 2));
                    sumB /= ((weight + 2) * (weight + 2));

                    var gR = ((c / (2 * c - 1)) * color.R) - (((1 - c) / (2 * c - 1)) * sumR);
                    var gG = ((c / (2 * c - 1)) * color.G) - (((1 - c) / (2 * c - 1)) * sumG);
                    var gB = ((c / (2 * c - 1)) * color.B) - (((1 - c) / (2 * c - 1)) * sumB);

                    gR = gR > 255 ? 255 : gR;
                    gR = gR < 0 ? 0 : gR;
                    gG = gG > 255 ? 255 : gG;
                    gG = gG < 0 ? 0 : gG;
                    gB = gB > 255 ? 255 : gB;
                    gB = gB < 0 ? 0 : gB;

                    var cl = Color.FromArgb((int)gR, (int)gG, (int)gB);
                    workImage.SetPixel(col, row, cl);
                }
            }
        }

        private void ApplyMedianFilter()
        {
            throw new NotImplementedException();
        }

        private Color CBP(int x, int y, int CS, int SR, int T)
        {
            Dictionary<Color, int> Q = new Dictionary<Color, int>();

            for (int col = x - SR; col <= x + SR; col++)
            {
                if (col <= 0 || col >= workImage.Width) continue;
                for (int row = y - SR; row <= y + SR; row++)
                {
                    if (row <= 0 || row >= workImage.Height) continue;
                    if (col == x && row == y) continue;
                    if (SAD(x, y, col, row, CS) < T && !Salt_Pepper(col, row))
                    {
                        var c = workImage.GetPixel(col, row);
                        if (Q.ContainsKey(c))
                        {
                            Q[c] = Q[c]++;
                        }
                        else
                        {
                            Q.Add(c, 1);
                        }
                    }
                }
            }

            if (Q.Count == 0)
            {
                var t = workImage.GetPixel(x, y);
                Q.Add(Color.Black, 1);
            }
            var maxFreq = Q.Values.Max();
            return Q.Keys.Where(color => Q[color] == maxFreq).First();
        }
        private bool Salt_Pepper(int x, int y)
        {
            var c = workImage.GetPixel(x, y);
            if (c.ToArgb() == Color.Black.ToArgb() || c.ToArgb() == Color.White.ToArgb()) return true;
            return false;
        }
        private int SAD(int x1, int y1, int x2, int y2, int CS)
        {
            int S = 0;
            for (int col = -CS / 2; col <= CS / 2; col++)
            {
                if ((col + x1 <= 0 || col + x1 >= workImage.Width)
                    || (col + x2 <= 0 || col + x2 >= workImage.Width)) continue;
                for (int row = -CS / 2; row <= CS / 2; row++)
                {
                    if ((row + y1 <= 0 || row + y1 >= workImage.Height)
                        || (row + y2 <= 0 || row + y2 >= workImage.Height)) continue;
                    if (col == 0 && row == 0) continue;

                    var c1 = workImage.GetPixel(col + x1, row + y1);
                    var c2 = workImage.GetPixel(col + x2, row + y2);
                    S += Math.Abs(c1.ToArgb() - c2.ToArgb());
                }
            }
            return S;
        }

        private void CBPF(int CS = 3, int SR = 4, int T = 500)
        {
            for (int col = 0; col < workImage.Width; col++)
            {
                for (int row = 0; row < workImage.Height; row++)
                {
                    if (Salt_Pepper(col, row))
                    {
                        workImage.SetPixel(col, row, CBP(col, row, CS, SR, T));
                    }
                }
            }
        }

        private void ApplyMarkovFilter()
        {
            CBPF();
        }

        private void ApplyKirsch()
        {
            workImage.Unlock();
            Bitmap originalBit = workImage.Image.Clone(new Rectangle(0, 0, workImage.Width, workImage.Height), workImage.Image.PixelFormat);
            FastImage original = new FastImage(originalBit);
            workImage.Lock();
            original.Lock();

            double[,] h1 =
            {
                { -1, 0, 1 },
                { -1, 0, 1 },
                { -1, 0, 1 }
            };
            double[,] h2 =
            {
                { 1, 1, 1 },
                { 0, 0, 0 },
                { -1, -1, -1 }
            };
            double[,] h3 =
            {
                { 0, 1, 1 },
                { -1, 0, 1 },
                { -1, -1, 0 }
            };
            double[,] h4 =
            {
                { 1, 1, 0 },
                { 1, 0, -1 },
                { 0, -1, -1 }
            };

            for (int r = 1; r < workImage.Height - 1; r++)
            {
                for (int c = 1; c < workImage.Width - 1; c++)
                {
                    double[] sumR = { 0, 0, 0, 0 };
                    double[] sumG = { 0, 0, 0, 0 };
                    double[] sumB = { 0, 0, 0, 0 };


                    for (int row = r - 1; row <= r + 1; row++)
                    {
                        for (int col = c - 1; col <= c + 1; col++)
                        {
                            var color = original.GetPixel(col, row);

                            sumR[0] += color.R * h1[row - r + 1, col - c + 1];
                            sumG[0] += color.G * h1[row - r + 1, col - c + 1];
                            sumB[0] += color.B * h1[row - r + 1, col - c + 1];

                            sumR[1] += color.R * h2[row - r + 1, col - c + 1];
                            sumG[1] += color.G * h2[row - r + 1, col - c + 1];
                            sumB[1] += color.B * h2[row - r + 1, col - c + 1];

                            sumR[2] += color.R * h3[row - r + 1, col - c + 1];
                            sumG[2] += color.G * h3[row - r + 1, col - c + 1];
                            sumB[2] += color.B * h3[row - r + 1, col - c + 1];

                            sumR[3] += color.R * h4[row - r + 1, col - c + 1];
                            sumG[3] += color.G * h4[row - r + 1, col - c + 1];
                            sumB[3] += color.B * h4[row - r + 1, col - c + 1];
                        }
                    }

                    double sumRM = sumR.Max();
                    double sumGM = sumR.Max();
                    double sumBM = sumR.Max();


                    sumRM = sumRM > 255 ? 255 : sumRM;
                    sumRM = sumRM < 0 ? 0 : sumRM;
                    sumGM = sumGM > 255 ? 255 : sumGM;
                    sumGM = sumGM < 0 ? 0 : sumGM;
                    sumBM = sumBM > 255 ? 255 : sumBM;
                    sumBM = sumBM < 0 ? 0 : sumBM;

                    var cl = Color.FromArgb((int)sumRM, (int)sumGM, (int)sumBM);
                    workImage.SetPixel(c, r, cl);
                }
            }
        }

        private void ApplyPrewitt()
        {
            workImage.Unlock();
            Bitmap originalBit = workImage.Image.Clone(new Rectangle(0, 0, workImage.Width, workImage.Height), workImage.Image.PixelFormat);
            FastImage original = new FastImage(originalBit);
            workImage.Lock();
            original.Lock();

            double[,] p =
            {
                { 1, 1, 1 },
                { 0, 0, 0 },
                { -1, -1, -1 }
            };
            double[,] q =
            {
                { -1, 0, 1 },
                { -1, 0, 1 },
                { -1, 0, 1 }
            };


            for (int r = 1; r < workImage.Height - 1; r++)
            {
                for (int c = 1; c < workImage.Width - 1; c++)
                {
                    double sumRp = 0;
                    double sumGp = 0;
                    double sumBp = 0;

                    double sumRq = 0;
                    double sumGq = 0;
                    double sumBq = 0;


                    for (int row = r - 1; row <= r + 1; row++)
                    {
                        for (int col = c - 1; col <= c + 1; col++)
                        {
                            var color = original.GetPixel(col, row);

                            sumRp += color.R * p[row - r + 1, col - c + 1];
                            sumGp += color.G * p[row - r + 1, col - c + 1];
                            sumBp += color.B * p[row - r + 1, col - c + 1];

                            sumRq += color.R * q[row - r + 1, col - c + 1];
                            sumGq += color.G * q[row - r + 1, col - c + 1];
                            sumBq += color.B * q[row - r + 1, col - c + 1];
                        }
                    }

                    double RR = Math.Sqrt((Math.Pow(sumRp, 2) + Math.Pow(sumRq, 2)));
                    double RG = Math.Sqrt((Math.Pow(sumGp, 2) + Math.Pow(sumGq, 2)));
                    double RB = Math.Sqrt((Math.Pow(sumBp, 2) + Math.Pow(sumBq, 2)));


                    RR = RR > 255 ? 255 : RR;
                    RR = RR < 0 ? 0 : RR;
                    RG = RG > 255 ? 255 : RG;
                    RG = RG < 0 ? 0 : RG;
                    RB = RB > 255 ? 255 : RB;
                    RB = RB < 0 ? 0 : RB;

                    var cl = Color.FromArgb((int)RR, (int)RG, (int)RB);
                    workImage.SetPixel(c, r, cl);
                }
            }
        }

        private void ApplyFreiChen()
        {
            workImage.Unlock();
            Bitmap originalBit = workImage.Image.Clone(new Rectangle(0, 0, workImage.Width, workImage.Height), workImage.Image.PixelFormat);
            FastImage original = new FastImage(originalBit);
            workImage.Lock();
            original.Lock();

            double[,,] f =
            {
                { { 1, Math.Sqrt(2), 1 },{ 0, 0, 0 },{ -1, -Math.Sqrt(2), -1 } },
                { { 1, 0, -1 },{ Math.Sqrt(2), 0, -Math.Sqrt(2) },{ 1, 0, -1 } },
                { { 0, -1, Math.Sqrt(2) },{ 1, 0, -1 },{ -Math.Sqrt(2), 1, 0 } },
                { { Math.Sqrt(2), -1, 0 },{ -1, 0, 1 },{ 0, 1, -Math.Sqrt(2) } },
                { { 0, 1, 0 },{ -1, 0, -1 },{ 0, 1, 0 } },
                { { -1, 0, 1 },{ 0, 0, 0 },{ 1, 0, -1 } },
                { { 1, -2, 1 },{ -2, 4, -2 },{ 1, -2, 1 } },
                { { -2, 1, -2 },{ 1, 4, 1 },{ -2, 1, -2 } },
                { { 1.0/9, 1.0/9, 1.0/9 },{ 1.0/9, 1.0/9, 1.0/9 },{ 1.0/9, 1.0/9, 1.0/9 } },
            };



            for (int r = 1; r < workImage.Height - 1; r++)
            {
                for (int c = 1; c < workImage.Width - 1; c++)
                {
                    double[] sumR = { 0, 0, 0, 0, 0, 0, 0, 0, 0, };
                    double[] sumG = { 0, 0, 0, 0, 0, 0, 0, 0, 0, };
                    double[] sumB = { 0, 0, 0, 0, 0, 0, 0, 0, 0, };

                    for (int row = r - 1; row <= r + 1; row++)
                    {
                        for (int col = c - 1; col <= c + 1; col++)
                        {
                            var color = original.GetPixel(col, row);

                            for (int i = 0; i < 9; i++)
                            {
                                sumR[i] += color.R * f[i, row - r + 1, col - c + 1];
                                sumG[i] += color.G * f[i, row - r + 1, col - c + 1];
                                sumB[i] += color.B * f[i, row - r + 1, col - c + 1];
                            }

                        }
                    }

                    double sumRs = 0;
                    double sumGs = 0;
                    double sumBs = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        sumRs += Math.Pow(sumR[i], 2);
                        sumGs += Math.Pow(sumG[i], 2);
                        sumBs += Math.Pow(sumB[i], 2);
                    }

                    double sumRj = 0;
                    double sumGj = 0;
                    double sumBj = 0;
                    for (int i = 0; i < 9; i++)
                    {
                        sumRj += Math.Pow(sumR[i], 2);
                        sumGj += Math.Pow(sumG[i], 2);
                        sumBj += Math.Pow(sumB[i], 2);
                    }

                    double RR = sumRj == 0 ? 0 : Math.Sqrt((sumRs / sumRj)) * 255;
                    double RG = sumGj == 0 ? 0 : Math.Sqrt((sumGs / sumGj)) * 255;
                    double RB = sumBj == 0 ? 0 : Math.Sqrt((sumBs / sumBj)) * 255;


                    RR = RR > 255 ? 255 : RR;
                    RR = RR < 0 ? 0 : RR;
                    RG = RG > 255 ? 255 : RG;
                    RG = RG < 0 ? 0 : RG;
                    RB = RB > 255 ? 255 : RB;
                    RB = RB < 0 ? 0 : RB;

                    var cl = Color.FromArgb((int)RR, (int)RG, (int)RB);
                    workImage.SetPixel(c, r, cl);
                }
            }
        }

        private void ResetImage()
        {
            var image = originalImage.Clone(new Rectangle(0, 0, originalImage.Width, originalImage.Height), originalImage.PixelFormat);
            workImage = new FastImage(image);
        }

        #region Events

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
            sSourceFileName = openFileDialog.FileName;

            if (sSourceFileName == "") return;

            tbBrightness.Value = 0;
            panelSource.BackgroundImage = new Bitmap(sSourceFileName);

            originalImage = new Bitmap(sSourceFileName);
            var image = new Bitmap(sSourceFileName);
            workImage = new FastImage(image);
        }

        private void btGrayscale_Click(object sender, EventArgs e)
        {
            if (workImage == null) return;

            SafeExecute(ApplyGrayscale);
            RefreshImage(workImage?.Image);
        }

        private void btNegative_Click(object sender, EventArgs e)
        {
            if (workImage == null) return;

            SafeExecute(ApplyNegative);
            RefreshImage(workImage?.Image);
        }

        private void tbBrightness_ValueChanged(object sender, EventArgs e)
        {
            if (workImage == null) return;
            lbBrightness.Text = $"Brightness {tbBrightness.Value}";

            //ResetImage();
            SafeExecute(ApplyBrightnessChange);
            RefreshImage(workImage?.Image);
        }

        private void btReset_Click(object sender, EventArgs e)
        {
            if (originalImage == null) return;

            ResetImage();
            RefreshImage(workImage.Image);
        }

        private void tbContrast_ValueChanged(object sender, EventArgs e)
        {
            if (workImage == null) return;
            lbContrast.Text = $"Contrastr {tbContrast.Value}";

            //ResetImage();
            SafeExecute(ApplyContrastChange);
            RefreshImage(workImage?.Image);
        }

        private void btHistoEqGs_Click(object sender, EventArgs e)
        {
            if (workImage == null) return;

            SafeExecute(ApplyHistogramEqualisationGrayscale);
            RefreshImage(workImage?.Image);
        }

        private void btReflexion_Click(object sender, EventArgs e)
        {
            if (workImage == null) return;

            SafeExecute(ApplyReflexion);
            RefreshImage(workImage?.Image);
        }

        private void btLowPassFilter_Click(object sender, EventArgs e)
        {
            if (workImage == null) return;

            SafeExecute(ApplayLowPassFilter);
            RefreshImage(workImage?.Image);
        }

        private void btMedianFilter_Click(object sender, EventArgs e)
        {
            if (workImage == null) return;

            SafeExecute(ApplyMedianFilter);
            RefreshImage(workImage?.Image);
        }
        private void btMarkovFilter_Click(object sender, EventArgs e)
        {
            if (workImage == null) return;

            SafeExecute(ApplyMarkovFilter);
            RefreshImage(workImage?.Image);
        }
        private void btHighPassFilter_Click(object sender, EventArgs e)
        {
            if (workImage == null) return;

            SafeExecute(ApplayHighPassFilter);
            RefreshImage(workImage?.Image);
        }
        private void btUnsharpMask_Click(object sender, EventArgs e)
        {
            if (workImage == null) return;

            SafeExecute(ApplayUnsharpMask);
            RefreshImage(workImage?.Image);
        }
        private void btKirsch_Click(object sender, EventArgs e)
        {
            if (workImage == null) return;

            SafeExecute(ApplyKirsch);
            RefreshImage(workImage?.Image);
        }
        private void btPrewitt_Click(object sender, EventArgs e)
        {
            if (workImage == null) return;

            SafeExecute(ApplyPrewitt);
            RefreshImage(workImage?.Image);
        }
        private void btFreiChen_Click(object sender, EventArgs e)
        {
            if (workImage == null) return;

            SafeExecute(ApplyFreiChen);
            RefreshImage(workImage?.Image);
        }

        #endregion

    }
}
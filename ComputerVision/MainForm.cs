using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

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
            workImage.Lock();
            action();
            workImage.Unlock();
        }

        private void RefreshImage(Bitmap img)
        {
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

            double temp = 1 / Math.Pow((weight + 2), 2);
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

                            sumR = sumR + color.R * h[row - r + 1, col - c + 1];
                            sumG = sumG + color.G * h[row - r + 1, col - c + 1];
                            sumB = sumB + color.B * h[row - r + 1, col - c + 1];
                        }
                    }

                    sumR = sumR / ((weight + 2) * (weight + 2));
                    sumG = sumG / ((weight + 2) * (weight + 2));
                    sumB = sumB / ((weight + 2) * (weight + 2));
                    var cl = Color.FromArgb((int)sumR, (int)sumG, (int)sumB);
                    workImage.SetPixel(c, r, cl);
                }

            }
        }

        private void ApplyMedianFilter()
        {

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
        #endregion


    }
}
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
                transf[i] = (histoC[i] * 255)/(workImage.Width * workImage.Height);
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

        private void ResetImage()
        {
            var image = originalImage.Clone(new Rectangle(0, 0, originalImage.Width, originalImage.Height), originalImage.PixelFormat);
            workImage = new FastImage(image);
        }
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
    }
}
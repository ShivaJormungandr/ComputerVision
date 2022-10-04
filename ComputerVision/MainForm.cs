using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace ComputerVision
{
    public partial class MainForm : Form
    {
        private string sSourceFileName = "";
        private FastImage workImage;
        private Bitmap image = null;

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
            panelDestination.BackgroundImage = image;
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

                    if (cbGrayscale.SelectedItem == "AVG")
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

        private void ApplyBrightness()
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

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
            sSourceFileName = openFileDialog.FileName;

            if (sSourceFileName == "") return;

            panelSource.BackgroundImage = new Bitmap(sSourceFileName);
            image = new Bitmap(sSourceFileName);
            workImage = new FastImage(image);
        }

        private void btGrayscale_Click(object sender, EventArgs e)
        {
            if (workImage == null) return;

            SafeExecute(ApplyGrayscale);
            RefreshImage(workImage.GetBitMap());
        }

        private void btNegative_Click(object sender, EventArgs e)
        {
            if (workImage == null) return;

            SafeExecute(ApplyNegative);
            RefreshImage(workImage.GetBitMap());
        }

        private void tbBrightness_ValueChanged(object sender, EventArgs e)
        {
            if (workImage == null) return;
            lbBrightness.Text = $"Brightness {tbBrightness.Value}";

            SafeExecute(ApplyBrightness);
            RefreshImage(workImage.GetBitMap());
        }
    }
}
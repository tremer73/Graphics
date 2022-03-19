using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Collections;
using System.Text;
using Microsoft.Win32;
using System.Collections.Generic;

namespace desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static System.Drawing.Bitmap MainBitmap;
        static Svg.SvgDocument SvgDoc;
        static byte[] Array_bytes;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BMPLoadImage(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            bool? ret = ofd.ShowDialog();
            if (ret.HasValue && ret.Value)
            {
                if (MainBitmap != null)
                {
                    MainBitmap.Dispose();
                }

                MainBitmap = System.Drawing.Image.FromFile(ofd.FileName) as System.Drawing.Bitmap;
                imageViewBmp.Source = MainBitmap.GetImageSource();
            }
        }

        private void BMPLoadText(object sender, RoutedEventArgs e)
        {
            string FileText;
            OpenFileDialog dText = new OpenFileDialog();
            bool? ret = dText.ShowDialog();
            if (ret.HasValue && ret.Value)
            {
                FileText = dText.FileName;
            }
            else
            {
                return;
            }
            try
            {
                Array_bytes =  Encoding.UTF8.GetBytes(File.ReadAllText(FileText));
            }
            catch 
            {
                return;
            }
            
            if (MainBitmap != null)
            {
                System.Drawing.Bitmap clone = MainBitmap.Clone() as System.Drawing.Bitmap;
                System.Drawing.Imaging.BitmapData dataSrc = MainBitmap.LockBits(new System.Drawing.Rectangle(0, 0, MainBitmap.Width, MainBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, MainBitmap.PixelFormat);
                System.Drawing.Imaging.BitmapData dataDst = clone.LockBits(new System.Drawing.Rectangle(0, 0, clone.Width, clone.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, clone.PixelFormat);

                int bytes = Math.Abs(dataSrc.Stride) * dataSrc.Height;
                byte[] rgbValuesSrc = new byte[bytes];
                byte[] rgbValuesDst = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(dataSrc.Scan0, rgbValuesSrc, 0, bytes);
                byte[] mass = new byte[Array_bytes.Length * 8];
                int z = 0;
                for (int i = 0; i < Array_bytes.Length; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        
                        mass[z] = Extensions.GetBit(Array_bytes[i], j);
                        z++;
                    }
                }
                z = 0;
                for (int y = 0; y < dataSrc.Height; ++y)
                {
                    int idx = y * dataSrc.Stride;
                    for (int x = 0; x < dataSrc.Width; ++x, idx += 3)
                    {

                        for (int i = 0; i < 3; ++i)
                        {
                            if (z < mass.Length)
                            {
                                rgbValuesSrc[idx + i] = Extensions.SetBit(rgbValuesSrc[idx + i], 0, mass[z]);
                                z++;
                            }
                            else
                            {
                                rgbValuesSrc[idx + i] = Extensions.SetBit(rgbValuesSrc[idx + i], 0, 0);
                            }
                        }
                    }
                }
                System.Runtime.InteropServices.Marshal.Copy(rgbValuesSrc, 0, dataDst.Scan0, bytes);

                MainBitmap.UnlockBits(dataSrc);
                clone.UnlockBits(dataDst);

                imageViewBmp.Source = clone.GetImageSource();


            }
            else 
            {
                return;
            }

        }
        private void BMPUploadText(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            bool? ret = ofd.ShowDialog();
            if (ret.HasValue && ret.Value)
            {
                if (MainBitmap != null)
                {
                    MainBitmap.Dispose();
                }

                MainBitmap = System.Drawing.Image.FromFile(ofd.FileName) as System.Drawing.Bitmap;
                imageViewBmp.Source = MainBitmap.GetImageSource();
                System.Drawing.Bitmap clone = MainBitmap.Clone() as System.Drawing.Bitmap;
                System.Drawing.Imaging.BitmapData dataSrc = MainBitmap.LockBits(new System.Drawing.Rectangle(0, 0, MainBitmap.Width, MainBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, MainBitmap.PixelFormat);
                System.Drawing.Imaging.BitmapData dataDst = clone.LockBits(new System.Drawing.Rectangle(0, 0, clone.Width, clone.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, clone.PixelFormat);
                int size = (dataSrc.Height * dataSrc.Width * 3);
                System.Collections.BitArray barr = new System.Collections.BitArray(size+1000);
                int bytes = Math.Abs(dataSrc.Stride) * dataSrc.Height;
                byte[] rgbValuesSrc = new byte[bytes];
                byte[] rgbValuesDst = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(dataSrc.Scan0, rgbValuesSrc, 0, bytes);
                int z = 0;
                for (int y = 0; y < dataSrc.Height; ++y)
                {
                    int idx = y * dataSrc.Stride;
                    for (int x = 0; x < dataSrc.Width; ++x, idx += 3)
                    {
                        
                        for (int i = 0; i < 3; ++i)
                        {
                            barr[z] = Convert.ToBoolean(Extensions.GetBit(rgbValuesSrc[idx + i], 0));
                            z++;
                        }
                    }
                }
                string a;
                byte[] LocalArrayByte = new byte[size];
                barr.CopyTo(LocalArrayByte, 0);
                a = Encoding.Default.GetString(LocalArrayByte);
                string FileText;
                OpenFileDialog dText = new OpenFileDialog();
                bool? ret1 = dText.ShowDialog();
                if (ret1.HasValue && ret1.Value)
                {
                    FileText = dText.FileName;
                }
                else
                {
                    return;
                }
                File.WriteAllBytes(FileText, LocalArrayByte);

            }
        }

        private void BMPOnlyRed(object sender, RoutedEventArgs e)
        {
            selectChannel(2);
        }

        private void BMPOnlyGreen(object sender, RoutedEventArgs e)
        {
            selectChannel(1);
        }

        private void BMPOnlyBlue(object sender, RoutedEventArgs e)
        {
            selectChannel(0);
        }

        private void BMPOnlyGrey(object sender, RoutedEventArgs e)
        {
            selectChannelGrey();
        }
        private void BMPOnlyGrey2(object sender, RoutedEventArgs e)
        {
            selectChannelGrey2();
        }

        private void BMPOnlyGreyDifference(object sender, RoutedEventArgs e)
        {
            selectChannelGreyDifference();
        }
        private void BMPOnlyH(object sender, RoutedEventArgs e)
        {
            selectChannelH();
        }

        private void BMPOnlyS(object sender, RoutedEventArgs e)
        {
            selectChannelS();
        }

        private void BMPOnlyV(object sender, RoutedEventArgs e)
        {
            selectChannelV();
        }

        private void selectChannelTextBmp()
        {
            if (MainBitmap != null)
            {
                System.Drawing.Bitmap clone = MainBitmap.Clone() as System.Drawing.Bitmap;
                System.Drawing.Imaging.BitmapData dataSrc = MainBitmap.LockBits(new System.Drawing.Rectangle(0, 0, MainBitmap.Width, MainBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, MainBitmap.PixelFormat);
                System.Drawing.Imaging.BitmapData dataDst = clone.LockBits(new System.Drawing.Rectangle(0, 0, clone.Width, clone.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, clone.PixelFormat);

                int bytes = Math.Abs(dataSrc.Stride) * dataSrc.Height;
                byte[] rgbValuesSrc = new byte[bytes];
                byte[] rgbValuesDst = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(dataSrc.Scan0, rgbValuesSrc, 0, bytes);

                for (int y = 0; y < dataSrc.Height; ++y)
                {
                    int idx = y * dataSrc.Stride;
                    for (int x = 0; x < dataSrc.Width; ++x, idx += 3)
                    {
                        
                        for (int i = 0; i < 3; ++i)
                        {
                            rgbValuesDst[idx + i] = (byte)(0.114 * rgbValuesSrc[idx + 0] + 0.587 * rgbValuesSrc[idx + 1] + 0.299 * rgbValuesSrc[idx + 2]);

                        }
                    }
                }

                System.Runtime.InteropServices.Marshal.Copy(rgbValuesDst, 0, dataDst.Scan0, bytes);

                MainBitmap.UnlockBits(dataSrc);
                clone.UnlockBits(dataDst);

                imageViewBmp.Source = clone.GetImageSource();
            }
        }

        private void selectChannelGrey()
        {
            if (MainBitmap != null)
            {
                System.Drawing.Bitmap clone = MainBitmap.Clone() as System.Drawing.Bitmap;
                System.Drawing.Imaging.BitmapData dataSrc = MainBitmap.LockBits(new System.Drawing.Rectangle(0, 0, MainBitmap.Width, MainBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, MainBitmap.PixelFormat);
                System.Drawing.Imaging.BitmapData dataDst = clone.LockBits(new System.Drawing.Rectangle(0, 0, clone.Width, clone.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, clone.PixelFormat);

                int bytes = Math.Abs(dataSrc.Stride) * dataSrc.Height;
                byte[] rgbValuesSrc = new byte[bytes];
                byte[] rgbValuesDst = new byte[bytes];
                byte[] rgbValuesDst1 = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(dataSrc.Scan0, rgbValuesSrc, 0, bytes);

                for (int y = 0; y < dataSrc.Height; ++y)
                {
                    int idx = y * dataSrc.Stride;
                    for (int x = 0; x < dataSrc.Width; ++x, idx += 3)
                    {
                        //rgbValuesDst[idx + 0] = rgbValuesDst[idx + 1] = (byte)(0.114 * rgbValuesSrc[idx + 0] + 0.587 * rgbValuesSrc[idx + 1] + 0.299 * rgbValuesSrc[idx + 2]);
                        for (int i = 0; i < 3; ++i)
                        {
                            rgbValuesDst[idx + i] = (byte)(0.114 * rgbValuesSrc[idx + 0] + 0.587 * rgbValuesSrc[idx + 1] + 0.299 * rgbValuesSrc[idx + 2]);
                            
                        }
                    }
                }

                System.Runtime.InteropServices.Marshal.Copy(rgbValuesDst, 0, dataDst.Scan0, bytes);

                MainBitmap.UnlockBits(dataSrc);
                clone.UnlockBits(dataDst);

                imageViewBmp.Source = clone.GetImageSource();
            }
        }

        private void selectChannelGrey2()
        {
            if (MainBitmap != null)
            {
                System.Drawing.Bitmap clone = MainBitmap.Clone() as System.Drawing.Bitmap;
                System.Drawing.Imaging.BitmapData dataSrc = MainBitmap.LockBits(new System.Drawing.Rectangle(0, 0, MainBitmap.Width, MainBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, MainBitmap.PixelFormat);
                System.Drawing.Imaging.BitmapData dataDst = clone.LockBits(new System.Drawing.Rectangle(0, 0, clone.Width, clone.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, clone.PixelFormat);

                int bytes = Math.Abs(dataSrc.Stride) * dataSrc.Height;
                byte[] rgbValuesSrc = new byte[bytes];
                byte[] rgbValuesDst = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(dataSrc.Scan0, rgbValuesSrc, 0, bytes);

                for (int y = 0; y < dataSrc.Height; ++y)
                {
                    int idx = y * dataSrc.Stride;
                    for (int x = 0; x < dataSrc.Width; ++x, idx += 3)
                    {
                        //rgbValuesDst[idx + 0] = rgbValuesDst[idx + 1] = (byte)(0.114 * rgbValuesSrc[idx + 0] + 0.587 * rgbValuesSrc[idx + 1] + 0.299 * rgbValuesSrc[idx + 2]);
                        for (int i = 0; i < 3; ++i)
                        {
                            rgbValuesDst[idx + i] = (byte)(0.0722 * rgbValuesSrc[idx + 0] + 0.7152 * rgbValuesSrc[idx + 1] + 0.2126 * rgbValuesSrc[idx + 2]);
                        }
                    }
                }

                System.Runtime.InteropServices.Marshal.Copy(rgbValuesDst, 0, dataDst.Scan0, bytes);

                MainBitmap.UnlockBits(dataSrc);
                clone.UnlockBits(dataDst);

                imageViewBmp.Source = clone.GetImageSource();
            }
        }

        private void selectChannelGreyDifference()
        {
            if (MainBitmap != null)
            {
                System.Drawing.Bitmap clone = MainBitmap.Clone() as System.Drawing.Bitmap;
                System.Drawing.Imaging.BitmapData dataSrc = MainBitmap.LockBits(new System.Drawing.Rectangle(0, 0, MainBitmap.Width, MainBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, MainBitmap.PixelFormat);
                System.Drawing.Imaging.BitmapData dataDst = clone.LockBits(new System.Drawing.Rectangle(0, 0, clone.Width, clone.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, clone.PixelFormat);

                int bytes = Math.Abs(dataSrc.Stride) * dataSrc.Height;
                byte[] rgbValuesSrc = new byte[bytes];
                byte[] rgbValuesDst = new byte[bytes];
                byte[] rgbValuesDst1 = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(dataSrc.Scan0, rgbValuesSrc, 0, bytes);
                double GreatGrey = 0;
                double Grey = 0;
                for (int y = 0; y < dataSrc.Height; ++y)
                {
                    int idx = y * dataSrc.Stride;
                    GreatGrey = 0.0722 * rgbValuesSrc[idx + 0] + 0.7152 * rgbValuesSrc[idx + 1] + 0.2126 * rgbValuesSrc[idx + 2];
                    Grey = 0.114 * rgbValuesSrc[idx + 0] + 0.587 * rgbValuesSrc[idx + 1] + 0.299 * rgbValuesSrc[idx + 2];
                    for (int x = 0; x < dataSrc.Width; ++x, idx += 3)
                    {
                        //rgbValuesDst[idx + 0] = rgbValuesDst[idx + 1] = (byte)(0.114 * rgbValuesSrc[idx + 0] + 0.587 * rgbValuesSrc[idx + 1] + 0.299 * rgbValuesSrc[idx + 2]);
                        for (int i = 0; i < 3; ++i)
                        {
                            rgbValuesDst[idx + i] = (byte)(255- Math.Abs(GreatGrey-Grey));
                        }
                    }
                }

                System.Runtime.InteropServices.Marshal.Copy(rgbValuesDst, 0, dataDst.Scan0, bytes);

                MainBitmap.UnlockBits(dataSrc);
                clone.UnlockBits(dataDst);

                imageViewBmp.Source = clone.GetImageSource();
            }
        }

        private void selectChannel(int id)
        {
            if (MainBitmap != null)
            {
                System.Drawing.Bitmap clone = MainBitmap.Clone() as System.Drawing.Bitmap;
                System.Drawing.Imaging.BitmapData dataSrc = MainBitmap.LockBits(new System.Drawing.Rectangle(0, 0, MainBitmap.Width, MainBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, MainBitmap.PixelFormat);
                System.Drawing.Imaging.BitmapData dataDst = clone.LockBits(new System.Drawing.Rectangle(0, 0, clone.Width, clone.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, clone.PixelFormat);

                int bytes = Math.Abs(dataSrc.Stride) * dataSrc.Height;
                byte[] rgbValuesSrc = new byte[bytes];
                byte[] rgbValuesDst = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(dataSrc.Scan0, rgbValuesSrc, 0, bytes);

                for (int y = 0; y < dataSrc.Height; ++y)
                {
                    int idx = y * dataSrc.Stride;
                    for (int x = 0; x < dataSrc.Width; ++x, idx += 3)
                    {
                        for (int i = 0; i < 3; ++i)
                        {
                            rgbValuesDst[idx + i] = i == id ? rgbValuesSrc[idx + i] : (byte)0;
                        }
                    }
                }

                System.Runtime.InteropServices.Marshal.Copy(rgbValuesDst, 0, dataDst.Scan0, bytes);

                MainBitmap.UnlockBits(dataSrc);
                clone.UnlockBits(dataDst);

                imageViewBmp.Source = clone.GetImageSource();
            }
        }

        private void selectChannelH()
        {
            if (MainBitmap != null)
            {
                System.Drawing.Bitmap clone = MainBitmap.Clone() as System.Drawing.Bitmap;
                System.Drawing.Imaging.BitmapData dataSrc = MainBitmap.LockBits(new System.Drawing.Rectangle(0, 0, MainBitmap.Width, MainBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, MainBitmap.PixelFormat);
                System.Drawing.Imaging.BitmapData dataDst = clone.LockBits(new System.Drawing.Rectangle(0, 0, clone.Width, clone.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, clone.PixelFormat);

                int bytes = Math.Abs(dataSrc.Stride) * dataSrc.Height;
                byte[] rgbValuesSrc = new byte[bytes];
                byte[] rgbValuesDst = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(dataSrc.Scan0, rgbValuesSrc, 0, bytes);
                double R,G,B;
                double min=0, max=0;
                for (int y = 0; y < dataSrc.Height; ++y)
                {
                    int idx = y * dataSrc.Stride;
                    for (int x = 0; x < dataSrc.Width; ++x, idx += 3)
                    {
                        R=rgbValuesSrc[idx + 2];
                        G=rgbValuesSrc[idx + 1];
                        B=rgbValuesSrc[idx + 0];
                        if ((B > G) && (B > R))
                        {
                            max = B;
                            if (G > R)
                            {
                                min = R;
                            }
                            else 
                            {
                                min = G;
                            }
                        }
                        else if ((G > R) && (G > B))
                        {
                            max = G;
                            if (B > R)
                            {
                                min = R;
                            }
                            else
                            {
                                min = B;
                            }
                        }
                        else if ((R > G) && (R > B))
                        {
                            max = R;
                            if (B > G)
                            {
                                min = G;
                            }
                            else
                            {
                                min = B;
                            }
                        }
                        double H=0;
                        if (max==B)
                        {
                            //for (int i = 0; i < 3; ++i)
                            {
                                H = ((R - G / max - min) * 60 + 240);
                                
                            }
                        }
                        else if (max==G)
                        {
                            //for (int i = 0; i < 3; ++i)
                            {
                                H = (byte)((B - R / max - min) * 60 + 120);
                            }
                        }
                        else if ((max==R) && (G<B))
                        {
                            //for (int i = 0; i < 3; ++i)
                            {
                                H = (byte)((G - B / max - min) * 60 + 360);
                            }
                        }
                        else if ((max==R) && (G >= B))
                        {
                            //for (int i = 0; i < 3; ++i)
                            {
                                H = (byte)((G - B / max - min) * 60);
                            }
                        }
                        rgbValuesDst[idx ] = rgbValuesDst[idx + 1] = rgbValuesDst[idx + 2] = (byte)(((H + 360) % 360) * 255.0 / 360.0);
                    }
                }

                System.Runtime.InteropServices.Marshal.Copy(rgbValuesDst, 0, dataDst.Scan0, bytes);

                MainBitmap.UnlockBits(dataSrc);
                clone.UnlockBits(dataDst);

                imageViewBmp.Source = clone.GetImageSource();
            }
        }

        private void selectChannelS()
        {
            if (MainBitmap != null)
            {
                System.Drawing.Bitmap clone = MainBitmap.Clone() as System.Drawing.Bitmap;
                System.Drawing.Imaging.BitmapData dataSrc = MainBitmap.LockBits(new System.Drawing.Rectangle(0, 0, MainBitmap.Width, MainBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, MainBitmap.PixelFormat);
                System.Drawing.Imaging.BitmapData dataDst = clone.LockBits(new System.Drawing.Rectangle(0, 0, clone.Width, clone.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, clone.PixelFormat);

                int bytes = Math.Abs(dataSrc.Stride) * dataSrc.Height;
                byte[] rgbValuesSrc = new byte[bytes];
                byte[] rgbValuesDst = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(dataSrc.Scan0, rgbValuesSrc, 0, bytes);
                double R, G, B;
                double min = 0, max = 0;
                double S=0;
                for (int y = 0; y < dataSrc.Height; ++y)
                {
                    int idx = y * dataSrc.Stride;
                    for (int x = 0; x < dataSrc.Width; ++x, idx += 3)
                    {
                        R = rgbValuesSrc[idx + 2];
                        G = rgbValuesSrc[idx + 1];
                        B = rgbValuesSrc[idx + 0];
                        if ((B > G) && (B > R))
                        {
                            max = B;
                            if (G > R)
                            {
                                min = R;
                            }
                            else
                            {
                                min = G;
                            }
                        }
                        else if ((G > R) && (G > B))
                        {
                            max = G;
                            if (B > R)
                            {
                                min = R;
                            }
                            else
                            {
                                min = B;
                            }
                        }
                        else if ((R > G) && (R > B))
                        {
                            max = R;
                            if (B > G)
                            {
                                min = G;
                            }
                            else
                            {
                                min = B;
                            }
                        }

                        if (max == 0)
                        {
                            S = 0;
                        }
                        else S = 1 - max / min;
                        rgbValuesDst[idx] = rgbValuesDst[idx + 1] = rgbValuesDst[idx + 2] = (byte)(S*255);
                    }
                }

                System.Runtime.InteropServices.Marshal.Copy(rgbValuesDst, 0, dataDst.Scan0, bytes);

                MainBitmap.UnlockBits(dataSrc);
                clone.UnlockBits(dataDst);

                imageViewBmp.Source = clone.GetImageSource();
            }
        }

        private void selectChannelV()
        {
            if (MainBitmap != null)
            {
                System.Drawing.Bitmap clone = MainBitmap.Clone() as System.Drawing.Bitmap;
                System.Drawing.Imaging.BitmapData dataSrc = MainBitmap.LockBits(new System.Drawing.Rectangle(0, 0, MainBitmap.Width, MainBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, MainBitmap.PixelFormat);
                System.Drawing.Imaging.BitmapData dataDst = clone.LockBits(new System.Drawing.Rectangle(0, 0, clone.Width, clone.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, clone.PixelFormat);

                int bytes = Math.Abs(dataSrc.Stride) * dataSrc.Height;
                byte[] rgbValuesSrc = new byte[bytes];
                byte[] rgbValuesDst = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(dataSrc.Scan0, rgbValuesSrc, 0, bytes);
                double R, G, B;
                double max = 0;
                double V = 0;
                for (int y = 0; y < dataSrc.Height; ++y)
                {
                    int idx = y * dataSrc.Stride;
                    for (int x = 0; x < dataSrc.Width; ++x, idx += 3)
                    {
                        R = rgbValuesSrc[idx + 2];
                        G = rgbValuesSrc[idx + 1];
                        B = rgbValuesSrc[idx + 0];
                        if ((B > G) && (B > R))
                        {
                            max = B;
                            
                        }
                        else if ((G > R) && (G > B))
                        {
                            max = G;
                            
                        }
                        else if ((R > G) && (R > B))
                        {
                            max = R;
                        }

                        
                        
                        V = max;
                       
                        
                        rgbValuesDst[idx] = rgbValuesDst[idx + 1] = rgbValuesDst[idx + 2] = (byte)(V * 255);
                    }
                }

                System.Runtime.InteropServices.Marshal.Copy(rgbValuesDst, 0, dataDst.Scan0, bytes);

                MainBitmap.UnlockBits(dataSrc);
                clone.UnlockBits(dataDst);

                imageViewBmp.Source = clone.GetImageSource();
            }
        }

        private void BMPClear10(object sender, RoutedEventArgs e)
        {
            if (MainBitmap != null)
            {
                System.Drawing.Bitmap clone = MainBitmap.Clone() as System.Drawing.Bitmap;
                System.Drawing.Imaging.BitmapData dataSrc = MainBitmap.LockBits(new System.Drawing.Rectangle(0, 0, MainBitmap.Width, MainBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, MainBitmap.PixelFormat);
                System.Drawing.Imaging.BitmapData dataDst = clone.LockBits(new System.Drawing.Rectangle(0, 0, clone.Width, clone.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, clone.PixelFormat);

                int bytes = Math.Abs(dataSrc.Stride) * dataSrc.Height;
                byte[] rgbValuesSrc = new byte[bytes];
                byte[] rgbValuesDst = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(dataSrc.Scan0, rgbValuesSrc, 0, bytes);

                for (int y = 0; y < dataSrc.Height; ++y)
                {
                    int idx = (dataSrc.Height - y - 1) * dataSrc.Stride;
                    for (int x = 0; x < dataSrc.Width; ++x, idx += 3)
                    {
                        for (int i = 0; i < 3; ++i)
                        {
                            rgbValuesDst[idx + i] = y >= 10 ? rgbValuesSrc[idx + i] : (byte)0;
                        }
                    }
                }

                System.Runtime.InteropServices.Marshal.Copy(rgbValuesDst, 0, dataDst.Scan0, bytes);

                MainBitmap.UnlockBits(dataSrc);
                clone.UnlockBits(dataDst);

                imageViewBmp.Source = clone.GetImageSource();
            }
        }

        private void BMPSave(object sender, RoutedEventArgs e)
        {
            BitmapImage image = imageViewBmp.Source as BitmapImage;
            MemoryStream dst = new MemoryStream();
            MemoryStream src = image.StreamSource as MemoryStream;
            long srcPos = src.Position;
            src.Position = 0;
            src.CopyTo(dst);
            src.Position = srcPos;
            dst.Position = 0;
            System.Drawing.Image bitmap = System.Drawing.Image.FromStream(dst);
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            bool? ret = sfd.ShowDialog();
            if (ret.HasValue && ret.Value)
            {
                bitmap.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
            }
            dst.Dispose();
            bitmap.Dispose();
        }

        private void SVGLoadImage(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            bool? ret = ofd.ShowDialog();
            if (ret.HasValue && ret.Value)
            {
                updateSvg(Svg.SvgDocument.Open(ofd.FileName));
            }
        }

        private void updateSvg(Svg.SvgDocument svgdoc)
        {
            SvgDoc = svgdoc;
            var bitmap = SvgDoc.Draw();
            imageViewSvg.Source = bitmap.GetImageSource();
            MemoryStream ms = new MemoryStream();
            SvgDoc.Write(ms);

            imageContentSvg.Text = File.ReadAllText(SvgDoc.BaseUri.OriginalString);
            ms.Dispose();
        }

        private void SVGLoadImageFromText(object sender, RoutedEventArgs e)
        {
            try
            {
                string tempFile = Path.GetTempFileName();
                File.WriteAllText(tempFile, imageContentSvg.Text, System.Text.Encoding.UTF8);
                updateSvg(Svg.SvgDocument.Open(tempFile));
                File.Delete(tempFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

    }
}

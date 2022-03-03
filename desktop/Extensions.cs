using System.IO;
using System.Windows.Media.Imaging;
using System.Linq;

namespace desktop
{
    public static class Extensions
    {

        public static System.Windows.Media.ImageSource GetImageSource(this System.Drawing.Image bitmap)
        {
            if (bitmap == null)
                return null;
            MemoryStream memStream = new MemoryStream();
            bitmap.Save(memStream, System.Drawing.Imaging.ImageFormat.Png);

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnDemand;
            image.StreamSource = memStream;
            image.EndInit();

            return image;
        }


        public static byte GetBit(this byte value, int index)
        {
            return (byte)((value >> index) & 1);
        }

        public static byte SetBit(this byte value, int index, int bitValue)
        {
            return (byte)((value & ~(1 << index)) | (bitValue << index));
        }

        // string str = Extensions.ShowBits(0x47);
        // 0x47    =>   "01000111"
        // номер бита    76543210
        public static string ShowBits(this byte value)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 7; i >= 0; --i)
                sb.Append(value.GetBit(i));
            return sb.ToString();
        }

        // string str = Extensions.ShowBits(new byte[] { 0x47, 0x48, 0x49 });
        // 0x47, 0x48, 0x49 =>  "01000111 01001000 01001001"
        // номер бита            76543210 76543210 76543210
        // номер байта               0        1       2
        public static string ShowBits(this byte[] value)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.Collections.BitArray barr = new System.Collections.BitArray(value);

            for (int n = 0; n < value.Length * 8; n += 8)
            {
                for (int i = 0; i < 8; ++i)
                    sb.Append(barr.Get(n + 7 - i) ? '1' : '0');
                sb.Append(' ');
            }
            return sb.ToString();
        }



    }
}
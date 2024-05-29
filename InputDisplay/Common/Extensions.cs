using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Color = System.Windows.Media.Color;
using System.Windows.Media.Imaging;
using System.IO;
using System.Diagnostics;

namespace InputDisplay
{
    public static class Extensions
    {

        public static string ToUTF8String(this byte[] buffer)
        {
            var value = Encoding.UTF8.GetString(buffer);
            return value.Remove(value.IndexOf((char)0));
        }

        public static string ToUTF16String(this byte[] buffer)
        {
            var value = Encoding.Unicode.GetString(buffer);
            return value.Remove(value.IndexOf((char)0));
        }

        public static int GetExponentOfPowerOfTwo(this int powerOfTwo)
        {
            BitArray b = new BitArray(new int[] { powerOfTwo });
            for (int i = 0; i < b.Length; i++)
                if (b.Get(i))
                    return i;
            return -1;
        }
        public static T[] Shift<T>(this T[] array, int k)
        {
            for (int times = 0; times < k; times++)
            {
                T tmp = array[array.Length - 1];
                for (int i = array.Length - 1; i > 0; i--)
                {
                    array[i] = array[i - 1];
                }
                array[0] = tmp;
            }

            return array;
        }
        public static T Unpool<T>(this Queue<T> queue) where T : new()
        {
            bool hasItems = queue.Count > 0;
            T item = hasItems ? queue.Dequeue() : new T();
            return item;
        }
        public static List<T> Shift<T>(this List<T> list, int k)
        {
            for (int times = 0; times < k; times++)
            {
                T tmp = list[list.Count - 1];
                for (int i = list.Count - 1; i > 0; i--)
                {
                    list[i] = list[i - 1];
                }
                list[0] = tmp;
            }
            return list;
        }

        public static BitmapSource ColorTint(this BitmapSource sourceBitmap, Color tint)
        {
            var bytesPerPixel = (sourceBitmap.Format.BitsPerPixel + 7) / 8;
            var stride = bytesPerPixel * sourceBitmap.PixelWidth;
            byte[] pixelBuffer = new byte[stride * sourceBitmap.PixelHeight];
            sourceBitmap.CopyPixels(pixelBuffer, stride, 0);


            float blue = 0;
            float green = 0;
            float red = 0;


            for (int k = 0; k + 4 < pixelBuffer.Length; k += 4)
            {
                blue = (pixelBuffer[k] * tint.B) / 255;
                green = (pixelBuffer[k + 1] * tint.G) / 255;
                red = (pixelBuffer[k + 2] * tint.R) / 255;


                if (blue > 1)
                { blue = 1; }


                if (green > 1)
                { green = 1; }


                if (red > 1)
                { red = 1; }


                pixelBuffer[k] = (byte)(blue * 255);
                pixelBuffer[k + 1] = (byte)(green * 255);
                pixelBuffer[k + 2] = (byte)(red * 255);


            }
            return BitmapSource.Create(sourceBitmap.PixelWidth, sourceBitmap.PixelHeight, sourceBitmap.DpiX, sourceBitmap.DpiY,
                sourceBitmap.Format, null, pixelBuffer, stride);
        }

        public static Uri GetPackUri(string relativePath, Type type)
        {
            var assemblyShortName = type.Assembly.ToString().Split(',')[0];
            var packUriString = $"pack://application:,,,/{assemblyShortName};component/{relativePath}";
            return new(packUriString);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

namespace smileDetection
{
    public class ConvMatrix
    {
        public int TopLeft = 0, TopMid = 0, TopRight = 0;
        public int MidLeft = 0, Pixel = 1, MidRight = 0;
        public int BottomLeft = 0, BottomMid = 0, BottomRight = 0;
        public int Factor = 1;
        public int Offset = 0;
        public void SetAll(int nVal)
        {
            TopLeft = TopMid = TopRight = MidLeft = Pixel = MidRight = BottomLeft = BottomMid = BottomRight = nVal;
        }
    }
    class transforms
    {
        public Bitmap median(Bitmap bmp)
        {
            Bitmap bp1;
            bp1 = bmp;

            Array red = Array.CreateInstance(typeof(Int32),9);
            Array blue = Array.CreateInstance(typeof(Int32), 9);
            Array green = Array.CreateInstance(typeof(Int32), 9);
            
            int count;

        
            

            for (int i = 0; i < bmp.Width; i++)
            {

                for (int j = 0; j < bmp.Height; j++)
                {
                    count = 0;

                    if (i + 1 < bmp.Width && j + 1 < bmp.Height)
                    {
                        red.SetValue( bmp.GetPixel(i + 1, j + 1).R,count);
                        blue.SetValue( bmp.GetPixel(i + 1, j + 1).B,count);
                        green.SetValue(bmp.GetPixel(i + 1, j + 1).G,count);
                        count++;
                    }
                    if (i + 1 < bmp.Width && j - 1 >= 0)
                    {
                        red.SetValue(bmp.GetPixel(i + 1, j - 1).R, count);
                        blue.SetValue(bmp.GetPixel(i + 1, j - 1).B, count);
                        green.SetValue(bmp.GetPixel(i + 1, j - 1).G, count);
                        count++;
                    }
                    if (i - 1 >= 0 && j + 1 < bmp.Height)
                    {
                        red.SetValue(bmp.GetPixel(i - 1, j + 1).R, count);
                        blue.SetValue(bmp.GetPixel(i - 1, j + 1).B, count);
                        green.SetValue(bmp.GetPixel(i - 1, j + 1).G, count);
                        count++;
                    }
                    if (i - 1 >= 0 && j - 1 >= 0)
                    {
                        red.SetValue(bmp.GetPixel(i - 1, j - 1).R, count);
                        blue.SetValue(bmp.GetPixel(i - 1, j - 1).B, count);
                        green.SetValue(bmp.GetPixel(i - 1, j - 1).G, count);
                        count++;
                    }


                    if (j + 1 < bmp.Height)
                    {
                        red.SetValue(bmp.GetPixel(i , j + 1).R, count);
                        blue.SetValue(bmp.GetPixel(i , j + 1).B, count);
                        green.SetValue(bmp.GetPixel(i , j + 1).G, count);
                        count++;
                    }
                    if (i + 1 < bmp.Width)
                    {
                        red.SetValue(bmp.GetPixel(i + 1, j ).R, count);
                        blue.SetValue(bmp.GetPixel(i + 1, j ).B, count);
                        green.SetValue(bmp.GetPixel(i + 1, j ).G, count);
                        count++;
                    }
                    if (i - 1 >= 0)
                    {
                        red.SetValue(bmp.GetPixel(i - 1, j ).R, count);
                        blue.SetValue(bmp.GetPixel(i - 1, j ).B, count);
                        green.SetValue(bmp.GetPixel(i - 1, j ).G, count);
                        count++;
                    }
                    if (j - 1 >= 0)
                    {
                        red.SetValue(bmp.GetPixel(i , j - 1).R, count);
                        blue.SetValue(bmp.GetPixel(i , j - 1).B, count);
                        green.SetValue(bmp.GetPixel(i , j - 1).G, count);
                        count++;
                    }


                    red.SetValue(bmp.GetPixel(i , j ).R, count);
                    blue.SetValue(bmp.GetPixel(i , j ).B, count);
                    green.SetValue(bmp.GetPixel(i , j ).G, count);
                    //count++;

                    Array.Sort(red);
                    Array.Sort(green);
                    Array.Sort(blue);
                                       
                    count=count/2;
                    //count = (count + 0) / 2;
                    int red1 = (int)red.GetValue(count);
                    int green1 = (int)green.GetValue(count);
                    int blue1 = (int)blue.GetValue(count);
                    
                    bp1.SetPixel(i, j, Color.FromArgb(red1, green1, blue1));
                    Array.Clear(red,0,9);
                    Array.Clear(blue, 0, 9);
                    Array.Clear(green, 0, 9);
                }
            }

            Random rand = new Random();
            bp1.Save(@"c:\image\median\median_" + rand.Next().ToString() + ".jpg");
            return bp1;
        }
    }
}


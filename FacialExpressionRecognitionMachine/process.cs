using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace smileDetection
{
    public class process
    {
        private Bitmap imagebmp;
        public void setImg(Bitmap bmp)
        {
            imagebmp = (Bitmap)bmp.Clone();
        }
        public Bitmap getImg()
        {
            return (Bitmap)imagebmp.Clone();
        }
        public void grayStretch(byte[,] matrix)
        {
            Bitmap tempbmp = (Bitmap)this.imagebmp.Clone();

            BitmapData data2 = tempbmp.LockBits(new Rectangle(0, 0, tempbmp.Width, tempbmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData data = imagebmp.LockBits(new Rectangle(0, 0, imagebmp.Width, imagebmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            byte[,] sElement = matrix;


            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                byte* tptr = (byte*)data2.Scan0;

                ptr += data.Stride + 3;
                tptr += data.Stride + 3;

                int remain = data.Stride - data.Width * 3;

                for (int i = 1; i < data.Height - 1; i++)
                {
                    for (int j = 1; j < data.Width - 1; j++)
                    {


                        byte* temp = ptr - data.Stride - 3;
                        byte max = 0;

                        for (int k = 0; k < 3; k++)
                        {
                            for (int l = 0; l < 3; l++)
                            {
                                if (max < temp[data.Stride * k + l * 3])
                                {
                                    if (sElement[k, l] != 0)
                                        max = temp[data.Stride * k + l * 3];
                                }
                            }
                        }

                        tptr[0] = tptr[1] = tptr[2] = max;


                        ptr += 3;
                        tptr += 3;
                    }
                    ptr += remain + 6;
                    tptr += remain + 6;
                }
            }

            imagebmp.UnlockBits(data);
            tempbmp.UnlockBits(data2);

            imagebmp = (Bitmap)tempbmp.Clone();
        }
        public void grayDevolution(byte[,] matrix)
        {
            Bitmap tempbmp = (Bitmap)this.imagebmp.Clone();

            BitmapData data2 = tempbmp.LockBits(new Rectangle(0, 0, tempbmp.Width, tempbmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData data = imagebmp.LockBits(new Rectangle(0, 0, imagebmp.Width, imagebmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            byte[,] sElement = matrix;


            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                byte* tptr = (byte*)data2.Scan0;

                ptr += data.Stride + 3;
                tptr += data.Stride + 3;

                int remain = data.Stride - data.Width * 3;

                for (int i = 1; i < data.Height - 1; i++)
                {
                    for (int j = 1; j < data.Width - 1; j++)
                    {


                        byte* temp = ptr - data.Stride - 3;
                        byte min = 255;

                        for (int k = 0; k < 3; k++)
                        {
                            for (int l = 0; l < 3; l++)
                            {
                                if (min > temp[data.Stride * k + l * 3])
                                {
                                    if (sElement[k, l] != 0)
                                        min = temp[data.Stride * k + l * 3];
                                }
                            }
                        }

                        tptr[0] = tptr[1] = tptr[2] = min;


                        ptr += 3;
                        tptr += 3;
                    }
                    ptr += remain + 6;
                    tptr += remain + 6;
                }
            }

            imagebmp.UnlockBits(data);
            tempbmp.UnlockBits(data2);

            imagebmp = (Bitmap)tempbmp.Clone();
        }
    }
}

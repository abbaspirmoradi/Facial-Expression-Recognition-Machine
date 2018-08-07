using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.Data.OleDb;

namespace FacialExpressionRecognitionMachine
{
    /// <summary>
    /// Interaction logic for EmotionDetection.xaml
    /// </summary>
    public partial class EmotionDetection : Window
    {
        int button33enableCount = 4;
        int[] count;
        int[][] big;
        int left_eye_number = 0;
        int right_eye_number = 0;
        int limit = 110;
        int[] AI;
        int skin_similar = 10;
        string _dataBasePath = "";
        int bLength;
        int xp, xq, yp, yq, spq;
        Bitmap Bit;
        int lip_number = 0;
        int button32enableCount = 4;
        double[] b1;
        double[] p1;
        int restEnableCount = 3;


        public EmotionDetection()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        public void pic(Bitmap b)
        {
            InputImage.Image = (System.Drawing.Image)b;
        }

        public void DataBasePath(string path)
        {
            _dataBasePath = path;
        }

        private void Convert2BinaryBtn_Click(object sender, RoutedEventArgs e)
        {
            limit = 110;
            Bitmap sam = new Bitmap(InputImage.Image.Width, InputImage.Image.Height);
            int i, j, h, w;
            w = InputImage.Image.Width;
            h = InputImage.Image.Height;
            Bitmap bsi = new Bitmap(InputImage.Image);

            for (i = 0; i < w; i++)
                for (j = 0; j < h; j++)
                    sam.SetPixel(i, j, bsi.GetPixel(i, j));



            Bitmap b = (Bitmap)black_white(sam);


            for (i = 0; i < b.Width; i++)
                b.SetPixel(i, 0, System.Drawing.Color.Black);

            ConvertedImage.Image = (System.Drawing.Image)b;
            FaceDetectionBtn.IsEnabled = true;
            Convert2BinaryBtn.BorderBrush = System.Windows.Media.Brushes.Green;

        }
        public Bitmap black_white(System.Drawing.Image Im)
        {
            Bitmap b = (Bitmap)Im;
            int A, B, C, c;

            for (int i = 1; i < b.Height; i++)   // loop for the image pixels height
            {
                for (int j = 1; j < b.Width; j++)  // loop for the image pixels width
                {
                    System.Drawing.Color col;
                    col = b.GetPixel(j, i);

                    /*
                    c = Convert.ToInt32(col.R) + Convert.ToInt32(col.G) + Convert.ToInt32(col.B);
                    c /=3 ;
                    if (c > 128)
                        c = 255;
                    else
                        c = 0;
                     */
                    A = Convert.ToInt32(col.R);
                    B = Convert.ToInt32(col.G);
                    C = Convert.ToInt32(col.B);
                    if (A > limit || B > limit || C > limit)
                        //// if ((A > limit && B > limit) || (A > limit && C > limit) || (C > limit && B > limit))
                        c = 255;
                    else
                        c = 0;

                    if (c == 0)
                        b.SetPixel(j, i, System.Drawing.Color.Black);
                    else
                        b.SetPixel(j, i, System.Drawing.Color.GhostWhite);

                }
            }
            return b;
        }

        private void FaceDetectionBtn_Click(object sender, RoutedEventArgs e)
        {
            FaceDetectionBtn.BorderBrush = System.Windows.Media.Brushes.Green;


            ConvertedImage.Image = (System.Drawing.Image)face(ConvertedImage.Image);
            Bitmap store = (Bitmap)InputImage.Image;
            Bitmap b = new Bitmap(xq - xp + 1, yq - yp + 1);
            int i, j;
            if (yq > store.Height)
                yq = store.Height;
            if (xq > store.Width)
                xq = store.Width;
            if (xp < 0)
                xp = 0;
            if (yp < 0)
                yp = 0;
            for (i = xp; i < xq; i++)
                for (j = yp; j < yq; j++)
                {   
                    b.SetPixel(i - (xp), j - (yp), store.GetPixel(i, j));
                }
            ConvertedImage.Image = (System.Drawing.Image)b;
            ComponentDetectionBtn.IsEnabled = true;

            Convert2BinaryBtn.BorderBrush = System.Windows.Media.Brushes.Green;
        }

        public System.Drawing.Image face(System.Drawing.Image Im)
        {
            Bitmap b = (Bitmap)Im;
            int i, j, k, w, w_f = 0;
            w = b.Width / 2;
            for (i = 0; i < b.Height; i++)
            {
                if (b.GetPixel(w, i).R != 0 && b.GetPixel(w, i).R != 0 && b.GetPixel(w, i).R != 0 && w_f == 1)
                {
                    break;
                }
                if (w_f == 0)
                    if (b.GetPixel(w, i).R == 0 && b.GetPixel(w, i).R == 0 && b.GetPixel(w, i).R == 0)
                        w_f = 1;
            }
            /*
                    for (j = 0; j < b.Width; j++)
                        b.SetPixel(j, i, Color.Red);
             */
            int copal_start = i, x_st = 5, x_en = 5, tet;
            int preco = 0, nowco = 0;
            for (i = copal_start + 2; i < copal_start + (b.Height - copal_start) / 4; i++)
            {
                w_f = 0;
                nowco = 0;
                for (j = w; j < b.Width; j++)
                {
                    if ((b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).R == 0))
                        break;
                    nowco++;
                }
                for (k = w - 1; k >= 0; k--)
                {
                    if ((b.GetPixel(k, i).R == 0 && b.GetPixel(k, i).R == 0 && b.GetPixel(k, i).R == 0))
                        break;
                    nowco++;
                }
                /*
                if(preco >= b.Width/3)
                if (preco/2 >= nowco)
                    break;
                */

                if (preco >= 15)
                    if (preco / 2 > nowco)
                    {
                        tet = 0;
                        for (j = x_st + 1; j < x_en; j++)
                        {
                            if ((b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).R == 0))
                                continue;
                            tet++;
                        }

                        ///if ((preco*2) / 3 >= tet)
                        if ((preco * 4) / 5 >= tet)
                            break;
                    }
                if (preco < nowco)
                {
                    preco = nowco;
                    x_st = k;
                    x_en = j;
                    spq = i;
                }
            }
            /// textBox1.Text += x_st;
            ////textBox1.Text += " , " + x_en;

            /*
                    for (i = 0; i < b.Height; i++)
                    {
                        b.SetPixel(x_st,i,Color.Red);
                        b.SetPixel(x_en, i, Color.Red);
                    }
            */
            int t;
            t = x_en - x_st;
            t = Convert.ToInt16(Convert.ToDouble(t) * 1.5);
            /*
             for (j = 0; j < b.Width; j++)
                     b.SetPixel(j,copal_start+t, Color.Red);
         */
            Bitmap B1 = new Bitmap(x_en - x_st + 1, t + 1);


            /*
                    for (i = x_st; i < x_en; i++)
                        for (j = copal_start; j < copal_start+t; j++)
                            B1.SetPixel(i - (x_st ), j - (copal_start), b.GetPixel(i, j));
            */

            xp = x_st;
            xq = x_en;
            yp = copal_start;
            yq = t + copal_start;
            spq = spq - copal_start;
            return (System.Drawing.Image)B1;
        }

        private void ComponentDetectionBtn_Click(object sender, RoutedEventArgs e)
        {
            limit = 128;
            ComponentDetectionBtn.BorderBrush = System.Windows.Media.Brushes.Green;

            Bitmap b = new Bitmap(ConvertedImage.Image);

            Bitmap b1 = new Bitmap(black_white(ConvertedImage.Image));
            ConvertedImage.Image = (System.Drawing.Image)b;
            eyelocal(b1);
            range();
            ComponentDetectionBtn.BorderBrush = System.Windows.Media.Brushes.Green;
               LEyePBtn.IsEnabled = true;
        }
        private void eyelocal(Bitmap b)
        {
            //////Bitmap b = new Bitmap(pictureBox2.Image);
            int w = b.Width;
            int h = b.Height;
            int ys1 = h, ye1 = h - 1, ys2 = h, ye2 = h - 1;
            int i, j, k;
            int mid = 0, max = 0;

            for (i = 0; i < h; i++)
            {
                b.SetPixel(0, i, System.Drawing.Color.White);
                b.SetPixel(1, i, System.Drawing.Color.White);
                b.SetPixel(w - 1, i, System.Drawing.Color.White);
                b.SetPixel(w - 2, i, System.Drawing.Color.White);
            }


            for (i = w / 4; i < w - (w / 4); i++)
            {
                for (j = spq; j < h; j++)
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).B == 0 && b.GetPixel(i, j).G == 0)
                        break;

                if (max < (j - spq))
                {
                    max = j - spq;
                    mid = i;
                }
            }
            int tp1 = mid - 5, tp2 = mid + 5, mp1 = h - 1, mp2 = h - 1;
            for (i = w / 8; i < w - (w / 8); i++)
            {
                for (j = spq; j < h; j++)
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).B == 0 && b.GetPixel(i, j).G == 0)
                        break;
                if (i <= mid)
                {
                    if (j - 1 < ys1)
                        ys1 = j - 1;
                    if (i >= mid / 2)
                        if (mp1 > j)
                        { tp1 = i; mp1 = j; }
                }
                else
                {
                    if (j - 1 < ys2)
                        ys2 = j - 1;
                    if (i <= mid + (w - mid) / 2)
                        if (mp2 > j)
                        { tp2 = i; mp2 = j; }
                }
            }

            int black = 0;

            for (i = mid / 2; i >= mid / 4; i--)
            {
                black = 0;
                for (j = ys1; j <= ys1 + (h - ys1) / 4; j++)
                {
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).G == 0 && b.GetPixel(i, j).B == 0)
                    {

                        if (black != 0 && black != j - 1)
                        {

                            for (k = black; k <= j; k++)
                                b.SetPixel(i, k, System.Drawing.Color.Black);

                        }
                        black = j;
                    }



                }


            }

            for (i = mid + (w - mid) / 4; i <= mid + 3 * (w - mid) / 4; i++)
            {
                black = 0;
                for (j = ys2; j <= ys2 + (h - ys2) / 4; j++)
                {
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).G == 0 && b.GetPixel(i, j).B == 0)
                    {

                        if (black != 0 && black != j - 1)
                        {

                            for (k = black; k <= j; k++)
                                b.SetPixel(i, k, System.Drawing.Color.Black);

                        }
                        black = j;
                    }



                }


            }

            ///// pictureBox2.Image = (Image)b;
            int df = 0;
            for (i = ys1 + 1; i < h; i++)
            {
                df = 0;
                for (j = mid; j >= mid / 8; j--)
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).B == 0 && b.GetPixel(j, i).G == 0)
                    {
                        df = 1;
                        break;
                    }
                if (df == 0)
                {
                    ye1 = i;
                    break;
                }
            }


            for (i = ys2 + 1; i < h; i++)
            {
                df = 0;
                for (j = mid + 1; j <= w - (mid / 8); j++)
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).B == 0 && b.GetPixel(j, i).G == 0)
                    {
                        df = 1;
                        break;
                    }
                if (df == 0)
                {
                    ye2 = i;
                    break;
                }
            }

            if ((ye2 - ys2) >= 3 * (ye1 - ys1) / 2)
            {
                ye2 = ys2 + (ye1 - ys1);
            }
            if ((ye1 - ys1) >= 3 * (ye2 - ys2) / 2)
            {
                ye1 = ys1 + (ye2 - ys2);
            }

            int xe1, xe2;
            xe1 = mid;
            xe2 = mid;
            df = 0;
            for (i = mid - 1; i >= 0; i--)
            {
                for (j = ys1; j < ye1; j++)
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).B == 0 && b.GetPixel(i, j).G == 0)
                    {
                        df = 1;
                        break;
                    }
                if (df == 1)
                    break;
                xe1 = i;
            }

            df = 0;
            for (i = mid + 1; i < w; i++)
            {
                for (j = ys2; j < ye2; j++)
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).B == 0 && b.GetPixel(i, j).G == 0)
                    {
                        df = 1;
                        break;
                    }
                if (df == 1)
                    break;
                xe2 = i;
            }

            int lip_h1 = 0, ac = 0;
            int po1, po2;
            po2 = (((w - xe2) / 2) + xe2);
            po1 = (xe1 / 2);
            int k1;

            for (i = ye1 + 1; i < h - 5; i++)
            {
                int cc = 0;
                for (j = po1; j <= po2; j++)
                {
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).B == 0 && b.GetPixel(j, i).G == 0)
                        cc++;
                    /////  b.SetPixel(j,i,Color.Red);
                }
                k1 = (cc * 100) / (po2 - po1);
                if (k1 / 10 > ac / 10)
                {
                    lip_h1 = i;
                    ac = k1;

                }
                if (ac >= 80)
                    break;
                /// if(ac>=10)
                ////if (ac >= 2 * k1)
                ///   break;
            }





            b = new Bitmap(ConvertedImage.Image);

            Bitmap B1 = new Bitmap(xe1 + 1, (ye1 - ys1) + 3);
            for (i = 0; i <= xe1; i++)
                for (j = ys1; j <= ye1 + 2; j++)
                {
                    B1.SetPixel(i, j - ys1, b.GetPixel(i, j));

                }

            LEyeInputImage.Image = (System.Drawing.Image)B1;

            Bitmap B2 = new Bitmap((w - xe2), (ye2 - ys2) + 3);
            for (i = xe2; i < w; i++)
                for (j = ys2; j <= ye2 + 2; j++)
                {
                    B2.SetPixel(i - xe2, j - ys2, b.GetPixel(i, j));

                }
            REyeInputImage.Image = (System.Drawing.Image)B2;

            int u1, u2, u3;
            u1 = xe1 / 4;
            u2 = w - (w - xe2) / 4;
            u3 = ys1;
            if (u3 > ys2)
                u3 = ys2;

            if (ye1 > ye2)
                u3 += 5 * ye2 / 6;
            else
                u3 += 5 * ye1 / 6;

            if (u3 >= h)
                u3 = h - 1;

            Bitmap B3 = new Bitmap(u2 - u1, h - u3);
            for (i = u1; i < u2; i++)
                for (j = u3; j < h; j++)
                    B3.SetPixel(i - u1, j - u3, b.GetPixel(i, j));
            MouthInputImage.Image = (System.Drawing.Image)B3;


        }

        public void range()
        {
            Bitmap b = new Bitmap(MouthInputImage.Image);

            int[] A = new int[300];
            int w, h;
            w = b.Width;
            h = b.Height;
            int i, j, k;
            for (i = 0; i < w; i++)
                for (j = 0; j < h; j++)
                {
                    k = b.GetPixel(i, j).R;
                    k += b.GetPixel(i, j).G;
                    k += b.GetPixel(i, j).B;
                    k = k / 3;
                    A[k]++;
                }
            int max = 0;

            int limit = (w * h) / 150;
            int min = 256;
            max = 0;

            for (i = 30; i < 256; i++)
                if (A[i] >= limit)
                {
                    if (min > i)
                        min = i;
                    if (max < i)
                        max = i;
                }
            if (max - min < 70)
                skin_similar = 7;
            else
                skin_similar = 10;

            ///// textBox1.Text = max + " " + min;

        }

        public Bitmap left_eye(Bitmap b)
        {

            b = black_white(b);
            int i, j, max, x, h, w, y;
            max = 0;
            x = 0;

            h = b.Height;
            w = b.Width;
            y = h - 1;
            for (i = 2; i < 2 * h / 3; i++)
            {
                for (j = 0; j < w; j++)
                {
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                        break;
                }
                if (j == w)
                    j = w - 1;
                if (max <= j && j != 0)
                {
                    max = j;
                    x = i;
                }
                if (i >= h / 2 && j < w / 3)
                    break;
            }

            for (i = h - 1; i >= h / 2; i--)
            {
                int count = 0;
                for (j = w / 4; j <= 3 * w / 4; j++)
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                        count++;
                if (count * 2 >= w / 2)
                    break;
            }
            if (i != h - 1)
                y = i + 1;

            int le_1 = 0, ri_1 = w - 1, ttt = 0;
            ttt = 0;
            for (j = 0; j < w; j++)
            {
                for (i = 0; i < h; i++)
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                    {
                        ttt = 1;
                        break;
                    }
                if (ttt == 1)
                {
                    le_1 = j;
                    break;
                }
            }
            ttt = 0;
            for (j = w - 1; j >= 0; j--)
            {
                for (i = 0; i < h; i++)
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                    {
                        ttt = 1;
                        break;
                    }
                if (ttt == 1)
                {
                    ri_1 = j;
                    break;
                }
            }

            ttt = 0;
            for (i = x; i < y; i++)
            {
                for (j = w / 4; j < w - w / 4; j++)
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                    {
                        ttt = 1;
                        break;
                    }
                if (ttt == 1)
                {
                    x = i;
                    break;
                }
            }

            ttt = 0;
            for (i = y; i >= x; i--)
            {
                for (j = w / 4; j < w - w / 4; j++)
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                    {
                        ttt = 1;
                        break;
                    }
                if (ttt == 1)
                {
                    y = i;
                    break;
                }
            }


            b = new Bitmap(LEyeInputImage.Image);
            Bitmap BB = new Bitmap(w - le_1, h - x);

            for (i = le_1; i < w; i++)
                for (j = x; j < h; j++)
                    BB.SetPixel(i - le_1, j - x, b.GetPixel(i, j));

            return BB;

        }


        public Bitmap skin_color(Bitmap b)
        {

            Bit = new Bitmap(b);
            int w = Bit.Width;
            int h = Bit.Height;
            int i, j;
            big = new int[w + 5][];
            for (i = 0; i < w; i++)
            {
                big[i] = new int[h + 5];
                for (j = 0; j < h; j++)
                    big[i][j] = 0;
            }


            for (i = 0; i < w; i++)
                if (big[i][0] == 0)
                    thriple(i, 0, w, h);
            for (i = 0; i < w; i++)
                if (big[i][h - 1] == 0)
                    thriple(i, h - 1, w, h);
            for (i = 0; i < h; i++)
                if (big[0][i] == 0)
                    thriple(0, i, w, h);
            for (i = 0; i < h; i++)
                if (big[w - 1][i] == 0)
                    thriple(w - 1, i, w, h);

            //// thriple(w-2,h-2,w,h);
            ///  thriple(0, h - 2, w, h);
            ////  thriple(w - 2,0, w, h);
            for (i = 0; i < w; i++)
                for (j = 0; j < h; j++)
                {
                    if (big[i][j] == 1)
                        Bit.SetPixel(i, j, System.Drawing.Color.White);
                    else
                        Bit.SetPixel(i, j, System.Drawing.Color.Black);
                }

            return Bit;

        }

        public int similar(int x, int y, int x1, int y1)
        {
            /*
            int total, total1;
            total = Bit.GetPixel(x, y).R + Bit.GetPixel(x, y).G + Bit.GetPixel(x, y).B;
            total1 = Bit.GetPixel(x1, y1).R + Bit.GetPixel(x1, y1).G + Bit.GetPixel(x1, y1).B;
            total = total / 3;
            total1 = total1 / 3;
            if (Math.Abs(total - total1) <= 8)
                return 1;
             */
            int R, G, B;
            R = Math.Abs(Bit.GetPixel(x, y).R - Bit.GetPixel(x1, y1).R);
            G = Math.Abs(Bit.GetPixel(x, y).G - Bit.GetPixel(x1, y1).G);
            B = Math.Abs(Bit.GetPixel(x, y).B - Bit.GetPixel(x1, y1).B);
            if (R < skin_similar && G < skin_similar && B < skin_similar)
                return 1;
            return 0;
        }

        public void thriple(int x, int y, int w, int h)
        {
            big[x][y] = 1;
            int[] bfs_x = new int[w * h];
            int[] bfs_y = new int[w * h];
            bfs_x[0] = x;
            bfs_y[0] = y;
            int n, m;
            n = 0;
            m = 1;
            while (n != m)
            {
                x = bfs_x[n];
                y = bfs_y[n];

                if (x - 1 >= 0)
                    if (similar(x, y, x - 1, y) == 1 && big[x - 1][y] == 0)
                    {
                        bfs_x[m] = x - 1;
                        bfs_y[m] = y;
                        big[x - 1][y] = 1;
                        m++;
                        /// thriple(x - 1, y, w, h);
                    }
                if (y - 1 >= 0)
                    if (similar(x, y, x, y - 1) == 1 && big[x][y - 1] == 0)
                    {
                        bfs_x[m] = x;
                        bfs_y[m] = y - 1;
                        big[x][y - 1] = 1;
                        m++;
                        ////thriple(x, y - 1, w, h);
                    }
                if (x + 1 < w)
                    if (similar(x, y, x + 1, y) == 1 && big[x + 1][y] == 0)
                    {
                        bfs_x[m] = x + 1;
                        bfs_y[m] = y;
                        big[x + 1][y] = 1;
                        m++;
                        ////// thriple(x + 1, y, w, h);
                    }
                if (y + 1 < h)
                    if (similar(x, y, x, y + 1) == 1 && big[x][y + 1] == 0)
                    {
                        bfs_x[m] = x;
                        bfs_y[m] = y + 1;
                        big[x][y + 1] = 1;
                        m++;
                        ////thriple(x, y + 1, w, h);
                    }


                if (x - 1 >= 0 && y - 1 >= 0)
                    if (similar(x, y, x - 1, y - 1) == 1 && big[x - 1][y - 1] == 0)
                    {
                        bfs_x[m] = x - 1;
                        bfs_y[m] = y - 1;
                        big[x - 1][y - 1] = 1;
                        m++;
                        ////thriple(x - 1, y - 1, w, h);
                    }
                if (x + 1 < w && y - 1 >= 0)
                    if (similar(x, y, x + 1, y - 1) == 1 && big[x + 1][y - 1] == 0)
                    {
                        bfs_x[m] = x + 1;
                        bfs_y[m] = y - 1;
                        big[x + 1][y - 1] = 1;
                        m++;
                        ////thriple(x + 1, y - 1, w, h);
                    }
                if (x + 1 < w && y + 1 < h)
                    if (similar(x, y, x + 1, y + 1) == 1 && big[x + 1][y + 1] == 0)
                    {
                        bfs_x[m] = x + 1;
                        bfs_y[m] = y + 1;
                        big[x + 1][y + 1] = 1;
                        m++;
                        /////thriple(x + 1, y + 1, w, h);
                    }
                if (x - 1 >= 0 && y + 1 < h)
                    if (similar(x, y, x - 1, y + 1) == 1 && big[x - 1][y + 1] == 0)
                    {
                        bfs_x[m] = x - 1;
                        bfs_y[m] = y + 1;
                        big[x - 1][y + 1] = 1;
                        m++;
                        /////thriple(x - 1, y + 1, w, h);
                    }
                n++;

            }
        }

        public Bitmap bezier_eye(Bitmap b)
        {

            int n, m;
            n = b.Height;
            m = b.Width;
            big = new int[m][];


            int i, j = 0, flag = 0;

            for (i = 0; i < m; i++)
            {
                big[i] = new int[n];
                for (j = 0; j < n; j++)
                    big[i][j] = 0;
            }
            int count = 0;
            b1 = new double[1000];
            p1 = new double[1000];
            int x1 = 0, y1 = 0, xn = 0, yn = 0, xm1 = 0, ym1 = 0, xm2 = 0, ym2 = 0, xm3 = 0, ym3 = 0, xm4 = 0, ym4 = 0;
            int yz1 = -1;

            for (i = 0; i < m; i++)
            {
                for (j = n - 1; j >= 0; j--)
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).G == 0 && b.GetPixel(i, j).B == 0)
                    {
                        x1 = i;
                        y1 = j;
                        yz1 = 1;
                        break;
                    }
                if (yz1 != -1)
                {
                    break;
                }
            }

            yz1 = -1;
            for (i = m - 1; i >= 0; i--)
            {
                for (j = n - 1; j >= 0; j--)
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).G == 0 && b.GetPixel(i, j).B == 0)
                    {
                        xn = i;
                        yn = j;
                        yz1 = 1;
                    }
                if (yz1 != -1)
                {
                    break;
                }
            }

            //////////Uper Lip///////////////
            ///////left/////////////////////
            int Q, R, T, start_x, p;
            double pi = 22 / 7;
            start_x = x1 - 2;
            if (start_x < 0)
                start_x = 0;
            p = y1;
            for (Q = 0; Q < 90; Q++)
            {
                flag = 0;
                for (i = start_x; i < m; i++)
                {
                    R = i - start_x;
                    T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
                    R = R + start_x;
                    T = p - T;
                    if (R >= m || T < 0)
                        break;
                    if (b.GetPixel(R, T).R == 0 && b.GetPixel(R, T).G == 0 && b.GetPixel(R, T).B == 0)
                    {
                        flag = 1; break;
                    }
                }
                if (flag == 0)
                    break;
            }
            xm1 = x1 + (xn - x1) / 3;
            R = xm1 - start_x;
            T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
            T = p - T;
            ym1 = T;
            if (ym1 < 0)
                ym1 = 0;
            ////////left//////////////////
            /////////right////////////////
            start_x = xn + 2;
            if (start_x >= m)
                start_x = m - 1;
            p = yn;
            for (Q = 0; Q < 90; Q++)
            {
                flag = 0;
                for (i = start_x; i >= 0; i--)
                {
                    R = start_x - i;
                    T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
                    R = start_x - R;
                    T = p - T;
                    if (R < 0 || T < 0)
                        break;
                    if (b.GetPixel(R, T).R == 0 && b.GetPixel(R, T).G == 0 && b.GetPixel(R, T).B == 0)
                    {
                        flag = 1; break;
                    }
                }
                if (flag == 0)
                    break;
            }
            xm2 = x1 + 2 * (xn - x1) / 3;
            R = start_x - xm2;
            T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
            T = p - T;
            ym2 = T;
            if (ym2 < 0)
                ym2 = 0;
            /////////right////////////////
            //////////Uper Lip///////////////
            //////////Lower Lip///////////////
            ///////left/////////////////////

            start_x = x1 - 2;
            if (start_x < 0)
                start_x = 0;
            p = y1;
            for (Q = 0; Q < 90; Q++)
            {
                flag = 0;
                for (i = start_x; i < m; i++)
                {
                    R = i - start_x;
                    T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
                    R = R + start_x;
                    T = p + T;
                    if (R >= m || T >= n)
                        break;
                    if (b.GetPixel(R, T).R == 0 && b.GetPixel(R, T).G == 0 && b.GetPixel(R, T).B == 0)
                    {
                        flag = 1; break;
                    }
                }
                if (flag == 0)
                    break;
            }
            xm3 = x1 + (xn - x1) / 3;
            R = xm3 - start_x;
            T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
            T = p + T;
            ym3 = T;
            if (ym3 > n)
                ym3 = n - 1;
            ////////left//////////////////
            /////////right////////////////
            start_x = xn + 2;
            if (start_x >= m)
                start_x = m - 1;
            p = yn;
            for (Q = 0; Q < 90; Q++)
            {
                flag = 0;
                for (i = start_x; i >= 0; i--)
                {
                    R = start_x - i;
                    T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
                    R = start_x - R;
                    T = p + T;
                    if (R < 0 || T >= n)
                        break;
                    if (b.GetPixel(R, T).R == 0 && b.GetPixel(R, T).G == 0 && b.GetPixel(R, T).B == 0)
                    {
                        flag = 1; break;
                    }
                }
                if (flag == 0)
                    break;
            }
            xm4 = x1 + 2 * (xn - x1) / 3;
            R = start_x - xm4;
            T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
            T = p + T;
            ym4 = T;
            if (ym4 > n)
                ym4 = n - 1;
            /////////right////////////////  
            //////////Lower Lip///////////////
            for (i = 0; i < n; i++)
                if (b.GetPixel(xm1, i).R == 0 && b.GetPixel(xm1, i).G == 0 && b.GetPixel(xm1, i).B == 0)
                { ym1 = i; break; }
            for (i = 0; i < n; i++)
                if (b.GetPixel(xm2, i).R == 0 && b.GetPixel(xm2, i).G == 0 && b.GetPixel(xm2, i).B == 0)
                { ym2 = i; break; }
            for (i = n - 1; i >= 0; i--)
                if (b.GetPixel(xm3, i).R == 0 && b.GetPixel(xm3, i).G == 0 && b.GetPixel(xm3, i).B == 0)
                { ym3 = i; break; }
            for (i = n - 1; i >= 0; i--)
                if (b.GetPixel(xm4, i).R == 0 && b.GetPixel(xm4, i).G == 0 && b.GetPixel(xm4, i).B == 0)
                { ym4 = i; break; }

            b1[2 * count] = x1;
            b1[2 * count + 1] = y1;
            count++;
            b1[2 * count] = xm1;
            b1[2 * count + 1] = ym1;
            count++;
            b1[2 * count] = xm2;
            b1[2 * count + 1] = ym2;
            count++;
            b1[2 * count] = xn;
            b1[2 * count + 1] = yn;
            count++;

            bLength = count * 2;
            Bezier2D(10);
            int x, y;
            int xs, ys, xe, ye;

            xs = Convert.ToInt16(p1[0]);
            ys = Convert.ToInt16(p1[1]);
            xe = Convert.ToInt16(p1[16]);
            ye = Convert.ToInt16(p1[17]);

            for (i = 0; i < 9; i++)
            {
                x = Convert.ToInt16(p1[i * 2]);
                y = Convert.ToInt16(p1[i * 2 + 1]);
                x1 = Convert.ToInt16(p1[(i + 1) * 2]);
                y1 = Convert.ToInt16(p1[(i + 1) * 2 + 1]);
                slope(x, y, x1, y1);
                if (x < xs)
                { xs = x; ys = y; }
                if (x1 < xs)
                { xs = x1; ys = y1; }
                if (x > xe)
                { xe = x; ye = y; }
                if (x1 > xe)
                { xe = x1; ye = y1; }
            }


            b1[2 * 1 + 1] = ym3;
            b1[2 * 2 + 1] = ym4;
            bLength = count * 2;
            Bezier2D(10);
            int x1s, y1s, x1e, y1e;

            x1s = Convert.ToInt16(p1[0]);
            y1s = Convert.ToInt16(p1[1]);
            x1e = Convert.ToInt16(p1[16]);
            y1e = Convert.ToInt16(p1[17]);

            for (i = 0; i < 9; i++)
            {
                x = Convert.ToInt16(p1[i * 2]);
                y = Convert.ToInt16(p1[i * 2 + 1]);
                x1 = Convert.ToInt16(p1[(i + 1) * 2]);
                y1 = Convert.ToInt16(p1[(i + 1) * 2 + 1]);
                slope(x, y, x1, y1);
                if (x < x1s)
                { x1s = x; y1s = y; }
                if (x1 < x1s)
                { x1s = x1; y1s = y1; }
                if (x > x1e)
                { x1e = x; y1e = y; }
                if (x1 > x1e)
                { x1e = x1; y1e = y1; }
            }


            slope(xs, ys, x1s, y1s);
            slope(xe, ye, x1e, y1e);
            Bitmap bp = new Bitmap(m, n);
            for (i = 0; i < m; i++)
                for (j = 0; j < n; j++)
                {
                    if (big[i][j] == 1)
                        bp.SetPixel(i, j, System.Drawing.Color.Black);
                    else
                        bp.SetPixel(i, j, System.Drawing.Color.White);
                }

            return bp;

        }

        void slope(long x1, long y1, long x2, long y2)
        {
            long x3, y3, p, q;
            double x, y, dx, dy, b, m;

            x = x1;
            y = y1;
            dx = x2 - x1;
            dy = y2 - y1;

            if (dx >= dy && dx != 0.0)
            {
                if (x1 > x2)
                {
                    x3 = x1;
                    x1 = x2;
                    x2 = x3;
                    y3 = y1;
                    y1 = y2;
                    y2 = y3;
                    x = x1;
                    y = y1;
                    dx = x2 - x1;
                    dy = y2 - y1;
                }
                m = dy / dx;
                p = Convert.ToInt16(x);
                q = Convert.ToInt16(y);
                big[p][q] = 1;
                while (x1 < x2)
                {
                    b = y - m * x;
                    x++;
                    y = m * x + b;
                    p = Convert.ToInt16(x);
                    q = Convert.ToInt16(y);
                    big[p][q] = 1;
                    x1++;
                }
            }
            else if (dy != 0.0 && dx != 0.0)
            {
                if (y1 > y2)
                {
                    x3 = x1;
                    x1 = x2;
                    x2 = x3;
                    y3 = y1;
                    y1 = y2;
                    y2 = y3;
                    x = x1;
                    y = y1;
                    dx = x2 - x1;
                    dy = y2 - y1;
                }
                m = dx / dy;
                p = Convert.ToInt16(x);
                q = Convert.ToInt16(y);
                big[p][q] = 1;
                while (y1 < y2)
                {
                    b = y - (x / m);
                    y++;
                    x = m * (y - b);
                    p = Convert.ToInt16(x);
                    q = Convert.ToInt16(y);
                    big[p][q] = 1;
                    y1++;
                }
            }
            else
            {
                if (y1 > y2)
                {
                    x3 = x1;
                    x1 = x2;
                    x2 = x3;
                    y3 = y1;
                    y1 = y2;
                    y2 = y3;
                    x = x1;
                    y = y1;
                    dx = x2 - x1;
                    dy = y2 - y1;
                }
                while (y1 <= y2)
                {
                    p = Convert.ToInt16(x);
                    q = Convert.ToInt16(y);
                    big[p][q] = 1;
                    y++;
                    y1++;
                }
            }

        }

        void Bezier2D(int cpts)
        {
            ////   int npts = (b.Length) / 2;
            int npts = (bLength) / 2;
            int icount, jcount;
            double step, t;

            // Calculate points on curve

            icount = 0;
            t = 0;
            step = (double)1.0 / (cpts - 1);

            for (int i1 = 0; i1 != cpts; i1++)
            {
                if ((1.0 - t) < 5e-6)
                    t = 1.0;

                jcount = 0;
                p1[icount] = 0.0;
                p1[icount + 1] = 0.0;
                for (int i = 0; i != npts; i++)
                {
                    double basis = Bernstein(npts - 1, i, t);
                    p1[icount] += basis * b1[jcount];
                    p1[icount + 1] += basis * b1[jcount + 1];
                    jcount = jcount + 2;
                }

                icount += 2;
                t += step;
            }
            /////	    for (i1 = 0; i1 != cpts; i1++)
            ///////	    printf("Point %ld : %lf %lf\n",i1,p[2*i1],p[2*i1+1]);
        }

        double Bernstein(int n, int i, double t)
        {
            double basis;
            double ti; /* this is t^i */
            double tni; /* this is (1 - t)^i */

            /* handle the special cases to avoid domain problem with pow */

            if (t == 0.0 && i == 0) ti = 1.0; else ti = Math.Pow(t, i);
            if (n == i && t == 1.0) tni = 1.0; else tni = Math.Pow((1 - t), (n - i));
            basis = Ni(n, i) * ti * tni; /* calculate Bernstein basis function */
            return basis;
        }

        double Ni(int n, int i)
        {
            double ni;
            ni = factrl(n) / (factrl(i) * factrl(n - i));
            return ni;
        }

        double factrl(int n)
        {
            int ntop = 6;
            double[] a = new double[100];
            a[0] = 1;
            for (int i = 1; i <= 6; i++)
                a[i] = a[i - 1] * i;
            /////if (n < 0) printf("\nNegative factorial in routine FACTRL\n");
            ////if (n > 32) printf("\nFactorial value too large in routine FACTRL\n");

            /////////while (ntop < n)
            { /* use the precalulated value for n = 0....6 */
                /*	j1 = ntop++;
              a[n]=a[j1]*ntop;
              */
                int j;
                if (ntop < n)
                    for (j = 7; j <= n; j++)
                        a[j] = a[j - 1] * j;
            }
            return a[n]; /* returns the value n! as a floating point number */
        }

        private void LEyePBtn_Click(object sender, RoutedEventArgs e)
        {
            LEyePBtn.BorderBrush = System.Windows.Media.Brushes.Green;

            if (left_eye_number == 0)
            {
                Bitmap b = new Bitmap(LEyeInputImage.Image);
                Bitmap ba = new Bitmap(left_eye(b));
                LEyeInputImage.Image = (System.Drawing.Image)ba;
                left_eye_number++;
            }
            else if (left_eye_number == 1)
            {
                Bitmap b = new Bitmap(LEyeInputImage.Image);
                Bitmap ba = new Bitmap(skin_color(b));
                LEyeConvertedImage.Image = (System.Drawing.Image)ba;
                left_eye_number++;
            }
            else if (left_eye_number == 2)
            {
                Bitmap b = new Bitmap(LEyeConvertedImage.Image);
                Bitmap ba = new Bitmap(big_conect(b));
                LEyeConvertedImage.Image = (System.Drawing.Image)ba;
                b = new Bitmap(LEyeConvertedImage.Image);
                ba = new Bitmap(big_conect(b));
                LEyeConvertedImage.Image = (System.Drawing.Image)ba;
                left_eye_number++;
            }
            else if (left_eye_number == 3)
            {
                Bitmap bs = new Bitmap(LEyeConvertedImage.Image);
                Bitmap bg = bezier_eye(bs);
                LEyeConvertedImage.Image = (System.Drawing.Image)bg;
                left_eye_number++;
            }
            button32enableCount--;
            if (button32enableCount <= 0)
            {
                REyePBtn.IsEnabled = true;
            }
        }

        
        
        public Bitmap big_conect(Bitmap b)
        {

            int start = 1, i, j;
            Bit = new Bitmap(b);

            int hight = Bit.Height;
            int weight = Bit.Width;
            ///hight -= 5;
            ///weight -= 5;

            big = new int[weight + 5][];
            count = new int[1000];

            for (i = 0; i < weight + 5; i++)
            {
                big[i] = new int[hight + 5];
                for (j = 0; j < hight + 5; j++)
                    big[i][j] = 0;
            }
            for (i = 0; i < weight; i++)
                for (j = 0; j < hight; j++)
                    if (big[i][j] == 0 && Bit.GetPixel(i, j).B == 0 && Bit.GetPixel(i, j).R == 0 && Bit.GetPixel(i, j).G == 0)
                    {
                        count[start] = 1;
                        big[i][j] = start;
                        conect(i, j, weight, hight);
                        start++;
                    }

            int max = 0, value = 1;
            for (i = 1; i < start; i++)
                if (count[i] > max)
                {
                    max = count[i];
                    value = i;
                }

            for (i = 0; i < weight; i++)
                for (j = 0; j < hight; j++)
                {
                    if (big[i][j] == value)
                        Bit.SetPixel(i, j, System.Drawing.Color.Black);
                    else
                        Bit.SetPixel(i, j, System.Drawing.Color.White);
                }
            /////label1.Text = "  " + max+"  ";
            /////pictureBox2.Image = (Image)Bit;
            shape();

            return Bit;

        }

        public void shape()
        {
            /////Bit = new Bitmap(pictureBox2.Image);
            int i, j, w, h;
            w = Bit.Width;
            h = Bit.Height;
            int[] a = new int[h];

            for (i = 0; i < h; i++)
            {
                a[i] = 0;
                for (j = 0; j < w; j++)
                    if (Bit.GetPixel(j, i).R == 0 && Bit.GetPixel(j, i).G == 0 && Bit.GetPixel(j, i).B == 0)
                        a[i]++;

            }

            int max = 0, max_i = 0;

            for (i = 0; i < h; i++)
                if (a[i] > max)
                {
                    max = a[i];
                    max_i = i;
                }
            max = 0;
            int t = -1;
            for (i = 0; i < max_i; i++)
            {
                if (max / 2 >= a[i])
                    t = i;
                if (a[i] > max)
                    max = a[i];
            }
            if (t != -1)
            {
                for (i = t; i >= 0; i--)
                    for (j = 0; j < w; j++)
                        Bit.SetPixel(j, i, System.Drawing.Color.White);

            }

            max = 0;
            t = -1;
            for (i = h - 1; i > max_i; i--)
            {
                if (max / 2 >= a[i])
                    t = i;
                if (a[i] > max)
                    max = a[i];
            }
            if (t != -1)
            {
                for (i = t; i < h; i++)
                    for (j = 0; j < w; j++)
                        Bit.SetPixel(j, i, System.Drawing.Color.White);

            }

            //////pictureBox2.Image=(Image)Bit;

        }

        public void conect(int x, int y, int w, int h)
        {
            int[] bfs_x = new int[w * h];
            int[] bfs_y = new int[w * h];
            int n, m;
            bfs_x[0] = x;
            bfs_y[0] = y;
            n = 0;
            m = 1;

            while (n != m)
            {
                x = bfs_x[n];
                y = bfs_y[n];

                if (x - 1 >= 0)
                    if (Bit.GetPixel(x - 1, y).B == 0 && Bit.GetPixel(x - 1, y).R == 0 && Bit.GetPixel(x - 1, y).G == 0 && big[x - 1][y] == 0)
                    {
                        count[big[x][y]]++;
                        big[x - 1][y] = big[x][y];
                        bfs_x[m] = x - 1;
                        bfs_y[m] = y;
                        m++;
                        ////conect(x - 1, y, w, h);
                    }
                if (y - 1 >= 0)
                    if (Bit.GetPixel(x, y - 1).B == 0 && Bit.GetPixel(x, y - 1).R == 0 && Bit.GetPixel(x, y - 1).G == 0 && big[x][y - 1] == 0)
                    {
                        count[big[x][y]]++;
                        big[x][y - 1] = big[x][y];
                        bfs_x[m] = x;
                        bfs_y[m] = y - 1;
                        m++;
                        ////conect(x, y - 1, w, h);
                    }
                if (x + 1 < w)
                    if (Bit.GetPixel(x + 1, y).B == 0 && Bit.GetPixel(x + 1, y).R == 0 && Bit.GetPixel(x + 1, y).G == 0 && big[x + 1][y] == 0)
                    {
                        count[big[x][y]]++;
                        big[x + 1][y] = big[x][y];
                        bfs_x[m] = x + 1;
                        bfs_y[m] = y;
                        m++;
                        ////conect(x + 1, y, w, h);
                    }
                if (y + 1 < h)
                    if (Bit.GetPixel(x, y + 1).B == 0 && Bit.GetPixel(x, y + 1).R == 0 && Bit.GetPixel(x, y + 1).G == 0 && big[x][y + 1] == 0)
                    {
                        count[big[x][y]]++;
                        big[x][y + 1] = big[x][y];
                        bfs_x[m] = x;
                        bfs_y[m] = y + 1;
                        m++;
                        /////conect(x, y + 1, w, h);
                    }


                if (x - 1 >= 0 && y - 1 >= 0)
                    if (Bit.GetPixel(x - 1, y - 1).B == 0 && Bit.GetPixel(x - 1, y - 1).R == 0 && Bit.GetPixel(x - 1, y - 1).G == 0 && big[x - 1][y - 1] == 0)
                    {
                        count[big[x][y]]++;
                        big[x - 1][y - 1] = big[x][y];
                        bfs_x[m] = x - 1;
                        bfs_y[m] = y - 1;
                        m++;
                        /////conect(x - 1, y - 1, w, h);
                    }
                if (x + 1 < w && y - 1 >= 0)
                    if (Bit.GetPixel(x + 1, y - 1).B == 0 && Bit.GetPixel(x + 1, y - 1).R == 0 && Bit.GetPixel(x + 1, y - 1).G == 0 && big[x + 1][y - 1] == 0)
                    {
                        count[big[x][y]]++;
                        big[x + 1][y - 1] = big[x][y];
                        bfs_x[m] = x + 1;
                        bfs_y[m] = y - 1;
                        m++;
                        /////conect(x + 1, y - 1, w, h);
                    }
                if (x + 1 < w && y + 1 < h)
                    if (Bit.GetPixel(x + 1, y + 1).B == 0 && Bit.GetPixel(x + 1, y + 1).R == 0 && Bit.GetPixel(x + 1, y + 1).G == 0 && big[x + 1][y + 1] == 0)
                    {
                        count[big[x][y]]++;
                        big[x + 1][y + 1] = big[x][y];
                        bfs_x[m] = x + 1;
                        bfs_y[m] = y + 1;
                        m++;
                        ///// conect(x + 1, y + 1, w, h);
                    }
                if (x - 1 >= 0 && y + 1 < h)
                    if (Bit.GetPixel(x - 1, y + 1).B == 0 && Bit.GetPixel(x - 1, y + 1).R == 0 && Bit.GetPixel(x - 1, y + 1).G == 0 && big[x - 1][y + 1] == 0)
                    {
                        count[big[x][y]]++;
                        big[x - 1][y + 1] = big[x][y];
                        bfs_x[m] = x - 1;
                        bfs_y[m] = y + 1;
                        m++;
                        ////conect(x - 1, y + 1, w, h);
                    }
                n++;
            }

        }

        public Bitmap right_eye(Bitmap b)
        {

            b = black_white(b);
            int i, j, max, x, h, w, y;
            max = 0;
            x = 0;

            h = b.Height;
            w = b.Width;
            y = h - 1;
            for (i = 2; i < 2 * h / 3; i++)
            {
                for (j = w - 1; j >= 0; j--)
                {
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                        break;
                }
                j = w - j;
                if (j == w)
                    j = w - 1;
                if (max <= j && j != 0)
                {
                    max = j;
                    x = i;
                }
                if (i >= h / 2 && j < w / 3)
                    break;
            }

            for (i = h - 1; i >= h / 2; i--)
            {
                int count = 0;
                for (j = w / 4; j <= 3 * w / 4; j++)
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                        count++;
                if (count * 2 >= w / 2)
                    break;
            }
            if (i != h - 1)
                y = i + 1;

            int le_1 = 0, ri_1 = w - 1, ttt = 0;
            ttt = 0;
            for (j = 0; j < w; j++)
            {
                for (i = 0; i < h; i++)
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                    {
                        ttt = 1;
                        break;
                    }
                if (ttt == 1)
                {
                    le_1 = j;
                    break;
                }
            }
            ttt = 0;
            for (j = w - 1; j >= 0; j--)
            {
                for (i = 0; i < h; i++)
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                    {
                        ttt = 1;
                        break;
                    }
                if (ttt == 1)
                {
                    ri_1 = j;
                    break;
                }
            }


            ttt = 0;
            for (i = x; i < y; i++)
            {
                for (j = w / 4; j < w - w / 4; j++)
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                    {
                        ttt = 1;
                        break;
                    }
                if (ttt == 1)
                {
                    x = i;
                    break;
                }
            }

            ttt = 0;
            for (i = y; i >= x; i--)
            {
                for (j = w / 4; j < w - w / 4; j++)
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                    {
                        ttt = 1;
                        break;
                    }
                if (ttt == 1)
                {
                    y = i;
                    break;
                }
            }




            b = new Bitmap(REyeInputImage.Image);
            if (x >= y)
                x = y - 2;

            le_1 -= 3;
            if (le_1 < 0)
                le_1 = 0;
            x -= 3;
            if (x < 0)
                x = 0;

            le_1 -= 3;
            if (le_1 < 0)
                le_1 = 0;
            x -= 3;
            if (x < 0)
                x = 0;

            Bitmap BB = new Bitmap(w - le_1, h - x);

            for (i = le_1; i < w; i++)
                for (j = x; j < h; j++)
                    BB.SetPixel(i - le_1, j - x, b.GetPixel(i, j));

            return BB;

        }


        private void REyePBtn_Click(object sender, RoutedEventArgs e)
        {
            REyePBtn.BorderBrush = System.Windows.Media.Brushes.Green;

            if (right_eye_number == 0)
            {
                Bitmap b = new Bitmap(REyeInputImage.Image);
                Bitmap ba = new Bitmap(right_eye(b));
                REyeInputImage.Image = (System.Drawing.Image)ba;
                right_eye_number++;
            }
            else if (right_eye_number == 1)
            {
                Bitmap b = new Bitmap(REyeInputImage.Image);
                Bitmap ba = new Bitmap(skin_color(b));
                REyeConvertedImage.Image = (System.Drawing.Image)ba;
                right_eye_number++;
            }
            else if (right_eye_number == 2)
            {
                Bitmap b = new Bitmap(REyeConvertedImage.Image);
                Bitmap ba = new Bitmap(big_conect(b));
                REyeConvertedImage.Image = (System.Drawing.Image)ba;
                b = new Bitmap(REyeConvertedImage.Image);
                ba = new Bitmap(big_conect(b));
                REyeConvertedImage.Image = (System.Drawing.Image)ba;
                right_eye_number++;
            }
            else if (right_eye_number == 3)
            {
                Bitmap bs = new Bitmap(REyeConvertedImage.Image);
                Bitmap bg = bezier_eye(bs);
                REyeConvertedImage.Image = (System.Drawing.Image)bg;
                right_eye_number++;
            }
            button33enableCount--;
            if (button33enableCount <= 0)
            {
                MouthPBtn.IsEnabled = true;
            }
        


        }

        private void MouthPBtn_Click(object sender, RoutedEventArgs e)
      
        {
            MouthPBtn.BorderBrush = System.Windows.Media.Brushes.Green;
            if (lip_number == 0)
            {
                Bitmap b = new Bitmap(MouthInputImage.Image);
                Bitmap ba = new Bitmap(skin_color(b));
                MouthConvertedImage.Image = (System.Drawing.Image)ba;
                lip_number++;
            }
            else if (lip_number == 1)
            {
                Bitmap b = new Bitmap(MouthConvertedImage.Image);
                Bitmap ba = new Bitmap(big_conect(b));
                MouthConvertedImage.Image = (System.Drawing.Image)ba;
                lip_number++;
            }
            else if (lip_number == 2)
            {
                Bitmap b = new Bitmap(MouthConvertedImage.Image);
                Bitmap ba = new Bitmap(bezier(b));
                MouthConvertedImage.Image = (System.Drawing.Image)ba;
                lip_number++;
            }
            restEnableCount--;
            if (restEnableCount <= 0)
            {
                textBox1.IsEnabled = true;
                textBox2.IsEnabled = true;
                comboBox1.IsEnabled = true;
                ExpresionDetectionBtn.IsEnabled = true;
                Save.IsEnabled = true;
            }
        

        }
        public Bitmap bezier(Bitmap b)
        {
            int n, m;
            n = b.Height;
            m = b.Width;
            big = new int[m][];


            int i, j = 0, flag = 0;

            for (i = 0; i < m; i++)
            {
                big[i] = new int[n];
                for (j = 0; j < n; j++)
                    big[i][j] = 0;
            }
            int count = 0;
            b1 = new double[1000];
            p1 = new double[1000];
            int x1 = 0, y1 = 0, xn = 0, yn = 0, xm1 = 0, ym1 = 0, xm2 = 0, ym2 = 0, xm3 = 0, ym3 = 0, xm4 = 0, ym4 = 0;
            int yz1 = -1, yz2 = -1;

            for (i = 0; i < m; i++)
            {
                for (j = 0; j < n; j++)
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).G == 0 && b.GetPixel(i, j).B == 0)
                    {
                        if (yz1 == -1)
                        { yz1 = j; yz2 = j; }
                        else
                            yz2 = j;
                    }
                if (yz1 != -1)
                {
                    x1 = i;
                    y1 = (yz1 + yz2) / 2;
                    break;
                }
            }

            yz1 = -1; yz2 = -1;
            for (i = m - 1; i >= 0; i--)
            {
                for (j = 0; j < n; j++)
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).G == 0 && b.GetPixel(i, j).B == 0)
                    {
                        if (yz1 == -1)
                        { yz1 = j; yz2 = j; }
                        else
                            yz2 = j;
                    }
                if (yz1 != -1)
                {
                    xn = i;
                    yn = (yz1 + yz2) / 2;
                    break;
                }
            }

            //////////Uper Lip///////////////
            ///////left/////////////////////
            int Q, R, T, start_x, p;
            double pi = 22 / 7;
            start_x = x1 - 2;
            if (start_x < 0)
                start_x = 0;
            p = y1;
            for (Q = 0; Q < 90; Q++)
            {
                flag = 0;
                for (i = start_x; i < m; i++)
                {
                    R = i - start_x;
                    T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
                    R = R + start_x;
                    T = p - T;
                    if (R >= m || T < 0)
                        break;
                    if (b.GetPixel(R, T).R == 0 && b.GetPixel(R, T).G == 0 && b.GetPixel(R, T).B == 0)
                    {
                        flag = 1; break;
                    }
                }
                if (flag == 0)
                    break;
            }
            xm1 = x1 + (xn - x1) / 3;
            R = xm1 - start_x;
            T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
            T = p - T;
            ym1 = T;
            if (ym1 < 0)
                ym1 = 0;
            ////////left//////////////////
            /////////right////////////////
            start_x = xn + 2;
            if (start_x >= m)
                start_x = m - 1;
            p = yn;
            for (Q = 0; Q < 90; Q++)
            {
                flag = 0;
                for (i = start_x; i >= 0; i--)
                {
                    R = start_x - i;
                    T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
                    R = start_x - R;
                    T = p - T;
                    if (R < 0 || T < 0)
                        break;
                    if (b.GetPixel(R, T).R == 0 && b.GetPixel(R, T).G == 0 && b.GetPixel(R, T).B == 0)
                    {
                        flag = 1; break;
                    }
                }
                if (flag == 0)
                    break;
            }
            xm2 = x1 + 2 * (xn - x1) / 3;
            R = start_x - xm2;
            T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
            T = p - T;
            ym2 = T;
            if (ym2 < 0)
                ym2 = 0;
            /////////right////////////////
            //////////Uper Lip///////////////
            //////////Lower Lip///////////////
            ///////left/////////////////////

            start_x = x1 - 2;
            if (start_x < 0)
                start_x = 0;
            p = y1;
            for (Q = 0; Q < 90; Q++)
            {
                flag = 0;
                for (i = start_x; i < m; i++)
                {
                    R = i - start_x;
                    T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
                    R = R + start_x;
                    T = p + T;
                    if (R >= m || T >= n)
                        break;
                    if (b.GetPixel(R, T).R == 0 && b.GetPixel(R, T).G == 0 && b.GetPixel(R, T).B == 0)
                    {
                        flag = 1; break;
                    }
                }
                if (flag == 0)
                    break;
            }
            xm3 = x1 + (xn - x1) / 3;
            R = xm3 - start_x;
            T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
            T = p + T;
            ym3 = T;
            if (ym3 > n)
                ym3 = n - 1;
            ////////left//////////////////
            /////////right////////////////
            start_x = xn + 2;
            if (start_x >= m)
                start_x = m - 1;
            p = yn;
            for (Q = 0; Q < 90; Q++)
            {
                flag = 0;
                for (i = start_x; i >= 0; i--)
                {
                    R = start_x - i;
                    T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
                    R = start_x - R;
                    T = p + T;
                    if (R < 0 || T >= n)
                        break;
                    if (b.GetPixel(R, T).R == 0 && b.GetPixel(R, T).G == 0 && b.GetPixel(R, T).B == 0)
                    {
                        flag = 1; break;
                    }
                }
                if (flag == 0)
                    break;
            }
            xm4 = x1 + 2 * (xn - x1) / 3;
            R = start_x - xm4;
            T = Convert.ToInt16(R * Math.Tan((Q * pi) / 180));
            T = p + T;
            ym4 = T;
            if (ym4 > n)
                ym4 = n - 1;
            /////////right////////////////  
            //////////Lower Lip///////////////

            for (i = 0; i < n; i++)
                if (b.GetPixel(xm1, i).R == 0 && b.GetPixel(xm1, i).G == 0 && b.GetPixel(xm1, i).B == 0)
                { ym1 = i; break; }
            for (i = 0; i < n; i++)
                if (b.GetPixel(xm2, i).R == 0 && b.GetPixel(xm2, i).G == 0 && b.GetPixel(xm2, i).B == 0)
                { ym2 = i; break; }
            for (i = n - 1; i >= 0; i--)
                if (b.GetPixel(xm3, i).R == 0 && b.GetPixel(xm3, i).G == 0 && b.GetPixel(xm3, i).B == 0)
                { ym3 = i; break; }
            for (i = n - 1; i >= 0; i--)
                if (b.GetPixel(xm4, i).R == 0 && b.GetPixel(xm4, i).G == 0 && b.GetPixel(xm4, i).B == 0)
                { ym4 = i; break; }

            b1[2 * count] = x1;
            b1[2 * count + 1] = y1;
            count++;
            b1[2 * count] = xm1;
            b1[2 * count + 1] = ym1;
            count++;
            b1[2 * count] = xm2;
            b1[2 * count + 1] = ym2;
            count++;
            b1[2 * count] = xn;
            b1[2 * count + 1] = yn;
            count++;

            bLength = count * 2;
            Bezier2D(10);
            int x, y;
            int xs, ys, xe, ye;

            xs = Convert.ToInt16(p1[0]);
            ys = Convert.ToInt16(p1[1]);
            xe = Convert.ToInt16(p1[16]);
            ye = Convert.ToInt16(p1[17]);

            for (i = 0; i < 9; i++)
            {
                x = Convert.ToInt16(p1[i * 2]);
                y = Convert.ToInt16(p1[i * 2 + 1]);
                x1 = Convert.ToInt16(p1[(i + 1) * 2]);
                y1 = Convert.ToInt16(p1[(i + 1) * 2 + 1]);
                slope(x, y, x1, y1);
                if (x < xs)
                { xs = x; ys = y; }
                if (x1 < xs)
                { xs = x1; ys = y1; }
                if (x > xe)
                { xe = x; ye = y; }
                if (x1 > xe)
                { xe = x1; ye = y1; }
            }


            b1[2 * 1 + 1] = ym3;
            b1[2 * 2 + 1] = ym4;
            bLength = count * 2;
            Bezier2D(10);
            int x1s, y1s, x1e, y1e;

            x1s = Convert.ToInt16(p1[0]);
            y1s = Convert.ToInt16(p1[1]);
            x1e = Convert.ToInt16(p1[16]);
            y1e = Convert.ToInt16(p1[17]);

            for (i = 0; i < 9; i++)
            {
                x = Convert.ToInt16(p1[i * 2]);
                y = Convert.ToInt16(p1[i * 2 + 1]);
                x1 = Convert.ToInt16(p1[(i + 1) * 2]);
                y1 = Convert.ToInt16(p1[(i + 1) * 2 + 1]);
                slope(x, y, x1, y1);
                if (x < x1s)
                { x1s = x; y1s = y; }
                if (x1 < x1s)
                { x1s = x1; y1s = y1; }
                if (x > x1e)
                { x1e = x; y1e = y; }
                if (x1 > x1e)
                { x1e = x1; y1e = y1; }
            }


            slope(xs, ys, x1s, y1s);
            slope(xe, ye, x1e, y1e);

            for (i = 0; i < m; i++)
                for (j = 0; j < n; j++)
                {
                    if (big[i][j] == 1)
                        b.SetPixel(i, j, System.Drawing.Color.Black);
                    else
                        b.SetPixel(i, j, System.Drawing.Color.White);
                }


            return b;


        }

        public void bezier_position(Bitmap b)
        {
            int i, j, w, h;
            int t1 = 0, t2 = 0, x1 = 0, y1 = 0, x2 = 0, y2 = 0, x3 = 0, y3 = 0, x4 = 0, y4 = 0;
            w = b.Width;
            h = b.Height;
            AI[0] = -1;
            AI[1] = -1;
            for (i = 0; i < w; i++)
            {
                for (j = 0; j < h; j++)
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).G == 0 && b.GetPixel(i, j).B == 0)
                    {
                        AI[0] = i;
                        AI[1] = j;
                        break;
                    }
                if (AI[0] != -1)
                    break;
            }

            AI[6] = -1;
            AI[7] = -1;
            for (i = w - 1; i >= 0; i--)
            {
                for (j = 0; j < h; j++)
                    if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).G == 0 && b.GetPixel(i, j).B == 0)
                    {
                        AI[6] = i;
                        AI[7] = j;
                        break;
                    }
                if (AI[6] != -1)
                    break;
            }
            int k;
            k = (AI[6] - AI[0]) / 3;
            AI[2] = AI[0] + k;
            i = AI[2];
            for (j = 0; j < h; j++)
                if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).G == 0 && b.GetPixel(i, j).B == 0)
                {
                    AI[3] = j;
                    break;
                }

            AI[4] = AI[0] + 2 * k;
            i = AI[4];
            for (j = 0; j < h; j++)
                if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).G == 0 && b.GetPixel(i, j).B == 0)
                {
                    AI[5] = j;
                    break;
                }

            AI[8] = AI[0] + k;
            i = AI[8];
            for (j = h - 1; j >= 0; j--)
                if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).G == 0 && b.GetPixel(i, j).B == 0)
                {
                    AI[9] = j;
                    break;
                }

            AI[10] = AI[0] + 2 * k;
            i = AI[10];
            for (j = h - 1; j >= 0; j--)
                if (b.GetPixel(i, j).R == 0 && b.GetPixel(i, j).G == 0 && b.GetPixel(i, j).B == 0)
                {
                    AI[11] = j;
                    break;
                }
            int h1, h2, h3, h4, h5;
            double q1, q2, q3, q4, q5, q6;
            int dis = 0;
            q5 = Convert.ToDouble(AI[6] - AI[0]);
            q6 = Convert.ToDouble(AI[7] - AI[1]);
            q5 = q5 * q5 + q6 * q6;
            q5 = Math.Sqrt(q5);
            dis = Convert.ToInt16(q5);

            for (j = 0; j < w; j++)
            {
                t1 = -1; t2 = -1;
                for (i = 0; i < h; i++)
                {
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                    {
                        if (t1 == -1)
                        { t1 = i; t2 = i; }
                        else
                            t2 = i;
                    }
                    else
                    {
                        if (t1 != -1)
                            break;
                    }
                }
                if (t1 != -1)
                    break;
            }
            x3 = j;
            y3 = (t1 + t2) / 2;

            for (j = w - 1; j >= 0; j--)
            {
                t1 = -1; t2 = -1;
                for (i = 0; i < h; i++)
                {
                    if (b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0)
                    {
                        if (t1 == -1)
                        { t1 = i; t2 = i; }
                        else
                            t2 = i;
                    }
                    else
                    {
                        if (t1 != -1)
                            break;
                    }
                }
                if (t1 != -1)
                    break;
            }
            x4 = j;
            y4 = (t1 + t2) / 2;

            t1 = (x4 - x3) / 3 + x3;

            j = t1;

            for (i = 0; i < h; i++)
                if ((b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0) || (b.GetPixel(j + 1, i).R == 0 && b.GetPixel(j + 1, i).G == 0 && b.GetPixel(j + 1, i).B == 0) || (b.GetPixel(j - 1, i).R == 0 && b.GetPixel(j - 1, i).G == 0 && b.GetPixel(j - 1, i).B == 0))
                {
                    break;
                }
            x1 = j;
            y1 = i;

            for (i = h - 1; i >= 0; i--)
                if ((b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0) || (b.GetPixel(j + 1, i).R == 0 && b.GetPixel(j + 1, i).G == 0 && b.GetPixel(j + 1, i).B == 0) || (b.GetPixel(j - 1, i).R == 0 && b.GetPixel(j - 1, i).G == 0 && b.GetPixel(j - 1, i).B == 0))
                {
                    break;
                }
            x2 = j;
            y2 = i;


            h1 = x4 - x3;
            h2 = y2 - y1;
            q1 = (Convert.ToDouble(y4 - y3) / Convert.ToDouble(x4 - x3));
            q2 = q1 * (Convert.ToDouble(x3));
            q3 = Convert.ToDouble(y3) - q2;
            q4 = q1 * (Convert.ToDouble(x1));
            q1 = q4 + q3;
            h3 = Convert.ToInt16(q1);
            AI[13] = ((y2 - h3) * 100) / dis;
            AI[12] = ((h3 - y1) * 100) / dis;

            t1 = 2 * (x4 - x3) / 3 + x3;

            j = t1;

            for (i = 0; i < h; i++)
                if ((b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0) || (b.GetPixel(j + 1, i).R == 0 && b.GetPixel(j + 1, i).G == 0 && b.GetPixel(j + 1, i).B == 0) || (b.GetPixel(j - 1, i).R == 0 && b.GetPixel(j - 1, i).G == 0 && b.GetPixel(j - 1, i).B == 0))
                {
                    break;
                }
            x1 = j;
            y1 = i;

            for (i = h - 1; i >= 0; i--)
                if ((b.GetPixel(j, i).R == 0 && b.GetPixel(j, i).G == 0 && b.GetPixel(j, i).B == 0) || (b.GetPixel(j + 1, i).R == 0 && b.GetPixel(j + 1, i).G == 0 && b.GetPixel(j + 1, i).B == 0) || (b.GetPixel(j - 1, i).R == 0 && b.GetPixel(j - 1, i).G == 0 && b.GetPixel(j - 1, i).B == 0))
                {
                    break;
                }
            x2 = j;
            y2 = i;
            h1 = x4 - x3;
            h2 = y2 - y1;
            q1 = (Convert.ToDouble(y4 - y3) / Convert.ToDouble(x4 - x3));
            q2 = q1 * (Convert.ToDouble(x3));
            q3 = Convert.ToDouble(y3) - q2;
            q4 = q1 * (Convert.ToDouble(x1));
            q1 = q4 + q3;
            h3 = Convert.ToInt16(q1);
            AI[15] = ((y2 - h3) * 100) / dis;
            AI[14] = ((h3 - y1) * 100) / dis;

        }


        private void ExpresionDetectionBtn_Click(object sender, RoutedEventArgs e)
        {
            ExpresionDetectionBtn.BorderBrush = System.Windows.Media.Brushes.Green;

            string person = textBox1.Text;
            int flag_man = 0;
            if (person != "")
            {
                OleDbConnection myConnection = new OleDbConnection();
                OleDbCommand myCommand = new OleDbCommand();
                string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + _dataBasePath;
                string query = "select * from Person where Name = '" + person + "'";
                myConnection.ConnectionString = connectionString;
                myConnection.Open();
                myCommand.Connection = myConnection;
                myCommand.CommandText = query;

                OleDbDataReader reader;
                reader = myCommand.ExecuteReader();

                while (reader.Read())
                {
                    flag_man = 1;
                }
                reader.Close();
            }

            if (person != "" && flag_man == 1)
            {
                int[] emotion = new int[6];

                OleDbConnection myConnection = new OleDbConnection();
                OleDbCommand myCommand = new OleDbCommand();
                string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + _dataBasePath;
                string query = "select * from Person where Name = '" + person + "'";
                myConnection.ConnectionString = connectionString;
                myConnection.Open();
                myCommand.Connection = myConnection;
                myCommand.CommandText = query;

                OleDbDataReader reader;
                reader = myCommand.ExecuteReader();

                while (reader.Read())
                {
                    emotion[0] = Convert.ToInt16(reader[1].ToString());
                    emotion[1] = Convert.ToInt16(reader[2].ToString());
                    emotion[2] = Convert.ToInt16(reader[3].ToString());
                    emotion[3] = Convert.ToInt16(reader[4].ToString());
                }
                reader.Close();

                AI = new int[16];
                Bitmap b = new Bitmap(MouthConvertedImage.Image);
                ////////detation(b);
                int lip_1, lip_2, lip_3, lip_4;

                bezier_position(b);
                lip_1 = AI[12];
                lip_2 = AI[13];
                lip_3 = AI[14];
                lip_4 = AI[15];
                b = new Bitmap(LEyeConvertedImage.Image);
                bezier_position(b);

                int l_e_1, l_e_2, l_e_3, l_e_4;
                l_e_1 = AI[12];
                l_e_2 = AI[13];
                l_e_3 = AI[14];
                l_e_4 = AI[15];

                b = new Bitmap(REyeConvertedImage.Image);
                bezier_position(b);

                int r_e_1, r_e_2, r_e_3, r_e_4;
                r_e_1 = AI[12];
                r_e_2 = AI[13];
                r_e_3 = AI[14];
                r_e_4 = AI[15];

                int emo = 0, min = 200;
                int pt = 0, pt1 = 0, pt2 = 0, pt3 = 0;
                if (emotion[0] != 0)
                {
                    query = "select * from [Position] where Id = " + emotion[0];
                    myCommand.CommandText = query;
                    reader = myCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        pt = Convert.ToInt16(reader[37].ToString());
                        pt1 = Convert.ToInt16(reader[38].ToString());
                        pt2 = Convert.ToInt16(reader[39].ToString());
                        pt3 = Convert.ToInt16(reader[40].ToString());
                    }
                    reader.Close();
                    if (Math.Abs(pt - lip_1) + Math.Abs(pt1 - lip_2) + Math.Abs(pt2 - lip_3) + Math.Abs(pt3 - lip_4) < min)
                    {
                        emo = 0;
                        min = Math.Abs(pt - lip_1) + Math.Abs(pt1 - lip_2) + Math.Abs(pt2 - lip_3) + Math.Abs(pt3 - lip_4);
                    }
                }
                if (emotion[1] != 0)
                {
                    query = "select * from [Position] where Id = " + emotion[1];
                    myCommand.CommandText = query;
                    reader = myCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        pt = Convert.ToInt16(reader[37].ToString());
                        pt1 = Convert.ToInt16(reader[38].ToString());
                        pt2 = Convert.ToInt16(reader[39].ToString());
                        pt3 = Convert.ToInt16(reader[40].ToString());
                    }
                    reader.Close();
                    if (Math.Abs(pt - lip_1) + Math.Abs(pt1 - lip_2) + Math.Abs(pt2 - lip_3) + Math.Abs(pt3 - lip_4) < min)
                    {
                        emo = 1;
                        min = Math.Abs(pt - lip_1) + Math.Abs(pt1 - lip_2) + Math.Abs(pt2 - lip_3) + Math.Abs(pt3 - lip_4);
                    }
                }
                if (emotion[2] != 0)
                {
                    query = "select * from [Position] where Id = " + emotion[2];
                    myCommand.CommandText = query;
                    reader = myCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        pt = Convert.ToInt16(reader[37].ToString());
                        pt1 = Convert.ToInt16(reader[38].ToString());
                        pt2 = Convert.ToInt16(reader[39].ToString());
                        pt3 = Convert.ToInt16(reader[40].ToString());
                    }
                    reader.Close();
                    if (Math.Abs(pt - lip_1) + Math.Abs(pt1 - lip_2) + Math.Abs(pt2 - lip_3) + Math.Abs(pt3 - lip_4) < min)
                    {
                        emo = 2;
                        min = Math.Abs(pt - lip_1) + Math.Abs(pt1 - lip_2) + Math.Abs(pt2 - lip_3) + Math.Abs(pt3 - lip_4);
                    }
                }
                if (emotion[3] != 0)
                {
                    query = "select * from [Position] where Id = " + emotion[3];
                    myCommand.CommandText = query;
                    reader = myCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        pt = Convert.ToInt16(reader[37].ToString());
                        pt1 = Convert.ToInt16(reader[38].ToString());
                        pt2 = Convert.ToInt16(reader[39].ToString());
                        pt3 = Convert.ToInt16(reader[40].ToString());
                    }
                    reader.Close();
                    if (Math.Abs(pt - lip_1) + Math.Abs(pt1 - lip_2) + Math.Abs(pt2 - lip_3) + Math.Abs(pt3 - lip_4) < min)
                    {
                        emo = 3;
                        min = Math.Abs(pt - lip_1) + Math.Abs(pt1 - lip_2) + Math.Abs(pt2 - lip_3) + Math.Abs(pt3 - lip_4);
                    }
                }

                if (emo == 0)
                    MessageBox.Show("لبخند بر لب");
                else if (emo == 1)
                    MessageBox.Show("عادی");
                else if (emo == 2)
                    MessageBox.Show("شگفت زده");
                else if (emo == 3)
                    MessageBox.Show("غمگین");
            }
            else
            {
                OleDbConnection myConnection = new OleDbConnection();
                OleDbCommand myCommand = new OleDbCommand();
                string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + _dataBasePath;
                string query = "select * from [Position]";
                myConnection.ConnectionString = connectionString;
                myConnection.Open();
                myCommand.Connection = myConnection;
                myCommand.CommandText = query;

                OleDbDataReader reader;
                reader = myCommand.ExecuteReader();
                int id = 0;
                AI = new int[16];
                Bitmap b = new Bitmap(MouthConvertedImage.Image);
                ////////detation(b);
                int lip_1, lip_2, lip_3, lip_4;

                bezier_position(b);
                lip_1 = AI[12];
                lip_2 = AI[13];
                lip_3 = AI[14];
                lip_4 = AI[15];
                b = new Bitmap(LEyeConvertedImage.Image);
                bezier_position(b);

                int l_e_1, l_e_2, l_e_3, l_e_4;
                l_e_1 = AI[12];
                l_e_2 = AI[13];
                l_e_3 = AI[14];
                l_e_4 = AI[15];

                b = new Bitmap(REyeConvertedImage.Image);
                bezier_position(b);

                int r_e_1, r_e_2, r_e_3, r_e_4;
                r_e_1 = AI[12];
                r_e_2 = AI[13];
                r_e_3 = AI[14];
                r_e_4 = AI[15];

                int emo = -1, min = 200, id1 = -1;
                int pt = 0, pt1 = 0, pt2 = 0, pt3 = 0;

                while (reader.Read())
                {
                    id = Convert.ToInt16(reader[0].ToString());
                    pt = Convert.ToInt16(reader[37].ToString());
                    pt1 = Convert.ToInt16(reader[38].ToString());
                    pt2 = Convert.ToInt16(reader[39].ToString());
                    pt3 = Convert.ToInt16(reader[40].ToString());
                    if (Math.Abs(pt - lip_1) + Math.Abs(pt1 - lip_2) + Math.Abs(pt2 - lip_3) + Math.Abs(pt3 - lip_4) < min)
                    {
                        id1 = id;
                        min = Math.Abs(pt - lip_1) + Math.Abs(pt1 - lip_2) + Math.Abs(pt2 - lip_3) + Math.Abs(pt3 - lip_4);
                    }

                }

                reader.Close();

                if (id1 == -1)
                    MessageBox.Show("مشکل در پایگاه داده");
                else
                {
                    query = "select * from Person where Smile = " + id1;
                    myCommand.CommandText = query;
                    reader = myCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        emo = 0;
                    }
                    reader.Close();

                    query = "select * from Person where Normal = " + id1;
                    myCommand.CommandText = query;
                    reader = myCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        emo = 1;
                    }
                    reader.Close();

                    query = "select * from Person where Surprise = " + id1;
                    myCommand.CommandText = query;
                    reader = myCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        emo = 2;
                    }
                    reader.Close();

                    query = "select * from Person where Sad = " + id1;
                    myCommand.CommandText = query;
                    reader = myCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        emo = 3;
                    }
                    reader.Close();

                    if (emo == 0)
                        MessageBox.Show("لبخند بر لب");
                    else if (emo == 1)
                        MessageBox.Show("عادی");
                    else if (emo == 2)
                        MessageBox.Show("شگفت زده");
                    else if (emo == 3)
                        MessageBox.Show("غمگین");
                    else
                        MessageBox.Show("ابهام");
                }
            }
        

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Save.BorderBrush = System.Windows.Media.Brushes.Green;
            OleDbConnection myConnection = new OleDbConnection();
            OleDbCommand myCommand = new OleDbCommand();
            string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + _dataBasePath;
            string query = "select * from [Position]";
            myConnection.ConnectionString = connectionString;
            myConnection.Open();
            myCommand.Connection = myConnection;
            myCommand.CommandText = query;

            OleDbDataReader reader;
            reader = myCommand.ExecuteReader();
            int po = 0, i;
            string ppp;
            int posi = comboBox1.SelectedIndex;
            while (reader.Read())
            {
                ppp = reader[0].ToString();
                i = Convert.ToInt16(ppp);
                if (i > po)
                    po = i;
            }
            po++;
            reader.Close();
            query = "select * from Person where Name = '" + textBox2.Text + "'";
            myCommand.CommandText = query;
            reader = myCommand.ExecuteReader();
            int flag_person = -1;
            int smile = 0, normal = 0, surprise = 0, sad = 0;
            while (reader.Read())
            {
                ppp = reader[1].ToString();
                smile = Convert.ToInt16(ppp);
                ppp = reader[2].ToString();
                normal = Convert.ToInt16(ppp);
                ppp = reader[3].ToString();
                surprise = Convert.ToInt16(ppp);
                ppp = reader[4].ToString();
                sad = Convert.ToInt16(ppp);
                flag_person = 1;
            }
            reader.Close();
            if (posi == 0)
                normal = po;
            else if (posi == 1)
                sad = po;
            else if (posi == 2)
                smile = po;
            else if (posi == 3)
                surprise = po;
            if (flag_person == -1)
            {
                query = "insert into Person ( Name , Smile , Normal, Surprise, Sad ) values ('" + textBox2.Text + "',";
                query += smile + "," + normal + "," + surprise + "," + sad + ");";
                myCommand.CommandText = query;
                reader = myCommand.ExecuteReader();
                reader.Close();
            }
            else
            {
                query = "update Person set Smile = " + smile + ", Normal = " + normal + ", Surprise = " + surprise;
                query += ", Sad = " + sad + " where Name ='" + textBox2.Text + "';";
                myCommand.CommandText = query;
                reader = myCommand.ExecuteReader();
                reader.Close();
            }

            /////////////////insert position table/////

            AI = new int[16];
            Bitmap b = new Bitmap(MouthConvertedImage.Image);
            bezier_position(b);



            query = "insert into [Position] values (" + po + ",";
            query += AI[0] + "," + AI[1] + "," + AI[2] + "," + AI[3] + "," + AI[4] + "," + AI[5] + "," + AI[6] + "," + AI[7] + "," + AI[8] + "," + AI[9] + "," + AI[10] + "," + AI[11] + ",";
            int lip_1, lip_2, lip_3, lip_4;
            lip_1 = AI[12];
            lip_2 = AI[13];
            lip_3 = AI[14];
            lip_4 = AI[15];
            b = new Bitmap(LEyeConvertedImage.Image);
            bezier_position(b);

            query += AI[0] + "," + AI[1] + "," + AI[2] + "," + AI[3] + "," + AI[4] + "," + AI[5] + "," + AI[6] + "," + AI[7] + "," + AI[8] + "," + AI[9] + "," + AI[10] + "," + AI[11] + ",";
            int l_e_1, l_e_2, l_e_3, l_e_4;
            l_e_1 = AI[12];
            l_e_2 = AI[13];
            l_e_3 = AI[14];
            l_e_4 = AI[15];

            b = new Bitmap(REyeConvertedImage.Image);
            bezier_position(b);

            query += AI[0] + "," + AI[1] + "," + AI[2] + "," + AI[3] + "," + AI[4] + "," + AI[5] + "," + AI[6] + "," + AI[7] + "," + AI[8] + "," + AI[9] + "," + AI[10] + "," + AI[11] + ",";

            int r_e_1, r_e_2, r_e_3, r_e_4;
            r_e_1 = AI[12];
            r_e_2 = AI[13];
            r_e_3 = AI[14];
            r_e_4 = AI[15];



            query += lip_1 + "," + lip_2 + "," + lip_3 + "," + lip_4 + "," + l_e_1 + "," + l_e_2 + "," + l_e_3 + "," + l_e_4 + "," + r_e_1 + "," + r_e_2 + "," + r_e_3 + "," + r_e_4 + ");";
            myCommand.CommandText = query;
            reader = myCommand.ExecuteReader();
            reader.Close();
            MessageBox.Show("Insert Succesfully!!!!!");


            /////////////////insert position table/////
            /////if (reader.Read())
            /////    MessageBox.Show(reader[0].ToString());
            ////MessageBox.Show(po.ToString());
            ////MessageBox.Show(posi.ToString());
            ///// reader.Close();

        

        }

        private void NosePBtn_Click(object sender, RoutedEventArgs e)
        {
            NosePBtn.BorderBrush = System.Windows.Media.Brushes.Green;


        }

        private void LAbruPBtn_Click(object sender, RoutedEventArgs e)
        {
            LAbruPBtn.BorderBrush = System.Windows.Media.Brushes.Green;

        }

        private void RAbruPBtn_Click(object sender, RoutedEventArgs e)
        {
            RAbruPBtn.BorderBrush = System.Windows.Media.Brushes.Green;

        }

        private void textBox2_MouseEnter(object sender, MouseEventArgs e)
        {
            textBox2.BorderBrush = System.Windows.Media.Brushes.Green;
        }

        private void textBox1_MouseEnter(object sender, MouseEventArgs e)
        {
            textBox1.BorderBrush = System.Windows.Media.Brushes.Green;
        }

        private void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
            comboBox1.BorderBrush = System.Windows.Media.Brushes.Green;
        }


    }
}

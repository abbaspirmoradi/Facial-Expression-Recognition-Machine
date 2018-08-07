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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using smileDetection;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace FacialExpressionRecognitionMachine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon m_notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1 ;
        string _dataBasePath = "";
        string save_file;
        string file_name;
        private process processing;
        Bitmap b;
        string crop_image;
        int bLength;
        int xp, xq, yp, yq, spq;
        Bitmap Bit;
        Bitmap undo_picture;//for undoing image
        bool[][] visited;
        int[][] big;
        int[] count;
        Queue<int> queue_i;// = new Queue<int>(capacity);
        Queue<int> queue_j;// = new Queue<int>(capacity);
        int first_i;
        int first_j;
        int count_region;
        int[] countt;

        public MainWindow()
        {
                        _dataBasePath = Directory.GetCurrentDirectory() + @"\db1.mdb";
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }
        Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();

        private void BrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            openFileDialog1.Title = " انتخاب تصویر";
            openFileDialog1.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
               "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
               "Portable Network Graphic (*.png)|*.png";
            if (openFileDialog1.ShowDialog() ==true)
            {
                file_name = openFileDialog1.FileName;
                InputImage.Image= System.Drawing.Image.FromFile(openFileDialog1.FileName);
               // BitmapImage bp = new BitmapImage(new Uri(openFileDialog1.FileName));
               // SourceImage = bp;
               // InputImage.Source = bp;
                contrast_function();
                EditBtn.IsEnabled = true;
                BrowseBtn.BorderBrush = System.Windows.Media.Brushes.Green;
            }
        }

        public Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                // return bitmap; <-- leads to problems, stream is closed/closing ...
                return new Bitmap(bitmap);
            }
        }

        public BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                ms.Position = 0;
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();

                return bi;
            }
        }


        private void contrast_function()
        {
            double nContrast = 30;
            double pixel = 0, contrast = (100.0 + nContrast) / 100.0;

            contrast *= contrast;

            int red, green, blue;
            Bitmap b = new Bitmap(InputImage.Image);

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = b.LockBits(new System.Drawing.Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - b.Width * 3;

                for (int y = 0; y < b.Height; ++y)
                {
                    for (int x = 0; x < b.Width; ++x)
                    {
                        blue = p[0];
                        green = p[1];
                        red = p[2];

                        pixel = red / 255.0;
                        pixel -= 0.5;
                        pixel *= contrast;
                        pixel += 0.5;
                        pixel *= 255;
                        if (pixel < 0) pixel = 0;
                        if (pixel > 255) pixel = 255;
                        p[2] = (byte)pixel;

                        pixel = green / 255.0;
                        pixel -= 0.5;
                        pixel *= contrast;
                        pixel += 0.5;
                        pixel *= 255;
                        if (pixel < 0) pixel = 0;
                        if (pixel > 255) pixel = 255;
                        p[1] = (byte)pixel;

                        pixel = blue / 255.0;
                        pixel -= 0.5;
                        pixel *= contrast;
                        pixel += 0.5;
                        pixel *= 255;
                        if (pixel < 0) pixel = 0;
                        if (pixel > 255) pixel = 255;
                        p[0] = (byte)pixel;

                        p += 3;
                    }
                    p += nOffset;
                }
            }

            b.UnlockBits(bmData);
            InputImage.Image = (System.Drawing.Image)b;

        }


        int cr_start = 140, cr_end = 170, cb_start = 105, cb_end = 150;

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            //Initalize the context menu strip

            contextMenuStrip1  = new System.Windows.Forms.ContextMenuStrip();
          //  contextMenuStrip1.Font=FontFamily.FamilyNam
            System.Windows.Forms.ToolStripMenuItem mI1 = new System.Windows.Forms.ToolStripMenuItem();
            System.Windows.Forms.ToolStripMenuItem mI2 = new System.Windows.Forms.ToolStripMenuItem();
            System.Windows.Forms.ToolStripMenuItem mI3 = new System.Windows.Forms.ToolStripMenuItem();
            System.Windows.Forms.ToolStripMenuItem mI4 = new System.Windows.Forms.ToolStripMenuItem();
            System.Windows.Forms.ToolStripMenuItem mI5 = new System.Windows.Forms.ToolStripMenuItem();
            System.Windows.Forms.ToolStripMenuItem mI6 = new System.Windows.Forms.ToolStripMenuItem();
            mI1.Text = "تیزی تصویر";
            mI2.Text = "وضوح";
            mI3.Text = "روشنایی";
            mI4.Text = "بازیابی تصویر اولیه";
            mI5.Text = "پیش نمایش";
            mI6.Text = "ذخیره";
            contextMenuStrip1.Items.Add(mI1);
            contextMenuStrip1.Items.Add(mI2);
            contextMenuStrip1.Items.Add(mI3);
            contextMenuStrip1.Items.Add(mI4);
            contextMenuStrip1.Items.Add(mI5);
            contextMenuStrip1.Items.Add(mI6);

            mI1.Click += new EventHandler(sharpenImageToolStripMenuItem_Click); //Add Click Handler
            mI2.Click += new EventHandler(contrastToolStripMenuItem_Click); //Add Click Handler
            mI3.Click += new EventHandler(brightnessToolStripMenuItem_Click); //Add Click Handler
            mI4.Click += new EventHandler(restoreImageToolStripMenuItem_Click); //Add Click Handler
            mI5.Click += new EventHandler(previewToolStripMenuItem_Click); //Add Click Handler
            mI6.Click += new EventHandler(SaveImagetoolStripMenuItem_Click); //Add Click Handler

            //Initalize Notify Icon

            m_notifyIcon = new System.Windows.Forms.NotifyIcon();

            m_notifyIcon.Text = "Application Title";

            //m_notifyIcon.Icon = new System.Drawing.Icon("Icon.ico");

            m_notifyIcon.ContextMenuStrip = contextMenuStrip1 ; //Associate the contextmenustrip with notify icon

            contextMenuStrip1 .Show(830, 400);
            m_notifyIcon.Visible = true;
            SkinColorBtn.IsEnabled = true;
            EditBtn.BorderBrush = System.Windows.Media.Brushes.Green;
        }

        private void previewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (InputImage.Image != null)
                {
                    previewForm newform = new previewForm();
                    newform.image = InputImage.Image;
                    newform.set_image();
                    newform.ShowDialog();
                }
            }
            catch
            {

            }

        }

        private void sharpenImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Bitmap m_Bitmap = (Bitmap)InputImage.Image;
                undo_picture = m_Bitmap;
                Bitmap mm_bit = (Bitmap)m_Bitmap.Clone();
                if (BitmapTransform.Sharpen(mm_bit, 11))
                    this.InputImage.Invalidate();
                InputImage.Image = (Bitmap)mm_bit;
                InputImage.Invalidate();
            }
            catch  (Exception e12)
            {
                System.Windows.Forms.MessageBox.Show(e12.Message);
            }
        }
        private void contrastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                argForm dlg = new argForm();
                dlg.nValue = 0;

                if (System.Windows.Forms.DialogResult.OK == dlg.ShowDialog())
                {
                    Bitmap m_Bitmap = (Bitmap)InputImage.Image;
                    undo_picture = m_Bitmap;
                    Bitmap mm_bit = (Bitmap)m_Bitmap.Clone();
                    if (BitmapTransform.Contrast(mm_bit, (sbyte)dlg.nValue))
                        this.InputImage.Invalidate();

                    InputImage.Image = (Bitmap)mm_bit;
                    InputImage.Invalidate();
                }
            }
            catch
            {

            }
        }
        private void restoreImageToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                InputImage.Image = (System.Drawing.Image)undo_picture.Clone();
                InputImage.Invalidate();
            }
            catch
            {
            }

        }
        private void brightnessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                argForm dlg = new argForm();
                dlg.nValue = 0;

                if (System.Windows.Forms. DialogResult.OK == dlg.ShowDialog())
                {
                    Bitmap m_Bitmap = (Bitmap)InputImage.Image;
                    undo_picture = m_Bitmap;
                    Bitmap mm_bit = (Bitmap)m_Bitmap.Clone();
                    if (BitmapTransform.Brightness(mm_bit, (sbyte)dlg.nValue))
                        this.InputImage.Invalidate();

                    InputImage.Image = (Bitmap)mm_bit;
                    InputImage.Invalidate();

                }
            }
            catch
            {

            }
        }
        private void SaveImagetoolStripMenuItem_Click(object sender, EventArgs e)
        {
            Microsoft.Win32.SaveFileDialog op = new Microsoft.Win32.SaveFileDialog();
            op.Title = "  ذخیره تصویر";
            save_file = openFileDialog1.FileName;
            Bitmap b = new Bitmap(InputImage.Image);
           // b.Save(save_file);
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
               "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
               "Portable Network Graphic (*.png)|*.png";
            if(op.ShowDialog()==true)    
            InputImage.Image.Save(save_file);
        }

        private void SkinColorBtn_Click(object sender, RoutedEventArgs e)
        {
            skin_color_segmentation();
            DetectBtn.IsEnabled = true;
            SkinColorBtn.BorderBrush = System.Windows.Media.Brushes.Green;


        }
        private void skin_color_segmentation()
        {
            Bitmap bm = (Bitmap)InputImage.Image;
            Bitmap bmp = new Bitmap(InputImage.Image.Width, InputImage.Image.Height);

            double tot_pixel = bm.Height * bm.Width;
            tot_pixel /= 100;
            tot_pixel *= 10;
            int min_x = bm.Width + 5;// Convert.ToInt16(tot_pixel);
            int max_x = 0;
            int max_y = 0;
            int min_y = bm.Height + 5;// Convert.ToInt16(tot_pixel);

            System.Drawing.Color color = new System.Drawing.Color();
            double g, r, avg = 0;
            double f_upper, f_lower, w;
            bool R1, R3, R4, s;
            double c, cr, cb;
            R1 = R3 = R4 = s = false;
            cr_start = 140;
            cr_end = 170;
            cb_start = 105;
            cb_end = 150;

            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    color = bm.GetPixel(i, j);
                    r = Convert.ToDouble(color.R) / Convert.ToDouble(color.R + color.G + color.B);
                    g = Convert.ToDouble(color.G) / Convert.ToDouble(color.R + color.G + color.B);

                    f_upper = -1.3767 * r * r + 1.0743 * r + 0.1452;
                    f_lower = -0.776 * r * r + 0.5601 * r + 0.1766;

                    if (g > f_lower && g < f_upper)
                        R1 = true;
                    else
                        R1 = false;

                    w = Math.Pow((r - 0.33), 2) + Math.Pow((g - 0.33), 2);
                    //avg += w;
                    //MessageBox.Show(w.ToString());
                    if (w <= 0.0004)
                    {
                        avg++;
                    }

                    if (color.R > color.G && color.G > color.B)
                        R3 = true;
                    else
                        R3 = false;

                    if ((color.R - color.G) >= 45)
                        R4 = true;
                    else
                        R4 = false;

                    if (R3 && R4)//&& R1 && !R2)//if(R1 && R2)//
                        s = true;
                    else
                        s = false;

                    c = 0.257 * Convert.ToDouble(color.R) + 0.504 * color.G + 0.098 * color.B + 16;
                    cb = 0.148 * Convert.ToDouble(color.R) - 0.291 * Convert.ToDouble(color.G) + 0.439 * Convert.ToDouble(color.B) + 128;
                    cr = 0.439 * Convert.ToDouble(color.R) - 0.368 * Convert.ToDouble(color.G) - 0.071 * Convert.ToDouble(color.B) + 128;


                    if (s)
                    {

                        bmp.SetPixel(i, j, System.Drawing.Color.Black);
                        R1 = R3 = R4 = s = false;
                        #region finding face rectangle
                        /*
                         * finding the minimum co-ordinate and maximum co-ordinate xy
                         * of the image between the Cb and Cr threshold value region
                         */

                        if (i < bm.Width / 2 && i < min_x)
                        {
                            min_x = i;
                        }
                        if ((i >= bm.Width / 2 && i < bm.Width) && i > max_x)
                        {
                            max_x = i;
                        }

                        if (j < bm.Height / 2 && j < min_y)
                        {
                            min_y = j;
                        }
                        if ((j >= bm.Height / 2 && i < bm.Height) && j > max_y)
                        {
                            max_y = j;
                        }
                        #endregion
                    }
                    else
                        bmp.SetPixel(i, j, System.Drawing.Color.White);
                }
            }

            ConvertedImage.Image = (Bitmap)bmp;
            ConvertedImage.Invalidate();
            //MessageBox.Show("End");

        }
        private void BFS(int i, int j)
        {

            visited[i][j] = true;
            queue_i.Enqueue(i);
            queue_j.Enqueue(j);

            int w;
            while (queue_i.Count != 0)
            {
                i = queue_i.Dequeue(); //deque
                j = queue_j.Dequeue(); // deque

                big[i][j] = count_region;//assaigning tag for connected region
                countt[count_region]++;
                w = find_first_neighbour(i, j); //find first neighbour...if no neighbour return -1

                while (w != -1) //visit all 8 neighbours
                {
                    if (!visited[first_i][first_j])//unvisited nodes
                    {
                        visited[first_i][first_j] = true;

                        queue_i.Enqueue(first_i);//enque
                        queue_j.Enqueue(first_j);//enque

                    }

                    w = find_first_neighbour(i, j);// again find first neighbour...if no neighbour return -1
                }

            }

        }

        private int find_first_neighbour(int x, int y)
        {
            int w = Bit.Width;
            int h = Bit.Height;

            if (x - 1 >= 0)
                if (!visited[x - 1][y] && Bit.GetPixel(x - 1, y).B == 0 && Bit.GetPixel(x - 1, y).R == 0 && Bit.GetPixel(x - 1, y).G == 0 && big[x - 1][y] == 0)
                {
                    first_i = x - 1;
                    first_j = y;
                    return 1;
                }
            if (y - 1 >= 0)
                if (!visited[x][y - 1] && Bit.GetPixel(x, y - 1).B == 0 && Bit.GetPixel(x, y - 1).R == 0 && Bit.GetPixel(x, y - 1).G == 0 && big[x][y - 1] == 0)
                {
                    first_i = x;
                    first_j = y - 1;
                    return 1;
                }
            if (x + 1 < w)
                if (!visited[x + 1][y] && Bit.GetPixel(x + 1, y).B == 0 && Bit.GetPixel(x + 1, y).R == 0 && Bit.GetPixel(x + 1, y).G == 0 && big[x + 1][y] == 0)
                {
                    first_i = x + 1;
                    first_j = y;
                    return 1;
                }
            if (y + 1 < h)
                if (!visited[x][y + 1] && Bit.GetPixel(x, y + 1).B == 0 && Bit.GetPixel(x, y + 1).R == 0 && Bit.GetPixel(x, y + 1).G == 0 && big[x][y + 1] == 0)
                {
                    first_i = x;
                    first_j = y + 1;
                    return 1;
                }


            if (x - 1 >= 0 && y - 1 >= 0)
                if (!visited[x - 1][y - 1] && Bit.GetPixel(x - 1, y - 1).B == 0 && Bit.GetPixel(x - 1, y - 1).R == 0 && Bit.GetPixel(x - 1, y - 1).G == 0 && big[x - 1][y - 1] == 0)
                {
                    first_i = x - 1;
                    first_j = y - 1;
                    return 1;
                }
            if (x + 1 < w && y - 1 >= 0)
                if (!visited[x + 1][y - 1] && Bit.GetPixel(x + 1, y - 1).B == 0 && Bit.GetPixel(x + 1, y - 1).R == 0 && Bit.GetPixel(x + 1, y - 1).G == 0 && big[x + 1][y - 1] == 0)
                {
                    first_i = x + 1;
                    first_j = y - 1;
                    return 1;
                }
            if (x + 1 < w && y + 1 < h)
                if (!visited[x + 1][y + 1] && Bit.GetPixel(x + 1, y + 1).B == 0 && Bit.GetPixel(x + 1, y + 1).R == 0 && Bit.GetPixel(x + 1, y + 1).G == 0 && big[x + 1][y + 1] == 0)
                {
                    first_i = x + 1;
                    first_j = y + 1;
                    return 1;
                }
            if (x - 1 >= 0 && y + 1 < h)
                if (!visited[x - 1][y + 1] && Bit.GetPixel(x - 1, y + 1).B == 0 && Bit.GetPixel(x - 1, y + 1).R == 0 && Bit.GetPixel(x - 1, y + 1).G == 0 && big[x - 1][y + 1] == 0)
                {
                    first_i = x - 1;
                    first_j = y + 1;
                    return 1;
                }

            return -1;

        }

        private void DetectBtn_Click(object sender, RoutedEventArgs e)
        {
            connected_area();
            NextBtn.IsEnabled = true;
            DetectBtn.BorderBrush = System.Windows.Media.Brushes.Green;
        }
        private void connected_area()
        {


            Bit = (Bitmap)ConvertedImage.Image.Clone();
            int capacity = Bit.Height * Bit.Width;
            queue_i = new Queue<int>(capacity);
            queue_j = new Queue<int>(capacity);

            countt = new int[capacity];
            visited = new bool[Bit.Width + 5][];

            #region initialization of visited boolean array
            for (int i = 0; i < Bit.Width + 5; i++)
            {
                visited[i] = new bool[Bit.Height + 5];
                for (int j = 0; j < Bit.Height + 5; j++)
                    visited[i][j] = false;
            }
            #endregion
            big = new int[Bit.Width + 5][];

            #region initialization of count region array
            for (int i = 0; i < Bit.Width + 5; i++)
            {
                big[i] = new int[Bit.Height + 5];
                for (int j = 0; j < Bit.Height + 5; j++)
                    big[i][j] = 0;
            }
            #endregion

            int max = 0, max_bit = 0;
            count_region = 1;
            for (int i = 0; i < Bit.Width; i++)
            {
                for (int j = 0; j < Bit.Height; j++)
                {
                    if (!visited[i][j] && (Bit.GetPixel(i, j).R == 0 && Bit.GetPixel(i, j).G == 0 && Bit.GetPixel(i, j).B == 0))
                    {
                        countt[count_region] = 0;
                        //MessageBox.Show(i+" "+j+" ");
                        BFS(i, j);
                        if (max < countt[count_region])
                        {
                            max = countt[count_region];
                            max_bit = count_region;
                            //MessageBox.Show(max+" ");
                        }
                        count_region++;

                    }
                }
            }


            Bitmap bmp = new Bitmap(ConvertedImage.Image.Width, ConvertedImage.Image.Height);

            int min_x = Bit.Width;
            int max_x = 0;
            int max_y = 0;
            int min_y = Bit.Height;

            //MessageBox.Show(max_bit.ToString());
            for (int i = 0; i < Bit.Width; i++)
            {
                for (int j = 0; j < Bit.Height; j++)
                {
                    if (big[i][j] == max_bit)
                    {
                        bmp.SetPixel(i, j, Bit.GetPixel(i, j));

                        #region calculating max min x and y of shorted image frame
                        if (min_x >= i)
                            min_x = i;
                        if (max_x < i)
                            max_x = i;
                        if (min_y >= j)
                            min_y = j;
                        if (max_y < j)
                            max_y = j;
                        #endregion
                    }
                    else
                        bmp.SetPixel(i, j, System.Drawing.Color.White);

                }
            }

            //pictureBox2.Image = (Image)bmp;
            //MessageBox.Show("d");
            //bmp = shape((Bitmap)bmp.Clone());
            //pictureBox2.Image = (Image)bmp;
            //pictureBox2.Invalidate();
            //MessageBox.Show("d");


            int w, h, t;
            double a, p;
            int flagforidentification = 0;

            if (max_x - min_x >= 30 && max_y - min_x >= 30)
            {
                min_x = min_x - 30;
                min_y = min_y - 30;
                max_x = max_x + 30;
                max_y = max_y + 30;
                flagforidentification = 1;
            }

            if (min_x < 0)
                min_x = 0;

            if (min_y < 0)
                min_y = 0;

            if (max_x > Bit.Width)
                max_x = Bit.Width;

            if (max_y > Bit.Height)
                max_y = Bit.Height;
            a = max_x - min_x;
            p = a * 0.12;
            t = Convert.ToInt16(p);
            if (flagforidentification == 1)
            {
                max_x -= t;
                min_x += t;
                min_y += t;
            }
            a = max_x - min_x;
            w = Convert.ToInt16(a);
            h = Convert.ToInt16(w * 1.5);
            // MessageBox.Show(w.ToString());

            if (h + min_y > max_y)
                h = max_y - min_y;


            Bitmap bbbb = new Bitmap(w, h);
            Bitmap pic1 = (Bitmap)InputImage.Image.Clone();
            ConvertedImage.Image = System.Drawing.Image.FromFile(file_name);
            Bitmap fre = (Bitmap)ConvertedImage.Image;
            for (int i = min_x; i < max_x; i++)
            {
                for (int j = min_y; j < min_y + h; j++)
                {
                    //if(bmp.GetPixel(i,j).B==0)
                    bbbb.SetPixel(i - min_x, j - min_y, fre.GetPixel(i, j));
                    ////// bbbb.SetPixel(i - min_x, j - min_y, pic1.GetPixel(i, j));
                    pic1.SetPixel(i, j, System.Drawing.Color.Black);
                    //else
                    //bbbb.SetPixel(i - min_x, j - min_y, Color.White);
                }
            }

            //bbbb = shape((Bitmap)bbbb.Clone());

            ConvertedImage.Image = (Bitmap)bbbb;
            ConvertedImage.Invalidate();
            InputImage.Image = (System.Drawing.Image)pic1;
            queue_i.Clear();
            queue_j.Clear();
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {


            Bitmap b = new Bitmap(ConvertedImage.Image);

            long newHeight = b.Height;
            long newWeight = b.Width;
            double Ratio = (Convert.ToDouble(newHeight) / Convert.ToDouble(newWeight));

            if ((Ratio >= 1.0 && Ratio <= 2.0) && newHeight >= 50 && newWeight >= 50)
            {
                EmotionDetection a = new EmotionDetection();
                a.pic(b);
                a.DataBasePath(_dataBasePath);
                a.Show();
            }
            else
            {
                System.Windows.MessageBox.Show("اين تصوير صورت نيست يا اينکه جهت تشخيص صورت، اندازه آن خيلي کوچک ميباشد.\r\n لطفا مجددا سعي کنيد.");
                if (newHeight < 50 || newWeight < 50)
                {
                    System.Windows.MessageBox.Show("شايد صورتي در تصوير قابل تشخيص نبود.\r\nبنابرين لطفا تصوير ديگري انتخاب فرماييد.");
                    DetectBtn.IsEnabled = false;
                    SkinColorBtn.IsEnabled = false;
                    NextBtn.IsEnabled = false;
                }
            }
            NextBtn.BorderBrush = System.Windows.Media.Brushes.Green;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

       



    }
}

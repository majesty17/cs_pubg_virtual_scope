using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections;

namespace cs_pubg_virtual_scope
{
    public partial class Form_scope : Form
    {
        //放大倍数、刷新频率、是否反转颜色
        public double times;
        public int freq;
        public bool color_reverse = false;
        public bool sharpen = false;

        private int aim_w;
        private int aim_h;
        private int aim_x, aim_y;
        
        //画东西的笔
        Pen pen_cross = new Pen(Color.Gold, 12f);

        public Form_scope()
        {
            InitializeComponent();
        }

        private void Form_scope_Load(object sender, EventArgs e)
        {

            //设置timer
            timer1.Interval = 1000 / freq;
            timer1.Start();

            //计算aim区域大小
            aim_w = (int)(this.Width / times);
            aim_h = (int)(this.Height / times);
            aim_x = Screen.PrimaryScreen.Bounds.Width / 2 - aim_w / 2;
            aim_y = Screen.PrimaryScreen.Bounds.Height / 2 - aim_h / 2;

            //准星
            panel1.Location = new Point(this.Width / 2 - panel1.Width / 2, this.Height / 2 - panel1.Height / 2);
            
        }




        /// <summary>
        /// timer定时程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            //如果窗口是可见的，则抓图
            if (this.Visible)
            {
                Bitmap image = new Bitmap(aim_w,aim_h);
                Graphics imgGraphics = Graphics.FromImage(image);
                if (color_reverse) {
                    imgGraphics.CopyFromScreen(aim_x, aim_y, 0, 0, new Size(aim_w, aim_h),CopyPixelOperation.MergePaint);
                }
                else
                    imgGraphics.CopyFromScreen(aim_x, aim_y, 0, 0, new Size(aim_w, aim_h));
                //实际上是被拉伸显示的

                if (sharpen)
                    this.BackgroundImage = SharpenImage(image);
                else
                    this.BackgroundImage = image;
                
            }
        }

        private void Form_scope_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
        }

        //锐化
        public Bitmap SharpenImage(Bitmap bmp)
        {
            int height = bmp.Height;
            int width = bmp.Width;
            Bitmap newbmp = new Bitmap(width, height);

            LockBitmap lbmp = new LockBitmap(bmp);
            LockBitmap newlbmp = new LockBitmap(newbmp);
            lbmp.LockBits();
            newlbmp.LockBits();

            Color pixel;
            //拉普拉斯模板
            int[] Laplacian = { -1, -1, -1, -1, 9, -1, -1, -1, -1 };
            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    int r = 0, g = 0, b = 0;
                    int Index = 0;
                    for (int col = -1; col <= 1; col++)
                    {
                        for (int row = -1; row <= 1; row++)
                        {
                            pixel = lbmp.GetPixel(x + row, y + col); r += pixel.R * Laplacian[Index];
                            g += pixel.G * Laplacian[Index];
                            b += pixel.B * Laplacian[Index];
                            Index++;
                        }
                    }
                    //处理颜色值溢出
                    r = r > 255 ? 255 : r;
                    r = r < 0 ? 0 : r;
                    g = g > 255 ? 255 : g;
                    g = g < 0 ? 0 : g;
                    b = b > 255 ? 255 : b;
                    b = b < 0 ? 0 : b;
                    newlbmp.SetPixel(x - 1, y - 1, Color.FromArgb(r, g, b));
                }
            }
            lbmp.UnlockBits();
            newlbmp.UnlockBits();
            return newbmp;
        }
    }
}

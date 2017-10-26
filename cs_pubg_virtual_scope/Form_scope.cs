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
            //热键
            Hotkey hotkey;
            hotkey = new Hotkey(this.Handle);
            Hotkey1 = hotkey.RegisterHotkey(System.Windows.Forms.Keys.G, Hotkey.KeyFlags.MOD_SHIFT);   //定义快键(Ctrl + F2)
            hotkey.OnHotkey += new HotkeyEventHandler(OnHotkey);

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

        public delegate void HotkeyEventHandler(int HotKeyID);
        private int Hotkey1;
        public class Hotkey : System.Windows.Forms.IMessageFilter
        {
            Hashtable keyIDs = new Hashtable();
            IntPtr hWnd;

            public event HotkeyEventHandler OnHotkey;

            public enum KeyFlags
            {
                MOD_ALT = 0x1,
                MOD_CONTROL = 0x2,
                MOD_SHIFT = 0x4,
                MOD_WIN = 0x8
            }
            [DllImport("user32.dll")]
            public static extern UInt32 RegisterHotKey(IntPtr hWnd, UInt32 id, UInt32 fsModifiers, UInt32 vk);

            [DllImport("user32.dll")]
            public static extern UInt32 UnregisterHotKey(IntPtr hWnd, UInt32 id);

            [DllImport("kernel32.dll")]
            public static extern UInt32 GlobalAddAtom(String lpString);

            [DllImport("kernel32.dll")]
            public static extern UInt32 GlobalDeleteAtom(UInt32 nAtom);

            public Hotkey(IntPtr hWnd)
            {
                this.hWnd = hWnd;
                Application.AddMessageFilter(this);
            }

            public int RegisterHotkey(Keys Key, KeyFlags keyflags)
            {
                UInt32 hotkeyid = GlobalAddAtom(System.Guid.NewGuid().ToString());
                RegisterHotKey((IntPtr)hWnd, hotkeyid, (UInt32)keyflags, (UInt32)Key);
                keyIDs.Add(hotkeyid, hotkeyid);
                return (int)hotkeyid;
            }

            public void UnregisterHotkeys()
            {
                Application.RemoveMessageFilter(this);
                foreach (UInt32 key in keyIDs.Values)
                {
                    UnregisterHotKey(hWnd, key);
                    GlobalDeleteAtom(key);
                }
            }

            public bool PreFilterMessage(ref System.Windows.Forms.Message m)
            {
                if (m.Msg == 0x312)
                {
                    if (OnHotkey != null)
                    {
                        foreach (UInt32 key in keyIDs.Values)
                        {
                            if ((UInt32)m.WParam == key)
                            {
                                OnHotkey((int)m.WParam);
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        }
        //热键处理，只是控制是否显示
        public void OnHotkey(int HotkeyID)
        {
            if (HotkeyID == Hotkey1)
            {
                this.Visible = !this.Visible;
            }
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
                this.BackgroundImage = image;
                
            }
        }

        private void Form_scope_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
        }


    }
}

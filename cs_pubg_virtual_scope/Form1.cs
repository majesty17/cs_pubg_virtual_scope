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
    public partial class Form1 : Form
    {
        private Form_scope form_scope = null;


        public Form1()
        {
            InitializeComponent();

            //热键
            Hotkey hotkey;
            hotkey = new Hotkey(this.Handle);
            Hotkey1 = hotkey.RegisterHotkey(System.Windows.Forms.Keys.G, Hotkey.KeyFlags.MOD_SHIFT);   //定义快键(Ctrl + F2)
            hotkey.OnHotkey += new HotkeyEventHandler(OnHotkey);

        }


        //开关
        private void checkBox_switch_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_switch.Checked)
            {
                getParamAndRun();
                this.WindowState = FormWindowState.Minimized;
            }
            else
            {
                form_scope.Visible = false;
                this.WindowState = FormWindowState.Normal;
            }
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
                checkBox_switch.Checked = !checkBox_switch.Checked;
            }
        }


        private void getParamAndRun()
        {
            if (form_scope != null)
            {
                form_scope.Dispose();
                form_scope = null;
            }

            int scope_weight = (int)(numericUpDown_scope_weight.Value);
            int scope_height = (int)(numericUpDown_scope_height.Value);

            double times = (double)numericUpDown_scope_times.Value;
            int freq = (int)numericUpDown_freq.Value;

            int screen_w = Screen.PrimaryScreen.Bounds.Width;
            int screen_h = Screen.PrimaryScreen.Bounds.Height;

            int scope_x = screen_w / 2 + (int)numericUpDown_scope_pos_X.Value - scope_weight / 2;
            int scope_y = screen_h / 2 + (int)numericUpDown_scope_pos_Y.Value - scope_height / 2;

            int scope_opa = (int)numericUpDown_scope_opa.Value;
            bool color_reverse = checkBox_color_rev.Checked;
            bool sharpen = checkBox_sharpen.Checked;

            form_scope = new Form_scope();
            form_scope.times = times;
            form_scope.freq = freq;
            form_scope.color_reverse = color_reverse;
            form_scope.sharpen = sharpen;
            form_scope.Height = scope_height;
            form_scope.Width = scope_weight;
            form_scope.Location = new Point(scope_x, scope_y);
            form_scope.TopMost = true;
            form_scope.Show();
            //form_scope.Visible = false;
            form_scope.Opacity = scope_opa / 100.0;
        }

    }
}

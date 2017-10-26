using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cs_pubg_virtual_scope
{
    public partial class Form1 : Form
    {
        private Form_scope form_scope = null;


        public Form1()
        {
            InitializeComponent();
        }

        //开关
        private void checkBox_switch_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_switch.Checked)
            {
                int scope_weight = (int)(numericUpDown_scope_weight.Value);
                int scope_height = (int)(numericUpDown_scope_height.Value);

                double times = (double)numericUpDown_scope_times.Value;
                int freq = (int)numericUpDown_freq.Value;

                int screen_w=Screen.PrimaryScreen.Bounds.Width;
                int screen_h=Screen.PrimaryScreen.Bounds.Height;

                int scope_x = screen_w / 2 + (int)numericUpDown_scope_pos_X.Value - scope_weight / 2;
                int scope_y = screen_h / 2 + (int)numericUpDown_scope_pos_Y.Value - scope_height / 2;

                int scope_opa = (int)numericUpDown_scope_opa.Value;
                bool color_reverse = checkBox_color_rev.Checked;


                form_scope = new Form_scope();
                form_scope.times = times;
                form_scope.freq = freq;
                form_scope.color_reverse = color_reverse;
                form_scope.Height = scope_height;
                form_scope.Width = scope_weight;
                form_scope.Location = new Point(scope_x, scope_y);
                form_scope.TopMost = true;
                form_scope.Show();
                form_scope.Visible = false;
                form_scope.Opacity = scope_opa / 100.0;
            }
            else
            {
                form_scope.Close();
                form_scope.Dispose();
            }
        }


    }
}

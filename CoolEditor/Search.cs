using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoolEditor
{
    public partial class Search : Form
    {
        public Search()
        {
            InitializeComponent();
        }

        public RichTextBox rtb;
        int start = 0;
        string str = "";
        RichTextBoxFinds f;
        private void button1_Click(object sender, EventArgs e)
        {
            str = this.textBox1.Text;
            //查找
            start = rtb.Find(str, start, f);
            if (start == -1)
            {
                MessageBox.Show("Sorry!Cannot find" + str + "！", "Reminder", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                start = 0;
            }
            else
            {
                start = start + str.Length;//Start next from the current position
                rtb.Focus(); // Give focus
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked == false)
                f = RichTextBoxFinds.None;
            else
                f = RichTextBoxFinds.MatchCase;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}

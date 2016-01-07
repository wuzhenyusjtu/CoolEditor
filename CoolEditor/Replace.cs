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
    public partial class Replace : Form
    {
        public Replace()
        {
            InitializeComponent();
        }

        public RichTextBox rtb;
        int start = 0;// Start position
        string str = "";//Search
        string str2 = ""; // Replace
        RichTextBoxFinds f;
        int i = 0;

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            str = this.textBox1.Text;
            //查找
            start = rtb.Find(str, start, f);
            if (start == -1)
            {
                MessageBox.Show("Sorry！Cannot find" + str + "！", "Reminder", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                start = 0;
            }
            else
            {
                start = start + str.Length;//If found, start next from current position
                rtb.Focus(); // Give focus
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            str = this.textBox1.Text;// Content for search
            str2 = this.textBox2.Text;
            start = rtb.Find(str, start, f);
            if (start == -1)
            {
                MessageBox.Show("Sorry！Cannot find" + str + "！", "Reminder", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                start = 0;
            }
            else
            {
                rtb.SelectedRtf = str2;
                start = start + str.Length;// If found, start next from current position
                rtb.Focus(); // Give focus
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            str = this.textBox1.Text;//Content for search
            str2 = this.textBox2.Text;
            start = rtb.Find(str, start, f);
            while (start != -1)
            {
                rtb.SelectedRtf = str2;
                start = start + str.Length;
                start = rtb.Find(str, start, f);
                i++;
            }
            MessageBox.Show("Complete!" + i.ToString() + "altogether", "Reminder");
            start = 0;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}

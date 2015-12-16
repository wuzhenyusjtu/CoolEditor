using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CoolEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.toolStripStatusLabel2.Text = "Lines:" + this.myTextBox1.Lines.Length;
        }


        private void newNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.myTextBox1.Modified == true)
            {
                DialogResult r =
                    MessageBox.Show("Do you want to save changes you made to ", "CoolEditor", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                if (r == DialogResult.Yes)
                {
                    saveAsAToolStripMenuItem_Click(sender, e);
                    this.myTextBox1.Clear();
                    this.Text = "";
                }
                else if (r == DialogResult.No)
                {
                    this.myTextBox1.Clear();
                    this.Text = "";
                }
                else
                    return;
            }
            else
            {
                this.myTextBox1.Clear();
            }
        }

        private void openOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = "Open File...";
            this.openFileDialog1.Filter = "text files(*.txt;*.rtf)|*.txt;*.rtf|all files(*.*)|*.*";
            this.openFileDialog1.FilterIndex = 1;
            this.openFileDialog1.InitialDirectory = @"C:\Users\";
            this.openFileDialog1.ShowReadOnly = true;
            this.openFileDialog1.ReadOnlyChecked = false;
            this.openFileDialog1.FileName = "";

            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.Text = this.openFileDialog1.FileName;

                StreamReader sr = new StreamReader
                    (this.openFileDialog1.FileName, Encoding.Default);
                this.myTextBox1.Rtf = sr.ReadToEnd();
                sr.Close();
            }
        }

        private void saveSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.FileName == "")//no open
            {
                if (this.saveFileDialog1.FileName == "")//no save
                {
                    //pop up message reminder
                    if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        StreamWriter sw = new StreamWriter(this.saveFileDialog1.FileName);
                        sw.Write(this.myTextBox1.Rtf);
                        sw.Close();
                    }
                }
                else//save
                {
                    //no pop up message reminder
                    StreamWriter sw = new StreamWriter(this.saveFileDialog1.FileName);
                    sw.Write(this.myTextBox1.Rtf);
                    sw.Close();
                }
            }
            else//open file
            {
                if (this.saveFileDialog1.FileName == "")//no save
                {
                    StreamWriter sw = new StreamWriter(this.openFileDialog1.FileName);
                    sw.Write(this.myTextBox1.Rtf);
                    sw.Close();
                }
                else
                {
                    StreamWriter sw = new StreamWriter(this.saveFileDialog1.FileName);
                    sw.Write(this.myTextBox1.Rtf);
                    sw.Close();
                }
            }
        }

        private void saveAsAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.Title = "SaveAs...";
            this.saveFileDialog1.Filter = "text files(*.txt;*.rtf)|*.txt;*.rtf|all files(*.*)|*.*";
            this.saveFileDialog1.InitialDirectory = @"C:\Users";
            this.saveFileDialog1.FileName = "";
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(this.saveFileDialog1.FileName);
                sw.Write(this.myTextBox1.Rtf);
                sw.Close();
            }
        }

        private void exitXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.myTextBox1.Modified == true)
            {
                DialogResult r =
                    MessageBox.Show("Do you want to save changes you made to" + this.Text, "CoolEditor", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                if (r == DialogResult.Yes)
                {
                    saveAsAToolStripMenuItem_Click(sender, e);
                    this.myTextBox1.Clear();
                    this.Text = "";
                }
                else if (r == DialogResult.No)
                {
                    Application.Exit();
                }
            }
        }

        private void undoUToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myTextBox1.Undo();
        }

        private void cutTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (myTextBox1.SelectedRtf == "")
                return;
            else
                myTextBox1.Cut();
        }

        private void copyCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (myTextBox1.SelectedRtf == "")
                return;
            else
                myTextBox1.Copy();
        }

        private void pastePToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myTextBox1.Paste();
        }

        private void deleteLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (myTextBox1.SelectedRtf != "")
                myTextBox1.SelectedRtf = "";
        }

        private void selectAllAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myTextBox1.SelectAll();
        }

        private void replaceRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Replace replace = new Replace();
            replace.rtb = this.myTextBox1; //pass value from main window to FindForm
            replace.Owner = this; //suspend on the current window
            replace.Show();
        }

        private void findFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Search f = new Search();
            f.rtb = this.myTextBox1; //pass value from main window to FindForm
            f.Owner = this; //suspend on the current window
            f.Show();
        }

        private void wordWrapWToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.wordWrapWToolStripMenuItem.Checked == false)
            {
                this.wordWrapWToolStripMenuItem.Checked = true;
                this.myTextBox1.WordWrap = true;
            }
            else
            {
                this.wordWrapWToolStripMenuItem.Checked = false;
                this.myTextBox1.WordWrap = false;
            }
        }

        private void fontFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.fontDialog1.ShowEffects = true;
            this.fontDialog1.Font = this.myTextBox1.SelectionFont;
            if (this.fontDialog1.ShowDialog() == DialogResult.OK)
            {
                this.myTextBox1.SelectionFont = this.fontDialog1.Font;
            }
        }

        private void colorCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.colorDialog1.AnyColor = true;
            this.colorDialog1.Color = this.myTextBox1.SelectionColor;
            if (this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                this.myTextBox1.SelectionColor = this.colorDialog1.Color;
            }
        }

        private void statusBarToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (this.statusBarToolStripMenuItem.Checked == false)
            {
                this.statusBarToolStripMenuItem.Checked = true;
                this.statusStrip1.Visible = true;
                this.myTextBox1.Height -= 22;
            }
            else
            {
                this.statusBarToolStripMenuItem.Checked = false;
                this.statusStrip1.Visible = false;
                this.myTextBox1.Height += 22;
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (myTextBox1.SelectedRtf != "")
                myTextBox1.SelectedRtf = "";
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myTextBox1.Undo();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myTextBox1.SelectAll();

        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (myTextBox1.SelectedRtf == "")
                return;
            else
                myTextBox1.Copy();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (myTextBox1.SelectedRtf == "")
                return;
            else
                myTextBox1.Cut();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myTextBox1.Paste();
        }

        private void maximizeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private void minimizeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.myTextBox1.Modified == true)
            {
                DialogResult r =
                    MessageBox.Show("Do you want to save changes you made to" + this.Text, "CoolEditor", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                if (r == DialogResult.Yes)
                {
                    saveAsAToolStripMenuItem_Click(sender, e);
                    this.myTextBox1.Clear();
                    this.Text = "";
                }
                else if (r == DialogResult.No)
                {
                    Application.Exit();
                }
            }
        }

        private void myTextBox1_TextChanged_1(object sender, EventArgs e)
        {
            this.toolStripStatusLabel2.Text = "Lines:" + this.myTextBox1.Lines.Length;
        }


        private void indentButton_Click(object sender, EventArgs e)
        {
            this.myTextBox1.SelectionColor = Color.Black;
            string code = this.myTextBox1.SelectedText;
            Indenter indenter = new Indenter();
            this.myTextBox1.SelectedText = indenter.ExplicitIndenting(code);
            this.myTextBox1.SelectionStart = this.myTextBox1.SelectedText.Length;
        }

        private void cleanUpButton_Click(object sender, EventArgs e)
        {
            this.myTextBox1.SelectionColor = Color.Black;
            string code = this.myTextBox1.SelectedText;
            CleanUpper cleanUpper = new CleanUpper();
            this.myTextBox1.SelectedText = cleanUpper.ExplicitCleaning(code);
            this.myTextBox1.SelectionStart = this.myTextBox1.SelectedText.Length;
        }

        private void highlightButton_Click(object sender, EventArgs e)
        {
            //create compilation
            string plainCode = this.myTextBox1.SelectedText;
            Colorer colorer = new Colorer();
            this.myTextBox1.SelectedRtf = colorer.ExplicitColoring(plainCode);
            this.myTextBox1.SelectionStart = this.myTextBox1.SelectedText.Length;
        }
    }
}

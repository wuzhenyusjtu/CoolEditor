namespace CoolEditor
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newNToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoUToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pastePToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formatOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wordWrapWToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.Format = new System.Windows.Forms.ToolStripButton();
            this.Cleanup = new System.Windows.Forms.ToolStripButton();
            this.Highlight = new System.Windows.Forms.ToolStripButton();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maximizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.minimizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.myTextBox1 = new CoolEditor.MyTextBox();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileFToolStripMenuItem,
            this.editEToolStripMenuItem,
            this.formatOToolStripMenuItem,
            this.viewVToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(601, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileFToolStripMenuItem
            // 
            this.fileFToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newNToolStripMenuItem,
            this.openOToolStripMenuItem,
            this.saveSToolStripMenuItem,
            this.saveAsAToolStripMenuItem,
            this.exitXToolStripMenuItem});
            this.fileFToolStripMenuItem.Name = "fileFToolStripMenuItem";
            this.fileFToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.fileFToolStripMenuItem.Text = "File(&F)";
            // 
            // newNToolStripMenuItem
            // 
            this.newNToolStripMenuItem.Name = "newNToolStripMenuItem";
            this.newNToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.newNToolStripMenuItem.Text = "New(&N)";
            this.newNToolStripMenuItem.Click += new System.EventHandler(this.newNToolStripMenuItem_Click);
            // 
            // openOToolStripMenuItem
            // 
            this.openOToolStripMenuItem.Name = "openOToolStripMenuItem";
            this.openOToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.openOToolStripMenuItem.Text = "Open(&O)";
            this.openOToolStripMenuItem.Click += new System.EventHandler(this.openOToolStripMenuItem_Click);
            // 
            // saveSToolStripMenuItem
            // 
            this.saveSToolStripMenuItem.Name = "saveSToolStripMenuItem";
            this.saveSToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.saveSToolStripMenuItem.Text = "Save(&S)";
            this.saveSToolStripMenuItem.Click += new System.EventHandler(this.saveSToolStripMenuItem_Click);
            // 
            // saveAsAToolStripMenuItem
            // 
            this.saveAsAToolStripMenuItem.Name = "saveAsAToolStripMenuItem";
            this.saveAsAToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.saveAsAToolStripMenuItem.Text = "Save As(&A)";
            this.saveAsAToolStripMenuItem.Click += new System.EventHandler(this.saveAsAToolStripMenuItem_Click);
            // 
            // exitXToolStripMenuItem
            // 
            this.exitXToolStripMenuItem.Name = "exitXToolStripMenuItem";
            this.exitXToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.exitXToolStripMenuItem.Text = "Exit(&X)";
            this.exitXToolStripMenuItem.Click += new System.EventHandler(this.exitXToolStripMenuItem_Click);
            // 
            // editEToolStripMenuItem
            // 
            this.editEToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutTToolStripMenuItem,
            this.undoUToolStripMenuItem,
            this.copyCToolStripMenuItem,
            this.pastePToolStripMenuItem,
            this.deleteLToolStripMenuItem,
            this.selectAllAToolStripMenuItem,
            this.replaceRToolStripMenuItem,
            this.findFToolStripMenuItem});
            this.editEToolStripMenuItem.Name = "editEToolStripMenuItem";
            this.editEToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.editEToolStripMenuItem.Text = "Edit(&E)";
            // 
            // cutTToolStripMenuItem
            // 
            this.cutTToolStripMenuItem.Name = "cutTToolStripMenuItem";
            this.cutTToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.cutTToolStripMenuItem.Text = "Cut(&T)";
            this.cutTToolStripMenuItem.Click += new System.EventHandler(this.cutTToolStripMenuItem_Click);
            // 
            // undoUToolStripMenuItem
            // 
            this.undoUToolStripMenuItem.Name = "undoUToolStripMenuItem";
            this.undoUToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.undoUToolStripMenuItem.Text = "Undo(&U)";
            this.undoUToolStripMenuItem.Click += new System.EventHandler(this.undoUToolStripMenuItem_Click);
            // 
            // copyCToolStripMenuItem
            // 
            this.copyCToolStripMenuItem.Name = "copyCToolStripMenuItem";
            this.copyCToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.copyCToolStripMenuItem.Text = "Copy(&C)";
            this.copyCToolStripMenuItem.Click += new System.EventHandler(this.copyCToolStripMenuItem_Click);
            // 
            // pastePToolStripMenuItem
            // 
            this.pastePToolStripMenuItem.Name = "pastePToolStripMenuItem";
            this.pastePToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.pastePToolStripMenuItem.Text = "Paste(&P)";
            this.pastePToolStripMenuItem.Click += new System.EventHandler(this.pastePToolStripMenuItem_Click);
            // 
            // deleteLToolStripMenuItem
            // 
            this.deleteLToolStripMenuItem.Name = "deleteLToolStripMenuItem";
            this.deleteLToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.deleteLToolStripMenuItem.Text = "Delete(&L)";
            this.deleteLToolStripMenuItem.Click += new System.EventHandler(this.deleteLToolStripMenuItem_Click);
            // 
            // selectAllAToolStripMenuItem
            // 
            this.selectAllAToolStripMenuItem.Name = "selectAllAToolStripMenuItem";
            this.selectAllAToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.selectAllAToolStripMenuItem.Text = "SelectAll(&A)";
            this.selectAllAToolStripMenuItem.Click += new System.EventHandler(this.selectAllAToolStripMenuItem_Click);
            // 
            // replaceRToolStripMenuItem
            // 
            this.replaceRToolStripMenuItem.Name = "replaceRToolStripMenuItem";
            this.replaceRToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.replaceRToolStripMenuItem.Text = "Replace(&R)";
            this.replaceRToolStripMenuItem.Click += new System.EventHandler(this.replaceRToolStripMenuItem_Click);
            // 
            // findFToolStripMenuItem
            // 
            this.findFToolStripMenuItem.Name = "findFToolStripMenuItem";
            this.findFToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.findFToolStripMenuItem.Text = "Find(&F)";
            this.findFToolStripMenuItem.Click += new System.EventHandler(this.findFToolStripMenuItem_Click);
            // 
            // formatOToolStripMenuItem
            // 
            this.formatOToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wordWrapWToolStripMenuItem,
            this.fontFToolStripMenuItem,
            this.colorCToolStripMenuItem});
            this.formatOToolStripMenuItem.Name = "formatOToolStripMenuItem";
            this.formatOToolStripMenuItem.Size = new System.Drawing.Size(74, 20);
            this.formatOToolStripMenuItem.Text = "Format(&O)";
            // 
            // wordWrapWToolStripMenuItem
            // 
            this.wordWrapWToolStripMenuItem.Name = "wordWrapWToolStripMenuItem";
            this.wordWrapWToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.wordWrapWToolStripMenuItem.Text = "Word Wrap(&W)";
            this.wordWrapWToolStripMenuItem.Click += new System.EventHandler(this.wordWrapWToolStripMenuItem_Click);
            // 
            // fontFToolStripMenuItem
            // 
            this.fontFToolStripMenuItem.Name = "fontFToolStripMenuItem";
            this.fontFToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.fontFToolStripMenuItem.Text = "Font(&F)";
            this.fontFToolStripMenuItem.Click += new System.EventHandler(this.fontFToolStripMenuItem_Click);
            // 
            // colorCToolStripMenuItem
            // 
            this.colorCToolStripMenuItem.Name = "colorCToolStripMenuItem";
            this.colorCToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.colorCToolStripMenuItem.Text = "Color(&C)";
            this.colorCToolStripMenuItem.Click += new System.EventHandler(this.colorCToolStripMenuItem_Click);
            // 
            // viewVToolStripMenuItem
            // 
            this.viewVToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBarToolStripMenuItem});
            this.viewVToolStripMenuItem.Name = "viewVToolStripMenuItem";
            this.viewVToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.viewVToolStripMenuItem.Text = "View(&V)";
            // 
            // statusBarToolStripMenuItem
            // 
            this.statusBarToolStripMenuItem.Name = "statusBarToolStripMenuItem";
            this.statusBarToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.statusBarToolStripMenuItem.Text = "Status Bar";
            this.statusBarToolStripMenuItem.Click += new System.EventHandler(this.statusBarToolStripMenuItem_Click_1);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Format,
            this.Cleanup,
            this.Highlight});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(601, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // Format
            // 
            this.Format.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Format.Image = ((System.Drawing.Image)(resources.GetObject("Format.Image")));
            this.Format.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Format.Name = "Format";
            this.Format.Size = new System.Drawing.Size(23, 22);
            this.Format.Text = "Format";
            this.Format.Click += new System.EventHandler(this.indentButton_Click);
            // 
            // Cleanup
            // 
            this.Cleanup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Cleanup.Image = ((System.Drawing.Image)(resources.GetObject("Cleanup.Image")));
            this.Cleanup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Cleanup.Name = "Cleanup";
            this.Cleanup.Size = new System.Drawing.Size(23, 22);
            this.Cleanup.Text = "Cleanup";
            this.Cleanup.Click += new System.EventHandler(this.cleanUpButton_Click);
            // 
            // Highlight
            // 
            this.Highlight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Highlight.Image = ((System.Drawing.Image)(resources.GetObject("Highlight.Image")));
            this.Highlight.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Highlight.Name = "Highlight";
            this.Highlight.Size = new System.Drawing.Size(23, 22);
            this.Highlight.Text = "Highlight";
            this.Highlight.Click += new System.EventHandler(this.highlightButton_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem,
            this.undoToolStripMenuItem,
            this.selectAllToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.maximizeToolStripMenuItem,
            this.minimizeToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(125, 202);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.selectAllToolStripMenuItem.Text = "SelectAll";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // maximizeToolStripMenuItem
            // 
            this.maximizeToolStripMenuItem.Name = "maximizeToolStripMenuItem";
            this.maximizeToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.maximizeToolStripMenuItem.Text = "Maximize";
            this.maximizeToolStripMenuItem.Click += new System.EventHandler(this.maximizeToolStripMenuItem_Click_1);
            // 
            // minimizeToolStripMenuItem
            // 
            this.minimizeToolStripMenuItem.Name = "minimizeToolStripMenuItem";
            this.minimizeToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.minimizeToolStripMenuItem.Text = "Minimize";
            this.minimizeToolStripMenuItem.Click += new System.EventHandler(this.minimizeToolStripMenuItem_Click_1);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 459);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(601, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel2.Text = "toolStripStatusLabel2";
            // 
            // myTextBox1
            // 
            this.myTextBox1.AcceptsTab = true;
            this.myTextBox1.AutoWordSelection = true;
            this.myTextBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.myTextBox1.DetectUrls = false;
            this.myTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myTextBox1.Font = new System.Drawing.Font("Consolas", 10F);
            this.myTextBox1.Location = new System.Drawing.Point(0, 49);
            this.myTextBox1.Name = "myTextBox1";
            this.myTextBox1.Size = new System.Drawing.Size(601, 432);
            this.myTextBox1.TabIndex = 2;
            this.myTextBox1.Text = "";
            this.myTextBox1.WordWrap = false;
            this.myTextBox1.TextChanged += new System.EventHandler(this.myTextBox1_TextChanged_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(601, 481);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.myTextBox1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "CoolEditor";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Members
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newNToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openOToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitXToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editEToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoUToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutTToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pastePToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceRToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem formatOToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wordWrapWToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fontFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem colorCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewVToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem maximizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem minimizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem statusBarToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripButton Format;
        private System.Windows.Forms.ToolStripButton Cleanup;
        private System.Windows.Forms.ToolStripButton Highlight;
        #endregion

        private MyTextBox myTextBox1;
    }
}


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Resources;

namespace CoolEditor
{
    public partial class MyTextBox : RichTextBox
    {
        #region Members

        #region Static Members
        private static Colorer myColorer = new Colorer();
        private static Indenter myIndenter = new Indenter();
        private static DisplayIntellisense myIntellisenser = new DisplayIntellisense();
        public static Image classImage = intellisense_image._class;
        public static Image eventImage = intellisense_image._event;
        public static Image interfaceImage = intellisense_image._interface;
        public static Image methodImage = intellisense_image.method;
        public static Image namespaceImage = intellisense_image._namespace;
        public static Image propertyImage = intellisense_image.property;

        #endregion

        #region static members of keys
        private static Keys[] highlightReminder = { Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.A,
            Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L,Keys.M, Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S,
            Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z, Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5,
            Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9, Keys.Multiply, Keys.Add, Keys.Separator, Keys.Subtract, Keys.Decimal, Keys.Divide,
            Keys.OemOpenBrackets, Keys.OemCloseBrackets, Keys.Oemcomma, Keys.OemMinus, Keys.Oemplus, Keys.OemSemicolon, Keys.OemBackslash, Keys.OemQuotes, Keys.Back, };
        private static Keys[] combinationKey = { Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.Subtract, Keys.Add,
            Keys.Multiply, Keys.Divide, Keys.OemMinus, Keys.Oemplus, Keys.OemQuestion,Keys.OemPeriod, Keys.Oemcomma, Keys.OemQuotes, Keys.Oemtilde, Keys.OemBackslash,
            Keys.OemCloseBrackets, Keys.OemOpenBrackets, Keys.OemSemicolon};
        private static Keys[] intellisenseReminder = { Keys.OemPeriod };
        #endregion

        #region private Members
        private IntellisenseListBox intellisenseBox;
        #endregion
        #endregion

        #region Drawing
        private bool enablePainting = true;
        private bool enableExplicitColoring = false;
        private bool enableDynamicColoring = false;
        private bool enableIndentting = false;
        private bool enableDynamicIndenting = false;
        private bool enableExplicitIndenting = false;
        private bool enableIntellisense = false;
        private bool enableGetAcessibleVariables = false;

        #endregion

        #region Properties
        #region Internal Properties

        /// <summary>
        /// Enables or disables the control's paint event.
        /// </summary>
        internal bool EnablePainting
        {
            get { return enablePainting; }
            set { enablePainting = value; }
        }
        internal IntellisenseListBox IntellisenseBox
        {
            get { return intellisenseBox; }
            set { intellisenseBox = value; }
        }
        internal bool EnableExplicitColoring
        {
            get { return enableExplicitColoring; }
            set { enableExplicitColoring = value; }
        }
        internal bool EnableDynamicColoring
        {
            get { return enableDynamicColoring; }
            set { enableDynamicColoring = value; }
        }
        internal bool EnableExplicitIndenting
        {
            get { return enableExplicitIndenting; }
            set { enableExplicitIndenting = value; }
        }

        internal bool EnableDynamicIndenting
        {
            get { return enableDynamicIndenting; }
            set { enableDynamicIndenting = value; }
        }

        internal bool EnableIntellisense
        {
            get { return enableIntellisense; }
            set { enableIntellisense = value; }
        }
        internal bool EnableGetAcessibleVariables
        {
            get { return enableGetAcessibleVariables; }
            set { enableGetAcessibleVariables = value; }
        }
        #endregion
        #endregion

        #region Constructors
        public MyTextBox()
        {
            InitializeComponent();

            //set some defaults...
            this.AcceptsTab = true;
            this.Font = new Font(FontFamily.GenericMonospace, 8f);

            //Do not enable drag and dropping text
            //The same problem, as paste - the onDragDrop event fires, BEFORE the text is written into the textbox
            //Need to be handled in WndPrc
            this.EnableAutoDragDrop = false;
            this.DetectUrls = false;
            this.WordWrap = false;
            this.AutoWordSelection = true;

            intellisenseBox = new IntellisenseListBox();

            #region Setup intellisense box
            this.Controls.Add(intellisenseBox);
            intellisenseBox.Size = new Size(250, 150);
            intellisenseBox.Visible = false;
            intellisenseBox.KeyDown += new KeyEventHandler(intellisenseBox_KeyDown);
            intellisenseBox.DoubleClick += new EventHandler(intellisenseBox_DoubleClick);
            #endregion
        }

        private void intellisenseBox_DoubleClick(object sender, EventArgs e)
        {
            myIntellisenser.ConfirmIntellisense(this);
            this.EnableDynamicColoring = true;
        }

        private void intellisenseBox_KeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(e);
        }

        #endregion


        #region Overridden methods

        private const int WM_PAINT = 0x000F;

        /// <summary>
        /// Let control enable and disable it's drawing...
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref System.Windows.Forms.Message msg)
        {
            switch (msg.Msg)
            {
                case WM_PAINT:
                    if (enablePainting)
                    {
                        base.WndProc(ref msg);
                    }
                    else
                    {
                        msg.Result = IntPtr.Zero;
                    }
                    break;

                default:
                    base.WndProc(ref msg);
                    break;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (intellisenseBox.Visible)
            {
                if (e.KeyData == Keys.Escape || e.KeyData == Keys.Back || e.KeyData == Keys.OemPeriod || e.KeyData == Keys.Space || e.KeyData == Keys.Tab)
                {
                    myIntellisenser.HideIntellisenseBox(this);
                }
                
                else if (e.KeyData == Keys.Up)
                {
                    myIntellisenser.NavigateUp(this,1);
                }
                else if (e.KeyData == Keys.Down)
                {
                    myIntellisenser.NavigateDown(this,1);
                }
                else if (e.KeyData == Keys.Home)
                {
                    myIntellisenser.NavigateHome(this);
                }
                else if (e.KeyData == Keys.End)
                {
                    myIntellisenser.NavigateEnd(this);
                }
                else
                {
                }
            }
            // If intellisense box is not visible
            else
            {
                // Enable highlighting
                if (highlightReminder.Contains(e.KeyCode) && e.Modifiers.CompareTo(Keys.Control) != 0)
                {
                    this.Focus();
                    base.OnKeyDown(e);
                    if (e.KeyData == Keys.Oemplus)
                    {
                        this.EnableGetAcessibleVariables = true;
                    }
                    this.EnableDynamicColoring = true;
                }
                // Paste Operation will NOT enable Indenting and Coloring
                else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.V)
                {
                    this.Focus();
                    base.OnKeyDown(e);
                    this.EnableExplicitIndenting = false;
                    this.EnableExplicitColoring = false;
                }
                // Cut Operation will enable Indenting and Coloring
                else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.X)
                {
                    this.Focus();
                    base.OnKeyDown(e);
                    this.EnableExplicitIndenting = true;
                    this.EnableExplicitColoring = true;
                }
                // If user types enter key...
                else if (e.KeyData == Keys.Enter)
                {
                    this.Focus();
                    base.OnKeyDown(e);
                    //this.EnableDynamicIndenting = true;
                    this.EnableGetAcessibleVariables = true;
                    this.EnableDynamicIndenting = true;
                    this.EnableExplicitColoring = true;
                }
                // If user types period key
                else if (intellisenseReminder.Contains(e.KeyData))
                {
                    this.Focus();
                    base.OnKeyDown(e);
                    this.EnableIntellisense = true;
                }
                else
                {
                    this.Focus();
                    base.OnKeyDown(e);
                }
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (EnableDynamicIndenting)
            {
                CodeWithPosition CodeWithPosition = myIndenter.DynamicIndenting(this.Text, this.SelectionStart);
                this.Text = CodeWithPosition.Code;
                this.SelectionStart = CodeWithPosition.Position;
                this.SelectionLength = 0;
                this.SelectionColor = Color.Black;
                this.EnableDynamicIndenting = false;
            }
            if (EnableExplicitIndenting)
            {
                this.Text = myIndenter.ExplicitIndenting(this.Text);
                this.SelectionStart = this.Text.Length;
                this.SelectionLength = 0;
                this.SelectionColor = Color.Black;
                this.EnableExplicitIndenting = false;
            }
            if (EnableExplicitColoring)
            {
                this.EnablePainting = false;
                int nPosition = this.SelectionStart;
                this.Rtf=myColorer.ExplicitColoring(this.Text);
                this.SelectionStart = nPosition;
                this.SelectionLength = 0;
                this.SelectionColor = Color.Black;
                this.EnablePainting = true;
                this.EnableExplicitColoring = false;
            }
            if (EnableDynamicColoring)
            {
                this.EnablePainting = false;
                // Save the position and make the whole line black
                int nPosition = this.SelectionStart;
                SpanWithType spanWithType = new SpanWithType();
                Color color = new Color();
                if (this.SelectionStart > 0 )
                {
                    spanWithType = myColorer.DynamicColoring(this.Text.Substring(0, this.SelectionStart));

                    #region color node according to its type
                    switch (spanWithType.Type)
                    {
                        case CodeType.keywordType:
                            color = Color.Blue;
                            break;
                        case CodeType.punctuationType:
                            color = Color.Black;
                            break;
                        case CodeType.operatorType:
                            color = Color.Black;
                            break;
                        case CodeType.numericType:
                            color = Color.Black;
                            break;
                        case CodeType.stringType:
                            color = Color.Magenta;
                            break;
                        case CodeType.variableType:
                            color = Color.Black;
                            break;
                        case CodeType.commentType:
                            color = Color.Green;
                            break;
                        case CodeType.classType:
                            color = Color.Teal;
                            break;
                        case CodeType.defaultType:
                            color = Color.Black;
                            break;
                        default:
                            color = Color.Black;
                            break;
                    }
                    #endregion

                    //color the last word marked by the cursor
                    if (spanWithType.Span != null)
                    {
                        this.SelectionStart = spanWithType.Span.Start;
                        this.SelectionLength = spanWithType.Span.Length;
                        this.SelectionColor = color;
                    }
                }
                //return the settings to the default
                this.SelectionStart = nPosition;
                this.SelectionLength = 0;
                this.SelectionColor = Color.Black;
                this.EnablePainting = true;
                this.EnableDynamicColoring = false;
            }
            if (EnableIntellisense)
            {
                // Show Intellisense
                myIntellisenser.ShowIntellisense(this);
                this.EnableIntellisense = false;
            }
            if (EnableGetAcessibleVariables)
            {
                // Show accesible variables at this scope
                myIntellisenser.ShowLocalVariables(this);
                this.EnableGetAcessibleVariables = false;
            }
            base.OnKeyUp(e);
        }
        #endregion
    }
}
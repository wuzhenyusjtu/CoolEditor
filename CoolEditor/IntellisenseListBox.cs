using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace CoolEditor
{
    public partial class IntellisenseListBox : ListBox
    {
        #region Members
        private Size itemSize = new Size(30, 30);
        #endregion

        #region Constructors
        public IntellisenseListBox()
        {
            InitializeComponent();
            this.DrawMode = DrawMode.OwnerDrawFixed;
        }
        #endregion

        #region Properties
        public Size ItemSize
        {
            get { return itemSize; }
            set { itemSize = value; }
        }
        #endregion

        public IntellisenseListBox(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        #region methods
        public void AddItem(IntellisenseItem item)
        {
            this.Items.Add(item);
        }

        public void RemoveItem(IntellisenseItem item)
        {
            this.Items.Remove(item);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (this.DesignMode)
            {
                base.OnDrawItem(e);
                return;
            }

            //draw background & focus rect
            e.DrawBackground();
            e.DrawFocusRectangle();

            //check if it is an item from the items collection
            if (e.Index < 0)
            {

                // not an item, draw the text (indented)
                e.Graphics.DrawString(this.Text,
                    e.Font,
                    new SolidBrush(e.ForeColor),
                    e.Bounds.Left + itemSize.Width,
                    e.Bounds.Top);
            }
            else
            {
                //check if item is an intellisense item
                if (this.Items[e.Index].GetType() == typeof(IntellisenseItem))
                {
                    //get item to draw
                    IntellisenseItem item = (IntellisenseItem)this.Items[e.Index];

                    //get forecolor & font
                    Color forecolor = Color.Black;
                    Font font = new Font(e.Font, FontStyle.Bold);
                    // draw the item...
                    if (item.Image != null)
                    {
                        #region If has image...
                        //Resize image if necessary...
                        if (
                            item.Image.Width != itemSize.Width ||
                            item.Image.Height != itemSize.Height
                            )
                        {
                            ResizeImage(item.Image, itemSize.Width, itemSize.Height);
                        }

                        //Draw image
                        e.Graphics.DrawImage(
                            item.Image,
                            e.Bounds.Left,
                            e.Bounds.Top);
                        #endregion
                    }
                    //Draw text
                    e.Graphics.DrawString(
                        item.Text,
                        font,
                        new SolidBrush(forecolor),
                        e.Bounds.Left + itemSize.Width,
                        e.Bounds.Top);
                }
                else
                    // it is not an ImageComboItem, draw it
                    e.Graphics.DrawString(this.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds.Left + itemSize.Width, e.Bounds.Top);
            }
            base.OnDrawItem(e);
        }
        #endregion

        #region Private methods
        private static Image ResizeImage(Image imgPhoto, int width, int height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)width / (float)sourceWidth);
            nPercentH = ((float)height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(width, height,
                              PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                             imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Red);
            grPhoto.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }
        #endregion
    }
}

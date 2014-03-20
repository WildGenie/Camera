using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CameraCapture
{
    public partial class CameraButtons : Control
    {
        Bitmap backIcon;
        public Bitmap BackIcon
        {
            get { return backIcon; }
            set { backIcon = value; }
        }
        public CameraButtons()
        {
            InitializeComponent();
        }
        protected override void InitLayout()
        {
            base.InitLayout();
            this.SetStyle(ControlStyles.SupportsTransparentBackColor
                 | ControlStyles.UserPaint
                 | ControlStyles.AllPaintingInWmPaint
                 | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Color.Transparent;
        }
        public Size getSize()
        {
            Size iconSize = new Size(76, 66); //缺省大小
            if (backIcon != null)
            {
                int width = backIcon.Width;
                int height = backIcon.Height;
                iconSize = new Size(width, height);   
            }
            return iconSize;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            RectangleF rect = new RectangleF(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            if (backIcon != null)
            {
                g.DrawImage(backIcon, 0, 0, rect.Width, rect.Height);    
            }
        }
    }
}

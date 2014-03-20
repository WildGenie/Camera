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
    /// <summary>
    /// 自定义矩形裁剪框控件
    /// </summary> 
    public partial class CroppedRect :Control
    {
        private Point lastPoint;
        private Point screenPoint;
        private Size lastSize;
        private Rectangle[] rectArrow = new Rectangle[4];
        private Rectangle cropArea;
        public ArrowType arrowType = ArrowType.none;
        private int blank = 1;
        private int radius = 10;


        public CroppedRect(int width, int height)
        {  
            this.Width = width;
            this.Height = height;
            //自定义控件透明
            this.SetStyle(ControlStyles.SupportsTransparentBackColor
             | ControlStyles.UserPaint
             | ControlStyles.AllPaintingInWmPaint 
             | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Color.Transparent;
           
            InitializeComponent();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.lastPoint = this.Location;
            this.screenPoint = this.PointToScreen(e.Location);
            this.lastSize = this.Size;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.arrowType = ArrowType.none;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (arrowType != ArrowType.move && arrowType != ArrowType.none)
            {
                reSize(e);
                return;
            }
            //调整鼠标形状
            if (Math.Abs(e.X-cropArea.Left)<=radius && Math.Abs(e.Y - cropArea.Top)<=radius)
            {
                this.Cursor = Cursors.SizeNWSE;//左上
            }
            else if (Math.Abs(e.X - cropArea.Right) <= radius && Math.Abs(e.Y - cropArea.Top) <= radius)
            {
                this.Cursor = Cursors.SizeNESW;//右上
            }
            else if (Math.Abs(e.X - cropArea.Left) <= radius && Math.Abs(e.Y - cropArea.Bottom) <= radius)
            {
                this.Cursor = Cursors.SizeNESW;//左下
            }
            else if (Math.Abs(e.X - cropArea.Right) <= radius && Math.Abs(e.Y - cropArea.Bottom) <= radius)
            {
                this.Cursor = Cursors.SizeNWSE;//右下
            }
            else if (cropArea.Contains(e.Location))
            {
                this.Cursor = Cursors.SizeAll;//中间移动
            }
            else
            {
                this.Cursor = Cursors.Default;
            }

            if(e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                Point tempScrPoint = this.PointToScreen(e.Location);
                Point tempLastPoint = this.lastPoint;

                if (Math.Abs(e.X - cropArea.Left) <= radius && Math.Abs(e.Y - cropArea.Top) <= radius)
                {
                    this.arrowType = ArrowType.leftUp;//左上
                }
                else if (Math.Abs(e.X - cropArea.Right) <= radius && Math.Abs(e.Y - cropArea.Top) <= radius)
                {
                    this.arrowType = ArrowType.rightUp;//右上
                }
                else if (Math.Abs(e.X - cropArea.Left) <= radius && Math.Abs(e.Y - cropArea.Bottom) <= radius)
                {
                    this.arrowType = ArrowType.leftDown;//左下
                }
                else if (Math.Abs(e.X - cropArea.Right) <= radius && Math.Abs(e.Y - cropArea.Bottom) <= radius)
                {
                    this.arrowType = ArrowType.rightDown;//右下
                }
                else if (cropArea.Contains(e.Location))
                {
                    this.arrowType = ArrowType.move;//中间移动
                }
                else
                {
                    this.arrowType = ArrowType.none;
                }

                tempLastPoint.Offset(tempScrPoint.X - this.screenPoint.X, tempScrPoint.Y - this.screenPoint.Y);

                if (arrowType == ArrowType.move)
                {
                    this.Location = tempLastPoint;
                    Refresh();
                    this.Parent.Refresh();
                }
                else
                {
                    reSize(e);
                }
            }


        }
        /// <summary>
        ///  rectArrow[0] 左上角调节裁剪框大小用的小圆圈
        ///  rectArrow[1] 右上角调节裁剪框大小用的小圆圈
        ///  rectArrow[2] 右下角调节裁剪框大小用的小圆圈
        ///  rectArrow[3] 左下角调节裁剪框大小用的小圆圈
        ///  rectArrow[4] 裁剪框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        System.Drawing.SolidBrush rectArrowBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(0, 255, 255));
        protected override void OnPaint(PaintEventArgs e)
        {
            rectArrow[0] = new Rectangle(new Point(blank, blank), new Size(radius * 2, radius * 2));
            rectArrow[1] = new Rectangle(new Point(this.Width - radius * 2 - blank, blank), new Size(radius * 2, radius * 2));
            rectArrow[2] = new Rectangle(new Point(this.Width - radius * 2 - blank, this.Height - radius * 2 - blank),
                new Size(radius * 2, radius * 2));
            rectArrow[3] = new Rectangle(new Point(blank, this.Height - radius * 2 - blank), new Size(radius * 2, radius * 2));
            cropArea = new Rectangle(new Point(blank + radius, blank + radius),
                new Size(this.Width - blank * 2 - radius * 2, this.Height - blank * 2 - radius * 2));

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            foreach (Rectangle item in rectArrow)
            {
                e.Graphics.DrawEllipse(Pens.Black, item);  
                e.Graphics.FillEllipse(rectArrowBrush, item);
            }
            e.Graphics.DrawRectangle(Pens.Black, cropArea);
        }


        //根据鼠标位置调整大小
        private void reSize(MouseEventArgs e)
        {
            Point t = this.PointToScreen(e.Location);
            Point l = this.lastPoint;

            l.Offset(t.X - this.screenPoint.X, t.Y - this.screenPoint.Y);

            switch (arrowType)
            {
                case ArrowType.leftUp:
                    {
                        this.Width = lastSize.Width - (t.X - this.screenPoint.X);
                        this.Height = lastSize.Height - (t.Y - this.screenPoint.Y);
                        this.Location = new Point(l.X, l.Y);
                        break;
                    }
                case ArrowType.leftDown:
                    {
                        this.Width = lastSize.Width - (t.X - this.screenPoint.X);
                        this.Height = lastSize.Height + (t.Y - this.screenPoint.Y);
                        this.Location = new Point(l.X, lastPoint.Y);
                        break;
                    }
                case ArrowType.rightUp:
                    {
                        this.Width = lastSize.Width + (t.X - this.screenPoint.X);
                        this.Height = lastSize.Height - (t.Y - this.screenPoint.Y);
                        this.Location = new Point(lastPoint.X, l.Y);
                        break;
                    }
                case ArrowType.rightDown:
                    {
                        this.Width = lastSize.Width + (t.X - this.screenPoint.X);
                        this.Height = lastSize.Height + (t.Y - this.screenPoint.Y);
                        break;
                    }
            }
            this.Refresh();

        }
        //箭头样式
        public enum ArrowType
        {
            leftUp,
            leftDown,
            rightUp,
            rightDown,
            move,
            none
        }
    }
}

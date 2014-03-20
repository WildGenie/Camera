using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace CameraCapture
{
    public partial class ScreenShotBase : Form
    {
        //mask调节遮盖（虚化）效果的透明度，透明度可调节
        private static SolidBrush mask = new SolidBrush(Color.FromArgb(180, 0, 0, 0));
        private Image baseImage;  //背景图片
        protected Rectangle selectedRect;  //背景框选择区域
        private Point mouseDownPoint;   //记录鼠标按下时的鼠标位置
        private Rectangle[] corners = new Rectangle[4];   //裁剪区域四个角的小矩形区域（画图时，填充为圆形）
        private int radius = 15;  //裁剪区域，四个角落小圆形的半径
        System.Drawing.SolidBrush dotBrush = new System.Drawing.SolidBrush(System.Drawing.Color.DarkGray);  //小圆形区域的填充色

        //截屏使用
        public ScreenShotBase()
        {
            InitializeComponent();
            baseImage = GetDestopImage();
            Image BackScreen = new Bitmap(baseImage);
            Graphics g = Graphics.FromImage(BackScreen);
            //画遮罩
            g.FillRectangle(mask, 0, 0, BackScreen.Width, BackScreen.Height);
            g.Dispose();
            //将有遮罩的图像作为背景
            this.BackgroundImage = BackScreen;
        }

        //相机裁剪使用
        public ScreenShotBase(Image backImg)
        {
            InitializeComponent();
            baseImage = backImg;
            Image BackScreen = new Bitmap(baseImage);
            Graphics g = Graphics.FromImage(BackScreen);
            //画遮罩
            g.FillRectangle(mask, 0, 0, BackScreen.Width, BackScreen.Height);
            g.Dispose();
            //将有遮罩的图像作为背景
            this.BackgroundImage = BackScreen;
        }

        private void ScreenShot_Load(object sender, EventArgs e)
        {
            
            SetStyle(ControlStyles.UserPaint 
                | ControlStyles.AllPaintingInWmPaint 
                | ControlStyles.OptimizedDoubleBuffer, true);  //开启双缓冲
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;  //去边框及全屏化
            this.Bounds = Screen.PrimaryScreen.Bounds;
            selectedRect = Rectangle.FromLTRB(this.Width / 4, this.Height / 4, 
                this.Width * 3 / 4, this.Height * 3 / 4);  //初始化裁剪区域位置
            this.TopMost = true; //置顶
        }
        public ArrowType arrowType; //控制鼠标形状
        private void calcArrowType(MouseEventArgs e)
        {
            if (Math.Abs(e.X - selectedRect.Left) <= radius && Math.Abs(e.Y - selectedRect.Top) <= radius)
            {
                this.arrowType = ArrowType.leftUp;//左上
                this.Cursor = Cursors.SizeNWSE;
            }
            else if (Math.Abs(e.X - selectedRect.Right) <= radius && Math.Abs(e.Y - selectedRect.Top) <= radius)
            {
                this.arrowType = ArrowType.rightUp;//右上
                this.Cursor = Cursors.SizeNESW;
            }
            else if (Math.Abs(e.X - selectedRect.Left) <= radius && Math.Abs(e.Y - selectedRect.Bottom) <= radius)
            {
                this.arrowType = ArrowType.leftDown;//左下
                this.Cursor = Cursors.SizeNESW;
            }
            else if (Math.Abs(e.X - selectedRect.Right) <= radius && Math.Abs(e.Y - selectedRect.Bottom) <= radius)
            {
                this.arrowType = ArrowType.rightDown;//右下
                this.Cursor = Cursors.SizeNWSE;
            }
            else if (selectedRect.Contains(e.Location))
            {
                this.arrowType = ArrowType.move;//中间移动
                this.Cursor = Cursors.SizeAll;
            }
            else
            {
                this.arrowType = ArrowType.none;
                this.Cursor = Cursors.Default;
            }
        }
       //画裁剪框的四个角
        private void setConrnerPoint()
        {
            //左上角
            corners[0] = new Rectangle(new Point(selectedRect.Left - radius, selectedRect.Top - radius), 
                new Size(radius * 2, radius * 2));
            //右上角
            corners[1] = new Rectangle(new Point(selectedRect.Right - radius, selectedRect.Top - radius), 
                new Size(radius * 2, radius * 2));
            //右下角
            corners[2] = new Rectangle(new Point(selectedRect.Right-radius, selectedRect.Bottom-radius),
                new Size(radius * 2, radius * 2));
            //左下角
            corners[3] = new Rectangle(new Point(selectedRect.Left - radius, selectedRect.Bottom - radius),
                new Size(radius * 2, radius * 2));
        }

        private bool isMove = false;
        private bool isResize = false;
        public bool isCropModel = true;  //提供给拍照裁剪用，提供裁剪全屏预览，而不是直接显示裁剪框
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
           
            Point cur_Point = e.Location;
            if (isMove)
            {
                //裁剪框移动功能
                selectedRect.Offset(cur_Point.X - mouseDownPoint.X, cur_Point.Y - mouseDownPoint.Y);
                if (selectedRect.Top < 0)
                {
                    selectedRect.Location = new Point(selectedRect.X, 0);
                }
                if (selectedRect.Left < 0)
                {
                    selectedRect.Location = new Point(0, selectedRect.Y);
                }
                if (selectedRect.Right > this.Width)
                {
                    selectedRect.Location = new Point(this.Width - selectedRect.Width, selectedRect.Y);
                }
                if (selectedRect.Bottom > this.Height)
                {
                    selectedRect.Location = new Point(selectedRect.X, this.Height - selectedRect.Height);
                }
                Refresh();
                mouseDownPoint = cur_Point;
                return;
            }
            else if (isResize)
            {
                reSize(e);
                return;
            }
            else
            {
                if (isCropModel)  //判定裁剪和非裁剪状态
                {
                    calcArrowType(e); //判断鼠标形状
                    if (e.Button == MouseButtons.Left | e.Button == MouseButtons.Right)
                    {
                        if (arrowType == ArrowType.move)
                        {
                            isMove = true;
                        }
                        else if (arrowType != ArrowType.none)
                        {
                            isResize = true;
                        }
                    }
                } 
            }
        }

        private void reSize(MouseEventArgs e)
        {
            Point cur_Point = e.Location;
            switch (arrowType)
            {
                    //根据鼠标形状判断如何改变大小
                case ArrowType.leftUp:
                    {
                        selectedRect.Width = oldSize.Width - (cur_Point.X - mouseDownPoint.X);
                        selectedRect.Height = oldSize.Height - (cur_Point.Y - mouseDownPoint.Y);
                        selectedRect.Location = cur_Point;
                        break;
                    }
                case ArrowType.leftDown:
                    {
                        selectedRect.Width  = oldSize.Width - (cur_Point.X - mouseDownPoint.X);
                        selectedRect.Height = oldSize.Height + (cur_Point.Y - mouseDownPoint.Y);
                        if (selectedRect.Width >= 0 && selectedRect.Height >= 0)
                        {
                            selectedRect.Location = new Point(cur_Point.X, selectedRect.Top);
                        }                      
                        break;
                    }
                case ArrowType.rightUp:
                    {
                        selectedRect.Width = oldSize.Width + (cur_Point.X - mouseDownPoint.X);
                        selectedRect.Height = oldSize.Height - (cur_Point.Y - mouseDownPoint.Y);
                        if (selectedRect.Width >= 0 && selectedRect.Height >= 0)
                        {
                            selectedRect.Location = new Point(selectedRect.Left, cur_Point.Y);
                        }           
                        break;
                    }
                case ArrowType.rightDown:
                    {
                        selectedRect.Width = oldSize.Width + (cur_Point.X - mouseDownPoint.X);
                        selectedRect.Height = oldSize.Height + (cur_Point.Y - mouseDownPoint.Y);
                        break;
                    }  
            }
            //控制裁剪框不超出屏幕范围
            if (selectedRect.Width < 0)
            {
                selectedRect.Width = 0;
            }
            if (selectedRect.Height < 0)
            {
                selectedRect.Height = 0;
            }
            this.Refresh();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isMove = false;
            isResize = false;
        }

        private Size oldSize;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            mouseDownPoint = e.Location;
            oldSize = selectedRect.Size;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            setConrnerPoint();
            Graphics oldg = e.Graphics;
            oldg.DrawImage(baseImage, selectedRect, selectedRect, GraphicsUnit.Pixel);  //画裁剪框内的内容，突出显示裁剪框，不会被虚化。
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            foreach (Rectangle item in corners)
            {
                e.Graphics.DrawEllipse(Pens.DarkGray, item);//画边角圆圈
                e.Graphics.FillEllipse(dotBrush, item);//圆圈填充
            }
            e.Graphics.DrawRectangle(Pens.DarkGray, selectedRect);  //绘制选择区域
        }
        /// <summary>
        /// 截取完整屏幕图片
        /// </summary>
        /// <returns></returns>
        private Image GetDestopImage()
        {
            Rectangle rect = Screen.GetBounds(this);
            Bitmap bmp = new Bitmap(
                rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);

            IntPtr gHdc = g.GetHdc();  //win32方法，获取画布设备上下文
            IntPtr deskHandle = NativeMethods.GetDesktopWindow();

            IntPtr dHdc = NativeMethods.GetDC(deskHandle);  //桌面画布设备上下文
            NativeMethods.BitBlt(
                gHdc,
                0,
                0,
                rect.Width,
                rect.Height,
                dHdc,
                0,
                0,
                NativeMethods.TernaryRasterOperations.SRCCOPY);  //将桌面画到创建的画布中
            NativeMethods.ReleaseDC(deskHandle, dHdc);
            g.ReleaseHdc(gHdc);
            return bmp;
        }

        public Image getSelectedArea()
        {
            int width = selectedRect.Width;
            int height = selectedRect.Height;
            int offsetX = selectedRect.Location.X;
            int offsetY = selectedRect.Location.Y;
            Bitmap croppedImg = new Bitmap(width, height);
            Bitmap temImg = (Bitmap)baseImage;
            using (Graphics g = Graphics.FromImage(croppedImg))
            {
                Rectangle resultRect = new Rectangle(0, 0, width, height);
                Rectangle sourceRect = new Rectangle(offsetX, offsetY, width, height);
                g.DrawImage(temImg, resultRect, sourceRect, GraphicsUnit.Pixel);
            }
            return croppedImg;
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

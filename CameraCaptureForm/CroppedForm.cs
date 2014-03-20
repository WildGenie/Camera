using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CameraCapture
{
    /// <summary>
    /// 继承ScreenShotBase窗口，用于拍照后的裁剪
    /// </summary>
    public partial class CroppedForm : ScreenShotBase
    {
        private CameraButtons ok_Btn;
        private CameraButtons cropped_Btn;
        private CameraButtons cancelled_Btn;
        private CameraCaptureForm camera;  //拍照窗口，两个窗口通信用


        public CroppedForm()
        {
            InitializeComponent();
        }
        //继承ScreenShotBase的构造函数，将img设置为背景
        public CroppedForm(Image img, CameraCaptureForm form):base(img)
        {
            this.camera = form;
            InitializeComponent();
        }

        private void CroppedForm_Load(object sender, EventArgs e)
        {
            //裁剪框全屏，隐藏裁剪框
            base.selectedRect = Rectangle.FromLTRB(0, 0, this.Width, this.Height);
            //非裁剪模式，裁剪框不可移动、变换尺寸
            base.isCropModel = false;
            initPreviewUI();   //初始化按钮
        }

        public void initPreviewUI()
        {
            //裁剪按钮
            if (cropped_Btn == null)
            {
                this.cropped_Btn = new CameraButtons();
            }
            this.Controls.Add(cropped_Btn);
            cropped_Btn.BackIcon = Properties.Resources._7;    
            cropped_Btn.Size = cropped_Btn.getSize();
            cropped_Btn.Location = new System.Drawing.Point(this.Width - cropped_Btn.Width*8, this.Height - cropped_Btn.Height*3/2);
            cropped_Btn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            //确定按钮
            if (ok_Btn == null)
            {
                this.ok_Btn = new CameraButtons();
            }
            this.Controls.Add(ok_Btn);
            ok_Btn.BackIcon = Properties.Resources._3;
            ok_Btn.Location = new System.Drawing.Point(this.Width - cropped_Btn.Width * 6, this.Height - cropped_Btn.Height*3/2);
            ok_Btn.Size = ok_Btn.getSize();
            ok_Btn.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;

            //取消按钮
            if (cancelled_Btn == null)
            {
                this.cancelled_Btn = new CameraButtons();
            }
            this.Controls.Add(cancelled_Btn);
            cancelled_Btn.BackIcon = Properties.Resources._4;
            cancelled_Btn.Location = new System.Drawing.Point(this.Width - cropped_Btn.Width * 4, this.Height - cropped_Btn.Height*3/2);
            cancelled_Btn.Size = cancelled_Btn.getSize();
            cancelled_Btn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            this.ok_Btn.MouseClick += OK_Click;
            this.cropped_Btn.MouseClick += crop_Click;
            this.cancelled_Btn.MouseClick += cancelled_Click;
         }

        private void cancelled_Click(object sender, MouseEventArgs e)
        {
            if (camera != null)
            {
                this.Dispose();

                camera.startCameraCapture(); //重新启动拍照
                camera.Show();
                this.Close();
            }
        }

        private void crop_Click(object sender, MouseEventArgs e)
        {
            //裁剪模式和非裁剪模式切换
            if (!base.isCropModel)
            {
                selectedRect = Rectangle.FromLTRB(this.Width / 4, this.Height / 4,
                this.Width * 3 / 4, this.Height * 3 / 4);
                base.isCropModel = true;
                cropped_Btn.BackIcon = Properties.Resources._7_2; 
            }
            else
            {
                base.selectedRect = Rectangle.FromLTRB(0, 0, this.Width, this.Height);
                base.arrowType = ArrowType.none;
                base.isCropModel = false;
                cropped_Btn.BackIcon = Properties.Resources._7;
            }
            this.Refresh();
        }

        private void OK_Click(object sender, MouseEventArgs e)
        {
            //此处可以返回裁剪所得图片croppedImage
            Image croppedImage = getSelectedArea();
            
            //***测试用代码，可以删除
            croppedImage.Save(System.IO.Path.GetFullPath("../../tempImg")+"\\"+"temp.jpg", ImageFormat.Jpeg);
            //*****
            if (camera != null)
            {
                camera.Dispose();
            }
            this.Dispose();
        }   
    }
}

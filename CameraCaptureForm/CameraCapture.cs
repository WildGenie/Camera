using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using AForge.Video.DirectShow;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing.Imaging;


namespace CameraCapture
{
    public partial class CameraCaptureForm : Form
    {
        private FilterInfoCollection videoDevices;  //枚举视频设备
        private VideoCaptureDevice videoSource;  //为选定的视频设备穿件视频源，采集画面
        private int currentVideoDeviceIndex = 0;
        private CameraButtons return_Btn;
        private CameraButtons cameraSwitched_Btn;  //切换相机按钮

        public CameraCaptureForm()
        {       
            InitializeComponent(); 
        }


        private void CameraCaptureForm_Load(object sender, EventArgs e)
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor
            | ControlStyles.UserPaint, true);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            initCaptureUI();
            this.videoSourcePlayer.SendToBack();  //将此控件至于底层
            this.Bounds = Screen.PrimaryScreen.Bounds;
            startCameraCapture();
        }

        //自定义透明控件上添加按钮，避免屏幕抖动
        TransparencyPanel transparencyPanel;
        public void initCaptureUI()
        {
            if (transparencyPanel == null)
            {
                 transparencyPanel = new TransparencyPanel();
            }
            transparencyPanel.Dock = DockStyle.Fill;
            transparencyPanel.Parent = this.videoSourcePlayer;
            this.videoSourcePlayer.Controls.Add(transparencyPanel);
            transparencyPanel.BringToFront();

            //返回按钮
            if (return_Btn == null)
            {
                this.return_Btn = new CameraButtons();
            }
            //return_Btn.Parent = this.transparencyPanel;
            this.transparencyPanel.Controls.Add(return_Btn);
            
            return_Btn.BackIcon = Properties.Resources._9;
            return_Btn.Location = new System.Drawing.Point(48, 29);
            return_Btn.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            return_Btn.Size = return_Btn.getSize();

            //切换相机
            if (cameraSwitched_Btn == null)
            {
                this.cameraSwitched_Btn = new CameraButtons();
            }
             // videoSourcePlayer.Controls.Add(cameraSwitched_Btn);
           this.transparencyPanel.Controls.Add(cameraSwitched_Btn);
            cameraSwitched_Btn.BackIcon = Properties.Resources._8;
            cameraSwitched_Btn.Location = new System.Drawing.Point(210, 389);
            cameraSwitched_Btn.Size = cameraSwitched_Btn.getSize();
            cameraSwitched_Btn.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;

            //添加事件
            this.return_Btn.MouseClick += return_Btn_Click;
            this.cameraSwitched_Btn.MouseClick += CameraSwitched_Click;
            this.transparencyPanel.MouseClick += videoSourcePlayer_Click;
        }

        /// <summary>
        /// 初始化，连接摄像头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void startCameraCapture()
        {
            videoSourcePlayer.Visible = true;
            try
            {
                //枚举视频设备
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                //创建视频源
                videoSource = new VideoCaptureDevice(videoDevices[currentVideoDeviceIndex].MonikerString);
                //重要参数，设置设置分辨率VideoCapabilities属性能获取到分辨率和帧率的一个列表
                //旧版本利用desireFrameRate和desireFrameSize来设置
                //新版本不提供对旧版本的支持，采用VideoResolution属性进行设置
                //此处修改，可提供选择分辨率功能。暂时只获取最高分辨率
                videoSource.VideoResolution = videoSource.VideoCapabilities[videoSource.VideoCapabilities.Length - 1];
                this.videoSourcePlayer.VideoSource = videoSource;
                videoSourcePlayer.Start();
            }
            catch (Exception)
            {
                //此处异常可能为没有视频设备，或者没有可支持的分辨率
                MessageBox.Show("没有可用视频设备");
                this.Dispose();
                this.Close();
            }
        }

        /// <summary>
        /// 关闭摄像头，退出拍照界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void return_Btn_Click(object sender, EventArgs e)
        {
            if (videoSourcePlayer.IsRunning == true)
            {
                videoSourcePlayer.SignalToStop();
                videoSourcePlayer.WaitForStop();
            }

            videoSourcePlayer.Dispose();
            videoSourcePlayer = null;

            this.Dispose();
            this.Close();
        }

        //单击拍照区域拍照
        private void videoSourcePlayer_Click(object sender, EventArgs e)
        {
            try
            {
                if (videoSourcePlayer.IsRunning)
                {
                    Bitmap curPic = videoSourcePlayer.GetCurrentVideoFrame();
                    //拍照完成后关摄像头
                    if (videoSourcePlayer != null && videoSourcePlayer.IsRunning)
                    {
                        videoSourcePlayer.SignalToStop();
                        videoSourcePlayer.WaitForStop();
                    }
                    //这里讲图片设置成和屏幕一样大
                    curPic = new Bitmap(curPic, new System.Drawing.Size(this.Width, this.Height));
                    //裁剪窗口，初始化时默认裁剪框全屏
                    //点击裁剪按钮后裁剪框缩小，并出现在中间
                    CroppedForm cropForm = new CroppedForm(curPic, this);
                    cropForm.Show();
                    this.Hide();
                }
            }
            catch (Exception ecp)
            {
                MessageBox.Show(ecp.ToString());
            }
        }

        //切换相机
        private void CameraSwitched_Click(object sender, EventArgs e)
        {
            int camera_nums = videoDevices.Count;  //可用摄像头设备的数目
            currentVideoDeviceIndex++;
            if(currentVideoDeviceIndex >= camera_nums)
            {
                currentVideoDeviceIndex = 0;
            }
            if (videoSourcePlayer != null && videoSourcePlayer.IsRunning)
            {
                videoSourcePlayer.SignalToStop();
                videoSourcePlayer.WaitForStop();
            }
            startCameraCapture();
        }
    }
}

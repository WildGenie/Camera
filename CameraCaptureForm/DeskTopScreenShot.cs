using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CameraCapture
{
    /// <summary>
    /// 继承ScreenShotBase，用于桌面截图
    /// </summary>
    public partial class DeskTopScreenShot : ScreenShotBase
    {
        private CameraButtons cancel;
        private CameraButtons clipboard;
        private CameraButtons cur_Page;
        private string imagePath;
        public string ImagePath 
        { 
            get
            {
                return this.imagePath;
            }
        }

        public DeskTopScreenShot()
        {
            InitializeComponent();
        }

        private void DeskTopScreenShot_Load(object sender, EventArgs e)
        {
            initialButtons();
        }
        public void initialButtons()
        {
            //初始化按钮
            cancel = new CameraButtons();
            this.Controls.Add(cancel);
            cancel.BackIcon = Properties.Resources.Cancel;
            cancel.Size = cancel.getSize();
            cancel.Location = new Point(this.Width - cancel.Width * 4, this.Height - cancel.Height * 3 / 2);
            cancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            clipboard = new CameraButtons();
            this.Controls.Add(clipboard);
            clipboard.BackIcon = Properties.Resources.Clipboard;
            clipboard.Size = clipboard.getSize();
            clipboard.Location = new Point(this.Width - cancel.Width * 6, this.Height - cancel.Height * 3 / 2);
            clipboard.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            cur_Page = new CameraButtons();
            this.Controls.Add(cur_Page);
            cur_Page.BackIcon = Properties.Resources.Cur_Page;
            cur_Page.Size = cur_Page.getSize();
            cur_Page.Location = new Point(this.Width - cancel.Width * 8, this.Height - cancel.Height * 3 / 2);
            cur_Page.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            //注册鼠标单击事件
            this.cancel.MouseClick += cancel_Click;
            this.clipboard.MouseClick += clipboard_Click;
            this.cur_Page.MouseClick += cur_Page_Click;
        }
         //当前页，将图片插入当前页

        private void cur_Page_Click(object sender, MouseEventArgs e)
        {
            //当前裁剪区域图片在此处获得
            Image cropped_Image = getSelectedArea();
            
        }
        //剪贴板，图片传给系统剪贴板
        private void clipboard_Click(object sender, MouseEventArgs e)
        {
            Image cropped_Image = getSelectedArea();
            Clipboard.Clear();
            Clipboard.SetImage(cropped_Image);
            cropped_Image.Dispose();  //图片资源释放
            this.Dispose();
        }
        
        //取消，退出截屏
        private void cancel_Click(object sender, MouseEventArgs e)
        {
            this.Dispose();
        }
    }
}

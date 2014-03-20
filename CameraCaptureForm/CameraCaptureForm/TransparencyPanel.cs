using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace CameraCapture
{
    public partial class TransparencyPanel : UserControl
    {     
        public TransparencyPanel()
        {
            InitializeComponent();
            //设置Style支持透明背景色
            this.SetStyle(ControlStyles.SupportsTransparentBackColor 
                | ControlStyles.UserPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.AllPaintingInWmPaint, true);
            this.BackColor = Color.FromArgb(1, 0, 0, 0);  
        }
    }
}

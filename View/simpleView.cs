using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp3.View
{
    public partial class simpleView : UIPage
    {
        public simpleView()
        {
            InitializeComponent();
        }

        private void simpleView_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 由 mainView 创建 8 张卡片时，告诉本卡片自己是第几通道。
        /// 设计器里 uiTextBox1.Text 写死成「通道」，8 张卡片就完全一样了。
        /// </summary>
        public void SetChannelNumber(int channelNo)
        {
            // 设计器给的宽度是 45px，只够「通道」两个字；加上数字要加宽，
            // 否则高 DPI 或字体放大时「通道8」会被裁掉。
            uiTextBox1.Width = 55;
            uiTextBox1.Text = $"通道{channelNo}";
        }

        //刷新simpleView中的数据（由采集后台线程调用）
        public void refreshData(ushort data)
        {
            //重要：在异步任务里面，如果我们需要对控件进行一个更改，要把它抛出到 UI 线程才行。
            //切页 / 关闭窗体时句柄可能正在销毁，直接 Invoke 会抛 InvalidOperationException，
            //甚至因为 Invoke 是同步等待而拖住程序退出，所以先校验、再用 BeginInvoke 投递。
            if (IsDisposed || !IsHandleCreated) return;

            try
            {
                Action update = () =>
                {
                    if (!IsDisposed) uiTextBox7.Text = data.ToString();
                };

                if (InvokeRequired) BeginInvoke(update);
                else update();
            }
            // ObjectDisposedException 继承自 InvalidOperationException，一个 catch 全覆盖
            catch (InvalidOperationException) { /* 句柄在投递瞬间被销毁，跳过本帧 */ }
        }
    }
}

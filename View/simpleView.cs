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

        /// <summary>
        /// 用换算后的物理量刷新卡片，并同步合格/不合格指示灯。由采集后台线程调用。
        /// ok 为 null 表示尚未标定，灯用灰色，不误报合格。
        /// </summary>
        public void refreshQuality(double value, bool? ok)
        {
            PostToUi(() =>
            {
                uiTextBox7.Text = value.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
                uiTextBox3.Text = ok switch
                {
                    true => "合格",
                    false => "不合格",
                    null => "未标定"
                };
                uiTextBox3.ForeColor = ok switch
                {
                    true => Color.Green,
                    false => Color.Red,
                    null => Color.Gray
                };
                uiLedBulb1.Color = ok switch
                {
                    true => Color.LimeGreen,
                    false => Color.Red,
                    null => Color.Gray
                };
                uiLedBulb2.Color = Color.Gray;   // 第二个灯保留给报警闪烁，暂不启用
            });
        }

        /// <summary>保留旧接口：只刷新原始计数（无判定时使用）。</summary>
        public void refreshData(ushort data) => PostToUi(() =>
            uiTextBox7.Text = data.ToString(System.Globalization.CultureInfo.InvariantCulture));

        /// <summary>
        /// 把控件更新投递回 UI 线程。切页 / 关闭窗体时句柄可能正在销毁，
        /// 直接 Invoke 会抛 InvalidOperationException，甚至拖住程序退出，
        /// 所以先校验、再用 BeginInvoke 投递。
        /// </summary>
        private void PostToUi(Action action)
        {
            if (IsDisposed || !IsHandleCreated) return;
            try
            {
                Action guarded = () => { if (!IsDisposed) action(); };
                if (InvokeRequired) BeginInvoke(guarded);
                else guarded();
            }
            // ObjectDisposedException 继承自 InvalidOperationException，一个 catch 全覆盖
            catch (InvalidOperationException) { /* 句柄在投递瞬间被销毁，跳过本帧 */ }
        }
    }
}

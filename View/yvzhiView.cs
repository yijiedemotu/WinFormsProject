using Sunny.UI;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
using TICP.Model;
using TICP.Common;
using TICP.Helper;

namespace WinFormsApp3.View
{
    public partial class yvzhiView : UIPage
    {
        private int index = 1;                 // 当前通道 1~8
        private UIPanel[] channelPanels = null!;
        private UITextBox[] boxes = null!;
        private (UITextBox set, UITextBox? current, UIButton apply)[] knives = null!;
        private Timer refreshTimer = null!;

        public yvzhiView()
        {
            InitializeComponent();
            yuzhiCommon.Load();

            channelPanels = new[] { uiPanel5, uiPanel6, uiPanel7, uiPanel8,
                                    uiPanel9, uiPanel10, uiPanel11, uiPanel12 };
            boxes = new[] { uiTextBox2, uiTextBox3, uiTextBox4, uiTextBox5, uiTextBox6, uiTextBox7 };
            knives = new (UITextBox set, UITextBox? current, UIButton apply)[]
            {
                (uiTextBox8,  null,        uiButton2),
                (uiTextBox9,  uiTextBox10, uiButton3),
                (uiTextBox12, uiTextBox11, uiButton4),
                (uiTextBox14, uiTextBox13, uiButton5),
            };

            for (int i = 0; i < channelPanels.Length; i++)
            {
                int n = i + 1;
                channelPanels[i].Click += (s, e) => SelectChannel(n);
            }
            for (int i = 0; i < knives.Length; i++)
            {
                int n = i;
                knives[i].apply.Enabled = true;      // Designer 里被设成 false
                knives[i].apply.Click += (s, e) => ApplyKnife(n, true);
            }

            uiButton1.Click += uiButton1_Click;      // 确认：保存当前通道标定/阈值
            uiButton6.Click += uiButton6_Click;      // 确认刀补

            SelectChannel(1);
            LoadKnifeUi();

            // 传感器当前值：直接读 mainCommon.data（mainView 采集线程每秒写入）
            refreshTimer = new Timer { Interval = 1000 };
            refreshTimer.Tick += (s, e) =>
            {
                if (IsDisposed || !IsHandleCreated) return;
                if (index >= 1 && index <= 8)
                    uiTextBox1.Text = Volatile.Read(ref mainCommon.data[index - 1])
                        .ToString(CultureInfo.InvariantCulture);
            };
            // 只在页面真正释放时停表：切页会重建句柄并触发 HandleDestroyed，
            // 用 HandleDestroyed 停掉后，切回来定时器就再也不会启动了。
            Disposed += (s, e) => { refreshTimer.Stop(); refreshTimer.Dispose(); };
            refreshTimer.Start();
        }

        // Designer 里已绑定这两个事件，保留方法避免编译错误
        private void yvzhiView_Load(object sender, EventArgs e) { }
        private void uiPanel5_Click(object sender, EventArgs e) { }

        private void SelectChannel(int n)
        {
            index = n;
            var m = yuzhiCommon.Current.channels[index - 1];
            uiTextBox2.Text = m.liangjuUp;      // 量具标准件上限
            uiTextBox3.Text = m.liangjuDown;    // 量具标准件下限
            uiTextBox4.Text = m.jiaozhunUp;     // 校准量具上限
            uiTextBox5.Text = m.jiaozhunDown;   // 校准量具下限
            uiTextBox6.Text = m.celiangUp;      // 测量上限
            uiTextBox7.Text = m.celiangDown;    // 测量下限

            for (int i = 0; i < channelPanels.Length; i++)
                channelPanels[i].ForeColor = i == index - 1
                    ? Color.RoyalBlue : Color.FromArgb(48, 48, 48);
        }

        private static bool TryNum(UITextBox box, out double v) =>
            double.TryParse(box.Text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out v);

        private bool CheckUpDown(UITextBox up, UITextBox down, string name)
        {
            if (TryNum(up, out double hi) && TryNum(down, out double lo) && hi <= lo)
            {
                MessageBox.Show($"{name}上限必须大于下限", "输入错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private bool ValidateInput()
        {
            foreach (var box in boxes)
            {
                string t = box.Text.Trim();
                if (t.Length == 0) continue;         // 空 = 未标定，允许
                if (!double.TryParse(t, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
                {
                    MessageBox.Show($"「{t}」不是合法数字", "输入错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    box.Focus();
                    return false;
                }
            }
            return CheckUpDown(uiTextBox2, uiTextBox3, "量具标准件")
                && CheckUpDown(uiTextBox4, uiTextBox5, "校准量具")
                && CheckUpDown(uiTextBox6, uiTextBox7, "测量");
        }

        private void uiButton1_Click(object? sender, EventArgs e)
        {
            if (!ValidateInput()) return;
            var m = new yuzhiModel
            {
                liangjuUp = uiTextBox2.Text.Trim(),
                liangjuDown = uiTextBox3.Text.Trim(),
                jiaozhunUp = uiTextBox4.Text.Trim(),
                jiaozhunDown = uiTextBox5.Text.Trim(),
                celiangUp = uiTextBox6.Text.Trim(),
                celiangDown = uiTextBox7.Text.Trim()
            };
            m.Normalize();
            yuzhiCommon.Current.channels[index - 1] = m;
            if (SaveOrWarn())
                MessageBox.Show($"通道{index} 参数已保存。", "完成",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LoadKnifeUi()
        {
            var c = yuzhiCommon.Current;
            for (int i = 0; i < knives.Length; i++)
            {
                knives[i].set.Text = c.knifeSet[i];
                if (knives[i].current != null) knives[i].current!.Text = c.knifeCurrent[i];
            }
            uiTextBox17.Text = c.knifeMaxUp;        // 补偿最大上限
            uiTextBox16.Text = c.knifeMaxDown;      // 补偿最大下限
        }

        private bool ApplyKnife(int i, bool save)
        {
            var g = knives[i];
            if (!TryNum(g.set, out double v))
            {
                MessageBox.Show($"第{i + 1}组补偿设定值不是合法数字", "输入错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if ((TryNum(uiTextBox16, out double min) && v < min) ||
                (TryNum(uiTextBox17, out double max) && v > max))
            {
                MessageBox.Show($"第{i + 1}组补偿值超出允许范围", "输入错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            string text = v.ToString(CultureInfo.InvariantCulture);
            if (g.current != null) g.current.Text = text;
            yuzhiCommon.Current.knifeSet[i] = text;
            yuzhiCommon.Current.knifeCurrent[i] = text;
            return !save || SaveOrWarn();
        }

        private void uiButton6_Click(object? sender, EventArgs e)
        {
            for (int i = 0; i < knives.Length; i++)
                if (!ApplyKnife(i, false)) return;

            if (TryNum(uiTextBox16, out double min) && TryNum(uiTextBox17, out double max) && min > max)
            {
                MessageBox.Show("补偿最大下限不能大于最大上限", "输入错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            yuzhiCommon.Current.knifeMaxDown = uiTextBox16.Text.Trim();
            yuzhiCommon.Current.knifeMaxUp = uiTextBox17.Text.Trim();
            if (SaveOrWarn())
                MessageBox.Show("刀补参数已保存。", "完成",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool SaveOrWarn()
        {
            try { yuzhiCommon.Save(); return true; }
            catch (Exception ex)
            {
                Log.Error("保存 yuzhi.json 失败", ex);
                MessageBox.Show("保存失败：" + ex.Message, "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}

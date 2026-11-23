using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using WinFormsApp3.Common;
using WinFormsApp3.Model;

namespace WinFormsApp3.View
{
    public partial class plotView : UIPage
    {
        public plotView()
        {
            InitializeComponent();

            // 通道下拉框原本没有任何候选项
            uiComboBox1.Items.Clear();
            for (int i = 1; i <= 8; i++) uiComboBox1.Items.Add(i.ToString());
            uiComboBox1.SelectedIndex = 0;
            SetDefaultDateRange();

            // Designer 里「查询」按钮没有绑定 Click，这里在代码中订阅
            uiButton1.Click += uiButton1_Click;
        }

        /// <summary>设计器把日期写死在 2026-11-12，改成库里真实的数据范围。</summary>
        private void SetDefaultDateRange()
        {
            try
            {
                var (min, max) = mainCommon.sqlHelper.GetTimeRange();
                if (min != null) uiDatePicker1.Value = min.Value.Date;
                if (max != null) uiDatePicker2.Value = max.Value.Date;
            }
            catch (Exception ex)
            {
                gkHelper.Log.Error("设置曲线默认日期失败", ex);
            }
        }

        // 首次打开页面就画一次，避免曲线区一片空白
        private void plotView_Load(object sender, EventArgs e) => LoadPlot();

        private void uiButton1_Click(object? sender, EventArgs e) => LoadPlot();

        /// <summary>按日期区间 + 通道，把 data1..data8 画成折线。</summary>
        private void LoadPlot()
        {
            try
            {
                const string f = "yyyy-MM-dd";
                string start = uiDatePicker1.Value.ToString(f, CultureInfo.InvariantCulture);
                string end = uiDatePicker2.Value.ToString(f, CultureInfo.InvariantCulture);
                var rows = mainCommon.sqlHelper.GetByConditions(start, end, "");

                if (!int.TryParse(uiComboBox1.Text.Trim(), out int ch) || ch < 1 || ch > 8) ch = 1;

                var xs = new List<double>();
                var ys = new List<double>();
                foreach (var r in rows)
                {
                    if (!DateTime.TryParseExact(r.testTime, "yyyy-MM-dd HH:mm:ss",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime t)) continue;

                    string raw = ChannelValue(r, ch);
                    if (!double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out double v)) continue;

                    xs.Add(t.ToOADate());   // ScottPlot 时间轴用 OLE 自动化日期数值
                    ys.Add(v);
                }

                formsPlot1.Plot.Clear();
                if (xs.Count > 0)
                {
                    var line = formsPlot1.Plot.Add.Scatter(xs.ToArray(), ys.ToArray());
                    line.LegendText = $"通道{ch}";
                    formsPlot1.Plot.Axes.DateTimeTicksBottom();
                }
                formsPlot1.Plot.Title(xs.Count == 0 ? "该条件下无数据" : $"通道{ch} 趋势（{xs.Count} 个点）");
                formsPlot1.Plot.XLabel("检测时间");
                formsPlot1.Plot.YLabel("测量值");
                formsPlot1.Refresh();
                gkHelper.Log.Info($"趋势曲线绘制完成：通道{ch}，{xs.Count} 个点，区间 {start} ~ {end}");
            }
            catch (Exception ex)
            {
                // 曲线是可选功能：失败只记日志并在标题提示，不弹窗打断其它页面
                gkHelper.Log.Error("绘制趋势曲线失败", ex);
                try
                {
                    formsPlot1.Plot.Title("绘制失败：" + ex.Message);
                    formsPlot1.Refresh();
                }
                catch { }
            }
        }

        /// <summary>按通道号取出对应的字符串数据。</summary>
        private static string ChannelValue(dataModel m, int ch) => ch switch
        {
            1 => m.data1, 2 => m.data2, 3 => m.data3, 4 => m.data4,
            5 => m.data5, 6 => m.data6, 7 => m.data7, 8 => m.data8,
            _ => ""
        };
    }
}

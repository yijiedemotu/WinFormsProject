using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using TICP.Common;
using TICP.Helper;
using TICP.Model;

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
                Log.Error("设置曲线默认日期失败", ex);
            }
        }

        // 首次打开页面就画一次，避免曲线区一片空白
        private void plotView_Load(object sender, EventArgs e) => LoadPlot();

        private void uiButton1_Click(object? sender, EventArgs e) => LoadPlot();

        /// <summary>
        /// 修复中文变方框（豆腐块）：ScottPlot 5 默认字体是 Segoe UI，字形表里没有汉字，
        /// 所以标题、轴标签、图例里的中文全部被画成 □。
        /// Fonts.Detect(中文样本) 会挑一个系统里真正支持这套字符的字体
        /// （中文 Windows 实测返回 Microsoft YaHei UI）；
        /// Plot.Font.Set 会把它应用到标题/轴标签，并改掉全局默认字体，
        /// 之后新增的 plottables 也自动用这个字体。
        /// </summary>
        private static void ApplyChineseFont(ScottPlot.Plot plot)
        {
            string font = ScottPlot.Fonts.Detect("通道趋势测量值检测时间");
            plot.Font.Set(font);
            plot.Legend.FontName = font;   // 图例字体是独立的，单独指定一次
        }

        /// <summary>按日期区间 + 通道，把 data1..data8 画成折线。</summary>
        private void LoadPlot()
        {
            try
            {
                // 每次重绘前统一字体：Clear() 只清 plottables，字体设置不会丢，
                // 但放在这里最稳妥，也方便以后换字体只改一处。
                ApplyChineseFont(formsPlot1.Plot);

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
                Log.Info($"趋势曲线绘制完成：通道{ch}，{xs.Count} 个点，区间 {start} ~ {end}");
            }
            catch (Exception ex)
            {
                // 曲线是可选功能：失败只记日志并在标题提示，不弹窗打断其它页面
                Log.Error("绘制趋势曲线失败", ex);
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

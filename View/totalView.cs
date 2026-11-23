using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TICP.Common;
using TICP.Helper;
using TICP.Model;

namespace WinFormsApp3.View
{
    public partial class totalView : UIPage
    {
        //用于绑定dataGridView1的数据源，BindingList是一个可绑定的列表，支持数据绑定和通知更改
        private BindingList<dataModel> dataModels = new BindingList<dataModel>();

        // 统计标签与两列判定信息都在代码里动态创建，避免改 Designer 的自动生成代码
        private UILabel statsLabel = new UILabel();
        private DataGridViewTextBoxColumn resultColumn = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn ngColumn = new DataGridViewTextBoxColumn();

        public totalView()
        {
            InitializeComponent();
            SetupExtraColumns();
            SetupStatsLabel();

            dataGridView1.AllowUserToAddRows = false;   // 查询页只读，不需要新增行
            dataGridView1.DataSource = dataModels;      // 绑定数据源
            FillComboBoxes();                          // 下拉框先有内容，用户才有得选
            SetDefaultDateRange();                     // 默认区间覆盖库里已有数据
            InitialLoadData();
            RefreshStats();
        }

        /// <summary>在表格右侧追加「判定 / 不合格通道」两列，绑定 dataModel 的新字段。</summary>
        private void SetupExtraColumns()
        {
            resultColumn.Name = "colResult";
            resultColumn.HeaderText = "判定";
            resultColumn.DataPropertyName = "result";
            resultColumn.Width = 70;
            ngColumn.Name = "colNgChannels";
            ngColumn.HeaderText = "不合格通道";
            ngColumn.DataPropertyName = "ngChannels";
            ngColumn.Width = 90;
            dataGridView1.Columns.Add(resultColumn);
            dataGridView1.Columns.Add(ngColumn);
        }

        /// <summary>顶部工具栏右侧放一个统计标签，显示当前查询条件下的合格率。</summary>
        private void SetupStatsLabel()
        {
            statsLabel.Font = new Font("宋体", 11F, FontStyle.Bold, GraphicsUnit.Point);
            statsLabel.ForeColor = Color.FromArgb(48, 48, 48);
            statsLabel.Location = new Point(875, 21);
            statsLabel.Size = new Size(170, 35);
            statsLabel.TextAlign = ContentAlignment.MiddleLeft;
            statsLabel.Text = "统计：—";
            uiPanel1.Controls.Add(statsLabel);
        }

        /// <summary>用与查询完全相同的条件统计，避免「表格和统计对不上」。</summary>
        private void RefreshStats()
        {
            try
            {
                const string f = "yyyy-MM-dd";
                var (total, ng, rate) = mainCommon.sqlHelper.GetStats(
                    uiDatePicker1.Value.ToString(f, CultureInfo.InvariantCulture),
                    uiDatePicker2.Value.ToString(f, CultureInfo.InvariantCulture),
                    uiComboBox1.Text.Trim());
                statsLabel.Text = total == 0
                    ? "统计：无数据"
                    : $"共 {total} 条 / NG {ng} / 合格率 {rate:F1}%";
                statsLabel.ForeColor = ng > 0 ? Color.Firebrick : Color.SeaGreen;
            }
            catch (Exception ex)
            {
                Log.Error("统计合格率失败", ex);
                statsLabel.Text = "统计：失败";
            }
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
                Log.Error("设置查询默认日期失败", ex);
            }
        }

        /// <summary>产品名称取数据库里已有的值；通道固定 1~8。</summary>
        private void FillComboBoxes()
        {
            uiComboBox2.Items.Clear();
            for (int i = 1; i <= 8; i++) uiComboBox2.Items.Add(i.ToString());

            uiComboBox1.Items.Clear();
            uiComboBox1.Items.Add("");                 // 空 = 不限产品名称
            try
            {
                foreach (string name in mainCommon.sqlHelper.GetProjectNames())
                    uiComboBox1.Items.Add(name);
            }
            catch (Exception ex)
            {
                Log.Error("加载产品名称下拉框失败", ex);
            }
        }

        private void InitialLoadData()
        {
            try
            {
                var datas = mainCommon.sqlHelper.GetAll();
                dataModels.Clear();
                foreach (var item in datas)
                {
                    dataModels.Add(item);
                }
            }
            catch (Exception ex)
            {
                // 启动阶段不让数据库异常拖垮整个窗体
                Log.Error("查询统计页加载历史数据失败", ex);
            }
        }

        //点击查询按钮，按条件查询并显示
        private void uiButton1_Click(object sender, EventArgs e)
        {
            // 只传 10 位日期：SqlHelper.NormalizeTime 会补成
            // 起始 00:00:00、结束 23:59:59，保证「结束日期当天」的数据也在范围内
            const string f = "yyyy-MM-dd";
            string start = uiDatePicker1.Value.ToString(f, CultureInfo.InvariantCulture);
            string end = uiDatePicker2.Value.ToString(f, CultureInfo.InvariantCulture);

            try
            {
                var datas = mainCommon.sqlHelper.GetByConditions(start, end, uiComboBox1.Text.Trim());
                dataModels.Clear();
                foreach (var item in datas)
                {
                    dataModels.Add(item);
                }

                // int.TryParse 代替 Convert.ToInt16：下拉框里出现非数字时不会抛异常
                if (int.TryParse(uiComboBox2.Text.Trim(), out int ch) && ch >= 1 && ch <= 8)
                    ShowOnlySpecifiedColumns(dataGridView1, "Column1", "Column2", "Column3",
                        $"Column{ch + 3}", "colResult", "colNgChannels");
                else
                    ShowAllColumns(dataGridView1);

                RefreshStats();
            }
            catch (Exception ex)
            {
                // 数据库被占用 / 文件损坏时只提示，别让一次查询把整个界面带崩
                Log.Error("按条件查询历史数据失败", ex);
                MessageBox.Show("查询失败：" + ex.Message, "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>只显示指定的列，其余列全部隐藏</summary>
        public static void ShowOnlySpecifiedColumns(DataGridView dataGridView, params string[] visibleColumnNames)
        {
            if (dataGridView == null || visibleColumnNames == null) return;

            var namesToShow = new HashSet<string>(visibleColumnNames, StringComparer.OrdinalIgnoreCase);
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.Visible = namesToShow.Contains(column.Name);
            }
        }

        /// <summary>显示 DataGridView 中的所有列</summary>
        public static void ShowAllColumns(DataGridView dataGridView)
        {
            if (dataGridView == null) return;
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.Visible = true;
            }
        }

        /// <summary>
        /// 导出当前查询结果（含判定列）为 CSV。带 BOM 的 UTF-8，Excel 双击打开中文不乱码。
        /// 没有引 Excel 互操作库：CSV 更简单、无 Office 依赖、工控机上更好用。
        /// </summary>
        private void uiButton2_Click(object sender, EventArgs e)
        {
            if (dataModels.Count == 0)
            {
                MessageBox.Show("当前没有可导出的数据，请先查询。", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var dialog = new SaveFileDialog
            {
                Filter = "CSV 文件 (*.csv)|*.csv",
                FileName = $"检测数据_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                Title = "导出当前查询结果"
            };
            if (dialog.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("序号,检测时间,产品名称,通道1,通道2,通道3,通道4,通道5,通道6,通道7,通道8,判定,不合格通道");
                foreach (var m in dataModels)
                {
                    sb.Append(string.Join(",",
                        Csv(m.Id.ToString()), Csv(m.testTime), Csv(m.projectName),
                        Csv(m.data1), Csv(m.data2), Csv(m.data3), Csv(m.data4),
                        Csv(m.data5), Csv(m.data6), Csv(m.data7), Csv(m.data8),
                        Csv(m.result), Csv(m.ngChannels)));
                    sb.AppendLine();
                }

                // UTF8Encoding(true) 写入 BOM，否则 Excel 会把中文当 ANSI 乱码
                File.WriteAllText(dialog.FileName, sb.ToString(), new UTF8Encoding(true));
                Log.Info($"导出 CSV：{dialog.FileName}，{dataModels.Count} 行");
                MessageBox.Show($"已导出 {dataModels.Count} 条记录。", "完成",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Log.Error("导出 CSV 失败", ex);
                MessageBox.Show("导出失败：" + ex.Message, "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>CSV 字段转义：引号包裹并把内部引号翻倍，兼容 "1,3" 这类值。</summary>
        private static string Csv(string value) =>
            "\"" + (value ?? "").Replace("\"", "\"\"") + "\"";
    }
}

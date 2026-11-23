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
using WinFormsApp3.Common;
using WinFormsApp3.Model;

namespace WinFormsApp3.View
{
    public partial class totalView : UIPage
    {
        //用于绑定dataGridView1的数据源，BindingList是一个可绑定的列表，支持数据绑定和通知更改
        private BindingList<dataModel> dataModels = new BindingList<dataModel>();

        public totalView()
        {
            InitializeComponent();
            dataGridView1.AllowUserToAddRows = false;   // 查询页只读，不需要新增行
            dataGridView1.DataSource = dataModels;      // 绑定数据源
            FillComboBoxes();                          // 下拉框先有内容，用户才有得选
            SetDefaultDateRange();                     // 默认区间覆盖库里已有数据
            InitialLoadData();
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
                gkHelper.Log.Error("设置查询默认日期失败", ex);
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
                gkHelper.Log.Error("加载产品名称下拉框失败", ex);
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
                gkHelper.Log.Error("查询统计页加载历史数据失败", ex);
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

            var datas = mainCommon.sqlHelper.GetByConditions(start, end, uiComboBox1.Text.Trim());
            dataModels.Clear();
            foreach (var item in datas)
            {
                dataModels.Add(item);
            }

            // int.TryParse 代替 Convert.ToInt16：下拉框里出现非数字时不会抛异常
            if (int.TryParse(uiComboBox2.Text.Trim(), out int ch) && ch >= 1 && ch <= 8)
                ShowOnlySpecifiedColumns(dataGridView1, "Column1", "Column2", "Column3", $"Column{ch + 3}");
            else
                ShowAllColumns(dataGridView1);
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

        //点击导出按钮，将dataGridView1中的数据导出为Excel文件
        private void uiButton2_Click(object sender, EventArgs e)
        {

        }
    }
}

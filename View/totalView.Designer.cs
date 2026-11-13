namespace WinFormsApp3.View
{
    partial class totalView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            uiPanel1 = new Sunny.UI.UIPanel();
            uiPanel2 = new Sunny.UI.UIPanel();
            uiButton1 = new Sunny.UI.UIButton();
            uiButton2 = new Sunny.UI.UIButton();
            uiLabel1 = new Sunny.UI.UILabel();
            uiDatePicker1 = new Sunny.UI.UIDatePicker();
            uiLabel2 = new Sunny.UI.UILabel();
            uiComboBox1 = new Sunny.UI.UIComboBox();
            uiComboBox2 = new Sunny.UI.UIComboBox();
            uiLabel3 = new Sunny.UI.UILabel();
            uiDatePicker2 = new Sunny.UI.UIDatePicker();
            uiLabel4 = new Sunny.UI.UILabel();
            uiDataGridView1 = new Sunny.UI.UIDataGridView();
            Column1 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            Column3 = new DataGridViewTextBoxColumn();
            Column4 = new DataGridViewTextBoxColumn();
            Column5 = new DataGridViewTextBoxColumn();
            Column6 = new DataGridViewTextBoxColumn();
            Column7 = new DataGridViewTextBoxColumn();
            Column8 = new DataGridViewTextBoxColumn();
            uiPanel1.SuspendLayout();
            uiPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)uiDataGridView1).BeginInit();
            SuspendLayout();
            // 
            // uiPanel1
            // 
            uiPanel1.Controls.Add(uiComboBox2);
            uiPanel1.Controls.Add(uiLabel3);
            uiPanel1.Controls.Add(uiDatePicker2);
            uiPanel1.Controls.Add(uiLabel4);
            uiPanel1.Controls.Add(uiComboBox1);
            uiPanel1.Controls.Add(uiLabel2);
            uiPanel1.Controls.Add(uiDatePicker1);
            uiPanel1.Controls.Add(uiLabel1);
            uiPanel1.Controls.Add(uiButton2);
            uiPanel1.Controls.Add(uiButton1);
            uiPanel1.Dock = DockStyle.Top;
            uiPanel1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiPanel1.Location = new Point(0, 0);
            uiPanel1.Margin = new Padding(4, 5, 4, 5);
            uiPanel1.MinimumSize = new Size(1, 1);
            uiPanel1.Name = "uiPanel1";
            uiPanel1.Size = new Size(1051, 100);
            uiPanel1.TabIndex = 0;
            uiPanel1.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // uiPanel2
            // 
            uiPanel2.Controls.Add(uiDataGridView1);
            uiPanel2.Dock = DockStyle.Fill;
            uiPanel2.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiPanel2.Location = new Point(0, 100);
            uiPanel2.Margin = new Padding(4, 5, 4, 5);
            uiPanel2.MinimumSize = new Size(1, 1);
            uiPanel2.Name = "uiPanel2";
            uiPanel2.Size = new Size(1051, 513);
            uiPanel2.TabIndex = 1;
            uiPanel2.TextAlignment = ContentAlignment.MiddleCenter;
            uiPanel2.Click += uiPanel2_Click;
            // 
            // uiButton1
            // 
            uiButton1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton1.Location = new Point(606, 21);
            uiButton1.MinimumSize = new Size(1, 1);
            uiButton1.Name = "uiButton1";
            uiButton1.Size = new Size(100, 35);
            uiButton1.TabIndex = 0;
            uiButton1.Text = "查询";
            uiButton1.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            // 
            // uiButton2
            // 
            uiButton2.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton2.Location = new Point(751, 21);
            uiButton2.MinimumSize = new Size(1, 1);
            uiButton2.Name = "uiButton2";
            uiButton2.Size = new Size(100, 35);
            uiButton2.TabIndex = 1;
            uiButton2.Text = "导出excel";
            uiButton2.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            // 
            // uiLabel1
            // 
            uiLabel1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiLabel1.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel1.Location = new Point(12, 9);
            uiLabel1.Name = "uiLabel1";
            uiLabel1.Size = new Size(100, 23);
            uiLabel1.TabIndex = 2;
            uiLabel1.Text = "开始日期:";
            // 
            // uiDatePicker1
            // 
            uiDatePicker1.DateCultureInfo = new System.Globalization.CultureInfo("");
            uiDatePicker1.FillColor = Color.White;
            uiDatePicker1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiDatePicker1.Location = new Point(97, 9);
            uiDatePicker1.Margin = new Padding(4, 5, 4, 5);
            uiDatePicker1.MaxLength = 10;
            uiDatePicker1.MinimumSize = new Size(63, 0);
            uiDatePicker1.Name = "uiDatePicker1";
            uiDatePicker1.Padding = new Padding(0, 0, 30, 2);
            uiDatePicker1.Size = new Size(150, 23);
            uiDatePicker1.SymbolDropDown = 61555;
            uiDatePicker1.SymbolNormal = 61555;
            uiDatePicker1.SymbolSize = 24;
            uiDatePicker1.TabIndex = 3;
            uiDatePicker1.Text = "uiDatePicker1";
            uiDatePicker1.TextAlignment = ContentAlignment.MiddleLeft;
            uiDatePicker1.Value = new DateTime(2026, 11, 12, 16, 12, 42, 998);
            uiDatePicker1.Watermark = "";
            // 
            // uiLabel2
            // 
            uiLabel2.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiLabel2.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel2.Location = new Point(12, 53);
            uiLabel2.Name = "uiLabel2";
            uiLabel2.Size = new Size(100, 23);
            uiLabel2.TabIndex = 4;
            uiLabel2.Text = "产品名称:";
            // 
            // uiComboBox1
            // 
            uiComboBox1.DataSource = null;
            uiComboBox1.FillColor = Color.White;
            uiComboBox1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiComboBox1.ItemHoverColor = Color.FromArgb(155, 200, 255);
            uiComboBox1.ItemSelectForeColor = Color.FromArgb(235, 243, 255);
            uiComboBox1.Location = new Point(97, 53);
            uiComboBox1.Margin = new Padding(4, 5, 4, 5);
            uiComboBox1.MinimumSize = new Size(63, 0);
            uiComboBox1.Name = "uiComboBox1";
            uiComboBox1.Padding = new Padding(0, 0, 30, 2);
            uiComboBox1.Size = new Size(150, 23);
            uiComboBox1.SymbolSize = 24;
            uiComboBox1.TabIndex = 10;
            uiComboBox1.Text = "请选择产品";
            uiComboBox1.TextAlignment = ContentAlignment.MiddleLeft;
            uiComboBox1.Watermark = "";
            // 
            // uiComboBox2
            // 
            uiComboBox2.DataSource = null;
            uiComboBox2.FillColor = Color.White;
            uiComboBox2.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiComboBox2.ItemHoverColor = Color.FromArgb(155, 200, 255);
            uiComboBox2.ItemSelectForeColor = Color.FromArgb(235, 243, 255);
            uiComboBox2.Location = new Point(365, 53);
            uiComboBox2.Margin = new Padding(4, 5, 4, 5);
            uiComboBox2.MinimumSize = new Size(63, 0);
            uiComboBox2.Name = "uiComboBox2";
            uiComboBox2.Padding = new Padding(0, 0, 30, 2);
            uiComboBox2.Size = new Size(150, 23);
            uiComboBox2.SymbolSize = 24;
            uiComboBox2.TabIndex = 14;
            uiComboBox2.Text = "请选择通道";
            uiComboBox2.TextAlignment = ContentAlignment.MiddleLeft;
            uiComboBox2.Watermark = "";
            // 
            // uiLabel3
            // 
            uiLabel3.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiLabel3.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel3.Location = new Point(280, 53);
            uiLabel3.Name = "uiLabel3";
            uiLabel3.Size = new Size(100, 23);
            uiLabel3.TabIndex = 13;
            uiLabel3.Text = "通道选择:";
            // 
            // uiDatePicker2
            // 
            uiDatePicker2.DateCultureInfo = new System.Globalization.CultureInfo("");
            uiDatePicker2.FillColor = Color.White;
            uiDatePicker2.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiDatePicker2.Location = new Point(365, 9);
            uiDatePicker2.Margin = new Padding(4, 5, 4, 5);
            uiDatePicker2.MaxLength = 10;
            uiDatePicker2.MinimumSize = new Size(63, 0);
            uiDatePicker2.Name = "uiDatePicker2";
            uiDatePicker2.Padding = new Padding(0, 0, 30, 2);
            uiDatePicker2.Size = new Size(150, 23);
            uiDatePicker2.SymbolDropDown = 61555;
            uiDatePicker2.SymbolNormal = 61555;
            uiDatePicker2.SymbolSize = 24;
            uiDatePicker2.TabIndex = 12;
            uiDatePicker2.Text = "2026-11-12";
            uiDatePicker2.TextAlignment = ContentAlignment.MiddleLeft;
            uiDatePicker2.Value = new DateTime(2026, 11, 12, 16, 12, 42, 998);
            uiDatePicker2.Watermark = "";
            // 
            // uiLabel4
            // 
            uiLabel4.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiLabel4.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel4.Location = new Point(280, 9);
            uiLabel4.Name = "uiLabel4";
            uiLabel4.Size = new Size(100, 23);
            uiLabel4.TabIndex = 11;
            uiLabel4.Text = "结束日期:";
            // 
            // uiDataGridView1
            // 
            dataGridViewCellStyle1.BackColor = Color.FromArgb(235, 243, 255);
            uiDataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            uiDataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            uiDataGridView1.BackgroundColor = Color.White;
            uiDataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(80, 160, 255);
            dataGridViewCellStyle2.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            uiDataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            uiDataGridView1.ColumnHeadersHeight = 32;
            uiDataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            uiDataGridView1.Columns.AddRange(new DataGridViewColumn[] { Column1, Column2, Column3, Column4, Column5, Column6, Column7, Column8 });
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = SystemColors.Window;
            dataGridViewCellStyle3.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(48, 48, 48);
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            uiDataGridView1.DefaultCellStyle = dataGridViewCellStyle3;
            uiDataGridView1.Dock = DockStyle.Fill;
            uiDataGridView1.EnableHeadersVisualStyles = false;
            uiDataGridView1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiDataGridView1.GridColor = Color.FromArgb(80, 160, 255);
            uiDataGridView1.Location = new Point(0, 0);
            uiDataGridView1.Name = "uiDataGridView1";
            uiDataGridView1.RectColor = Color.White;
            uiDataGridView1.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = Color.FromArgb(235, 243, 255);
            dataGridViewCellStyle4.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            dataGridViewCellStyle4.ForeColor = Color.FromArgb(48, 48, 48);
            dataGridViewCellStyle4.SelectionBackColor = Color.FromArgb(80, 160, 255);
            dataGridViewCellStyle4.SelectionForeColor = Color.White;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
            uiDataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            dataGridViewCellStyle5.BackColor = Color.White;
            dataGridViewCellStyle5.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiDataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle5;
            uiDataGridView1.ScrollBarRectColor = Color.FromArgb(224, 224, 224);
            uiDataGridView1.ScrollBarStyleInherited = false;
            uiDataGridView1.SelectedIndex = -1;
            uiDataGridView1.Size = new Size(1051, 513);
            uiDataGridView1.StripeOddColor = Color.FromArgb(235, 243, 255);
            uiDataGridView1.TabIndex = 0;
            // 
            // Column1
            // 
            Column1.HeaderText = "序号";
            Column1.Name = "Column1";
            // 
            // Column2
            // 
            Column2.HeaderText = "通道";
            Column2.Name = "Column2";
            // 
            // Column3
            // 
            Column3.HeaderText = "检测时间";
            Column3.Name = "Column3";
            // 
            // Column4
            // 
            Column4.HeaderText = "产品名称";
            Column4.Name = "Column4";
            // 
            // Column5
            // 
            Column5.HeaderText = "测量上限/mm";
            Column5.Name = "Column5";
            // 
            // Column6
            // 
            Column6.HeaderText = "测量下限/mm";
            Column6.Name = "Column6";
            // 
            // Column7
            // 
            Column7.HeaderText = "测量实际值/mm";
            Column7.Name = "Column7";
            // 
            // Column8
            // 
            Column8.HeaderText = "合格/不合格";
            Column8.Name = "Column8";
            // 
            // totalView
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1051, 613);
            Controls.Add(uiPanel2);
            Controls.Add(uiPanel1);
            Name = "totalView";
            Text = "totalView";
            Load += totalView_Load;
            uiPanel1.ResumeLayout(false);
            uiPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)uiDataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Sunny.UI.UIPanel uiPanel1;
        private Sunny.UI.UIButton uiButton2;
        private Sunny.UI.UIButton uiButton1;
        private Sunny.UI.UIPanel uiPanel2;
        private Sunny.UI.UILabel uiLabel2;
        private Sunny.UI.UIDatePicker uiDatePicker1;
        private Sunny.UI.UILabel uiLabel1;
        private Sunny.UI.UIComboBox uiComboBox2;
        private Sunny.UI.UILabel uiLabel3;
        private Sunny.UI.UIDatePicker uiDatePicker2;
        private Sunny.UI.UILabel uiLabel4;
        private Sunny.UI.UIComboBox uiComboBox1;
        private Sunny.UI.UIDataGridView uiDataGridView1;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn Column3;
        private DataGridViewTextBoxColumn Column4;
        private DataGridViewTextBoxColumn Column5;
        private DataGridViewTextBoxColumn Column6;
        private DataGridViewTextBoxColumn Column7;
        private DataGridViewTextBoxColumn Column8;
    }
}
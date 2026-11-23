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
            uiPanel1 = new Sunny.UI.UIPanel();
            uiComboBox2 = new Sunny.UI.UIComboBox();
            uiLabel3 = new Sunny.UI.UILabel();
            uiDatePicker2 = new Sunny.UI.UIDatePicker();
            uiLabel4 = new Sunny.UI.UILabel();
            uiComboBox1 = new Sunny.UI.UIComboBox();
            uiLabel2 = new Sunny.UI.UILabel();
            uiDatePicker1 = new Sunny.UI.UIDatePicker();
            uiLabel1 = new Sunny.UI.UILabel();
            uiButton2 = new Sunny.UI.UIButton();
            uiButton1 = new Sunny.UI.UIButton();
            uiPanel2 = new Sunny.UI.UIPanel();
            dataGridView1 = new DataGridView();
            Column1 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            Column3 = new DataGridViewTextBoxColumn();
            Column4 = new DataGridViewTextBoxColumn();
            Column5 = new DataGridViewTextBoxColumn();
            Column6 = new DataGridViewTextBoxColumn();
            Column7 = new DataGridViewTextBoxColumn();
            Column8 = new DataGridViewTextBoxColumn();
            Column9 = new DataGridViewTextBoxColumn();
            Column10 = new DataGridViewTextBoxColumn();
            Column11 = new DataGridViewTextBoxColumn();
            uiPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
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
            uiPanel1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiPanel1.Location = new Point(0, 0);
            uiPanel1.Margin = new Padding(4, 5, 4, 5);
            uiPanel1.MinimumSize = new Size(1, 1);
            uiPanel1.Name = "uiPanel1";
            uiPanel1.Size = new Size(1051, 100);
            uiPanel1.TabIndex = 0;
            uiPanel1.Text = null;
            uiPanel1.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // uiComboBox2
            // 
            uiComboBox2.DataSource = null;
            uiComboBox2.FillColor = Color.White;
            uiComboBox2.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
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
            uiComboBox2.TextAlignment = ContentAlignment.MiddleLeft;
            uiComboBox2.Watermark = "";
            // 
            // uiLabel3
            // 
            uiLabel3.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
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
            uiDatePicker2.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
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
            uiDatePicker2.Text = "2026-11-13";
            uiDatePicker2.TextAlignment = ContentAlignment.MiddleLeft;
            uiDatePicker2.Value = new DateTime(2026, 11, 13, 0, 0, 0, 0);
            uiDatePicker2.Watermark = "";
            // 
            // uiLabel4
            // 
            uiLabel4.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiLabel4.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel4.Location = new Point(280, 9);
            uiLabel4.Name = "uiLabel4";
            uiLabel4.Size = new Size(100, 23);
            uiLabel4.TabIndex = 11;
            uiLabel4.Text = "结束日期:";
            // 
            // uiComboBox1
            // 
            uiComboBox1.DataSource = null;
            uiComboBox1.FillColor = Color.White;
            uiComboBox1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
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
            uiComboBox1.TextAlignment = ContentAlignment.MiddleLeft;
            uiComboBox1.Watermark = "";
            // 
            // uiLabel2
            // 
            uiLabel2.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiLabel2.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel2.Location = new Point(12, 53);
            uiLabel2.Name = "uiLabel2";
            uiLabel2.Size = new Size(100, 23);
            uiLabel2.TabIndex = 4;
            uiLabel2.Text = "产品名称:";
            // 
            // uiDatePicker1
            // 
            uiDatePicker1.DateCultureInfo = new System.Globalization.CultureInfo("");
            uiDatePicker1.FillColor = Color.White;
            uiDatePicker1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
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
            uiDatePicker1.Text = "2026-11-13";
            uiDatePicker1.TextAlignment = ContentAlignment.MiddleLeft;
            uiDatePicker1.Value = new DateTime(2026, 11, 13, 0, 0, 0, 0);
            uiDatePicker1.Watermark = "";
            // 
            // uiLabel1
            // 
            uiLabel1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiLabel1.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel1.Location = new Point(12, 9);
            uiLabel1.Name = "uiLabel1";
            uiLabel1.Size = new Size(100, 23);
            uiLabel1.TabIndex = 2;
            uiLabel1.Text = "开始日期:";
            // 
            // uiButton2
            // 
            uiButton2.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiButton2.Location = new Point(751, 21);
            uiButton2.MinimumSize = new Size(1, 1);
            uiButton2.Name = "uiButton2";
            uiButton2.Size = new Size(100, 35);
            uiButton2.TabIndex = 1;
            uiButton2.Text = "导出excel";
            uiButton2.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            uiButton2.Click += uiButton2_Click;
            // 
            // uiButton1
            // 
            uiButton1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiButton1.Location = new Point(606, 21);
            uiButton1.MinimumSize = new Size(1, 1);
            uiButton1.Name = "uiButton1";
            uiButton1.Size = new Size(100, 35);
            uiButton1.TabIndex = 0;
            uiButton1.Text = "查询";
            uiButton1.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            uiButton1.Click += uiButton1_Click;
            // 
            // uiPanel2
            // 
            uiPanel2.Dock = DockStyle.Fill;
            uiPanel2.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiPanel2.Location = new Point(0, 100);
            uiPanel2.Margin = new Padding(4, 5, 4, 5);
            uiPanel2.MinimumSize = new Size(1, 1);
            uiPanel2.Name = "uiPanel2";
            uiPanel2.Size = new Size(1051, 513);
            uiPanel2.TabIndex = 1;
            uiPanel2.Text = null;
            uiPanel2.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // dataGridView1
            // 
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.BackgroundColor = SystemColors.InactiveBorder;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { Column1, Column2, Column3, Column4, Column5, Column6, Column7, Column8, Column9, Column10, Column11 });
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(0, 100);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(1051, 513);
            dataGridView1.TabIndex = 2;
            // 
            // Column1
            // 
            Column1.DataPropertyName = "Id";
            Column1.HeaderText = "序号";
            Column1.Name = "Column1";
            // 
            // Column2
            // 
            Column2.DataPropertyName = "testTime";
            Column2.HeaderText = "检测时间";
            Column2.Name = "Column2";
            // 
            // Column3
            // 
            Column3.DataPropertyName = "projectName";
            Column3.HeaderText = "产品名称";
            Column3.Name = "Column3";
            // 
            // Column4
            // 
            Column4.DataPropertyName = "data1";
            Column4.HeaderText = "通道1数据";
            Column4.Name = "Column4";
            // 
            // Column5
            // 
            Column5.DataPropertyName = "data2";
            Column5.HeaderText = "通道2数据";
            Column5.Name = "Column5";
            // 
            // Column6
            // 
            Column6.DataPropertyName = "data3";
            Column6.HeaderText = "通道3数据";
            Column6.Name = "Column6";
            // 
            // Column7
            // 
            Column7.DataPropertyName = "data4";
            Column7.HeaderText = "通道4数据";
            Column7.Name = "Column7";
            // 
            // Column8
            // 
            Column8.DataPropertyName = "data5";
            Column8.HeaderText = "通道5数据";
            Column8.Name = "Column8";
            // 
            // Column9
            // 
            Column9.DataPropertyName = "data6";
            Column9.HeaderText = "通道6数据";
            Column9.Name = "Column9";
            // 
            // Column10
            // 
            Column10.DataPropertyName = "data7";
            Column10.HeaderText = "通道7数据";
            Column10.Name = "Column10";
            // 
            // Column11
            // 
            Column11.DataPropertyName = "data8";
            Column11.HeaderText = "通道8数据";
            Column11.Name = "Column11";
            // 
            // totalView
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1051, 613);
            Controls.Add(dataGridView1);
            Controls.Add(uiPanel2);
            Controls.Add(uiPanel1);
            Name = "totalView";
            Text = "totalView";
            uiPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
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
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn Column3;
        private DataGridViewTextBoxColumn Column4;
        private DataGridViewTextBoxColumn Column5;
        private DataGridViewTextBoxColumn Column6;
        private DataGridViewTextBoxColumn Column7;
        private DataGridViewTextBoxColumn Column8;
        private DataGridViewTextBoxColumn Column9;
        private DataGridViewTextBoxColumn Column10;
        private DataGridViewTextBoxColumn Column11;
    }
}
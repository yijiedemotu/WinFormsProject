namespace WinFormsApp3.View
{
    partial class mainView
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
            components = new System.ComponentModel.Container();
            uiPanel1 = new Sunny.UI.UIPanel();
            uiButton3 = new Sunny.UI.UIButton();
            uiButton2 = new Sunny.UI.UIButton();
            uiLabel3 = new Sunny.UI.UILabel();
            uiLabel2 = new Sunny.UI.UILabel();
            uiLabel1 = new Sunny.UI.UILabel();
            uiTextBox1 = new Sunny.UI.UITextBox();
            uiComboBox1 = new Sunny.UI.UIComboBox();
            uiButton1 = new Sunny.UI.UIButton();
            uiPanel2 = new Sunny.UI.UIPanel();
            uiTableLayoutPanel1 = new Sunny.UI.UITableLayoutPanel();
            uiTitlePanel1 = new Sunny.UI.UITitlePanel();
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
            timer1 = new System.Windows.Forms.Timer(components);
            uiPanel1.SuspendLayout();
            uiPanel2.SuspendLayout();
            uiTitlePanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // uiPanel1
            // 
            uiPanel1.Controls.Add(uiButton3);
            uiPanel1.Controls.Add(uiButton2);
            uiPanel1.Controls.Add(uiLabel3);
            uiPanel1.Controls.Add(uiLabel2);
            uiPanel1.Controls.Add(uiLabel1);
            uiPanel1.Controls.Add(uiTextBox1);
            uiPanel1.Controls.Add(uiComboBox1);
            uiPanel1.Controls.Add(uiButton1);
            uiPanel1.Dock = DockStyle.Top;
            uiPanel1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiPanel1.Location = new Point(0, 0);
            uiPanel1.Margin = new Padding(4, 5, 4, 5);
            uiPanel1.MinimumSize = new Size(1, 1);
            uiPanel1.Name = "uiPanel1";
            uiPanel1.Size = new Size(1051, 39);
            uiPanel1.TabIndex = 0;
            uiPanel1.Text = null;
            uiPanel1.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // uiButton3
            // 
            uiButton3.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiButton3.Location = new Point(836, 5);
            uiButton3.MinimumSize = new Size(1, 1);
            uiButton3.Name = "uiButton3";
            uiButton3.Size = new Size(128, 28);
            uiButton3.TabIndex = 7;
            uiButton3.Text = "保存数据";
            uiButton3.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            uiButton3.Click += uiButton3_Click;
            // 
            // uiButton2
            // 
            uiButton2.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiButton2.Location = new Point(663, 5);
            uiButton2.MinimumSize = new Size(1, 1);
            uiButton2.Name = "uiButton2";
            uiButton2.Size = new Size(136, 28);
            uiButton2.TabIndex = 6;
            uiButton2.Text = "隐藏通道";
            uiButton2.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            // 
            // uiLabel3
            // 
            uiLabel3.Font = new Font("宋体", 12F, FontStyle.Bold, GraphicsUnit.Point);
            uiLabel3.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel3.Location = new Point(259, 9);
            uiLabel3.Name = "uiLabel3";
            uiLabel3.Size = new Size(90, 24);
            uiLabel3.TabIndex = 5;
            uiLabel3.Text = "无连接";
            // 
            // uiLabel2
            // 
            uiLabel2.Font = new Font("宋体", 12F, FontStyle.Bold, GraphicsUnit.Point);
            uiLabel2.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel2.Location = new Point(167, 8);
            uiLabel2.Name = "uiLabel2";
            uiLabel2.Size = new Size(91, 26);
            uiLabel2.TabIndex = 4;
            uiLabel2.Text = "当前连接:";
            // 
            // uiLabel1
            // 
            uiLabel1.Font = new Font("宋体", 12F, FontStyle.Bold, GraphicsUnit.Point);
            uiLabel1.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel1.Location = new Point(3, 9);
            uiLabel1.Name = "uiLabel1";
            uiLabel1.Size = new Size(84, 28);
            uiLabel1.TabIndex = 3;
            uiLabel1.Text = "产品名称:";
            // 
            // uiTextBox1
            // 
            uiTextBox1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiTextBox1.Location = new Point(91, 5);
            uiTextBox1.Margin = new Padding(4, 5, 4, 5);
            uiTextBox1.MinimumSize = new Size(1, 16);
            uiTextBox1.Name = "uiTextBox1";
            uiTextBox1.Padding = new Padding(5);
            uiTextBox1.ShowText = false;
            uiTextBox1.Size = new Size(72, 29);
            uiTextBox1.TabIndex = 2;
            uiTextBox1.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox1.Watermark = "";
            // 
            // uiComboBox1
            // 
            uiComboBox1.DataSource = null;
            uiComboBox1.FillColor = Color.White;
            uiComboBox1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiComboBox1.ItemHoverColor = Color.FromArgb(155, 200, 255);
            uiComboBox1.ItemSelectForeColor = Color.FromArgb(235, 243, 255);
            uiComboBox1.Location = new Point(353, 5);
            uiComboBox1.Margin = new Padding(4, 5, 4, 5);
            uiComboBox1.MinimumSize = new Size(63, 0);
            uiComboBox1.Name = "uiComboBox1";
            uiComboBox1.Padding = new Padding(0, 0, 30, 2);
            uiComboBox1.Size = new Size(145, 29);
            uiComboBox1.SymbolSize = 24;
            uiComboBox1.TabIndex = 1;
            uiComboBox1.TextAlignment = ContentAlignment.MiddleLeft;
            uiComboBox1.Watermark = "";
            uiComboBox1.SelectedIndexChanged += uiComboBox1_SelectedIndexChanged;
            // 
            // uiButton1
            // 
            uiButton1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiButton1.Location = new Point(510, 5);
            uiButton1.MinimumSize = new Size(1, 1);
            uiButton1.Name = "uiButton1";
            uiButton1.Size = new Size(109, 28);
            uiButton1.TabIndex = 0;
            uiButton1.Text = "连接";
            uiButton1.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            uiButton1.Click += uiButton1_Click;
            // 
            // uiPanel2
            // 
            uiPanel2.Controls.Add(uiTableLayoutPanel1);
            uiPanel2.Dock = DockStyle.Top;
            uiPanel2.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiPanel2.Location = new Point(0, 39);
            uiPanel2.Margin = new Padding(4, 5, 4, 5);
            uiPanel2.MinimumSize = new Size(1, 1);
            uiPanel2.Name = "uiPanel2";
            uiPanel2.Size = new Size(1051, 244);
            uiPanel2.TabIndex = 1;
            uiPanel2.Text = "uiPanel2";
            uiPanel2.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // uiTableLayoutPanel1
            // 
            uiTableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            uiTableLayoutPanel1.ColumnCount = 4;
            uiTableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            uiTableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            uiTableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            uiTableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            uiTableLayoutPanel1.Dock = DockStyle.Fill;
            uiTableLayoutPanel1.Location = new Point(0, 0);
            uiTableLayoutPanel1.Name = "uiTableLayoutPanel1";
            uiTableLayoutPanel1.RowCount = 2;
            uiTableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            uiTableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            uiTableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            uiTableLayoutPanel1.Size = new Size(1051, 244);
            uiTableLayoutPanel1.TabIndex = 0;
            uiTableLayoutPanel1.TagString = null;
            // 
            // uiTitlePanel1
            // 
            uiTitlePanel1.Controls.Add(dataGridView1);
            uiTitlePanel1.Dock = DockStyle.Fill;
            uiTitlePanel1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiTitlePanel1.Location = new Point(0, 283);
            uiTitlePanel1.Margin = new Padding(4, 5, 4, 5);
            uiTitlePanel1.MinimumSize = new Size(1, 1);
            uiTitlePanel1.Name = "uiTitlePanel1";
            uiTitlePanel1.Padding = new Padding(1, 35, 1, 1);
            uiTitlePanel1.ShowText = false;
            uiTitlePanel1.Size = new Size(1051, 330);
            uiTitlePanel1.TabIndex = 2;
            uiTitlePanel1.Text = "实时数据";
            uiTitlePanel1.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // dataGridView1
            // 
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.BackgroundColor = SystemColors.InactiveBorder;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { Column1, Column2, Column3, Column4, Column5, Column6, Column7, Column8, Column9, Column10, Column11 });
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(1, 35);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(1049, 294);
            dataGridView1.TabIndex = 0;
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
            // timer1
            // 
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            // 
            // mainView
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1051, 613);
            Controls.Add(uiTitlePanel1);
            Controls.Add(uiPanel2);
            Controls.Add(uiPanel1);
            Name = "mainView";
            Text = "mainView";
            Load += mainView_Load;
            uiPanel1.ResumeLayout(false);
            uiPanel2.ResumeLayout(false);
            uiTitlePanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Sunny.UI.UIPanel uiPanel1;
        private Sunny.UI.UIPanel uiPanel2;
        private Sunny.UI.UITableLayoutPanel uiTableLayoutPanel1;
        private Sunny.UI.UILabel uiLabel1;
        private Sunny.UI.UITextBox uiTextBox1;
        private Sunny.UI.UIComboBox uiComboBox1;
        private Sunny.UI.UIButton uiButton1;
        private Sunny.UI.UIButton uiButton3;
        private Sunny.UI.UIButton uiButton2;
        private Sunny.UI.UILabel uiLabel3;
        private Sunny.UI.UILabel uiLabel2;
        private Sunny.UI.UITitlePanel uiTitlePanel1;
        private DataGridView dataGridView1;
        private System.Windows.Forms.Timer timer1;
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
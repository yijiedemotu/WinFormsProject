namespace WinFormsApp3.View
{
    partial class plotView
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
            uiLabel1 = new Sunny.UI.UILabel();
            uiDatePicker1 = new Sunny.UI.UIDatePicker();
            uiDatePicker2 = new Sunny.UI.UIDatePicker();
            uiLabel2 = new Sunny.UI.UILabel();
            uiLabel3 = new Sunny.UI.UILabel();
            uiComboBox1 = new Sunny.UI.UIComboBox();
            uiButton1 = new Sunny.UI.UIButton();
            formsPlot1 = new ScottPlot.WinForms.FormsPlot();
            uiPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // uiPanel1
            // 
            uiPanel1.Controls.Add(uiButton1);
            uiPanel1.Controls.Add(uiComboBox1);
            uiPanel1.Controls.Add(uiLabel3);
            uiPanel1.Controls.Add(uiDatePicker2);
            uiPanel1.Controls.Add(uiLabel2);
            uiPanel1.Controls.Add(uiDatePicker1);
            uiPanel1.Controls.Add(uiLabel1);
            uiPanel1.Dock = DockStyle.Top;
            uiPanel1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiPanel1.Location = new Point(0, 0);
            uiPanel1.Margin = new Padding(4, 5, 4, 5);
            uiPanel1.MinimumSize = new Size(1, 1);
            uiPanel1.Name = "uiPanel1";
            uiPanel1.Size = new Size(1051, 79);
            uiPanel1.TabIndex = 0;
            uiPanel1.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // uiLabel1
            // 
            uiLabel1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiLabel1.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel1.Location = new Point(31, 26);
            uiLabel1.Name = "uiLabel1";
            uiLabel1.Size = new Size(100, 23);
            uiLabel1.TabIndex = 0;
            uiLabel1.Text = "开始时间:";
            // 
            // uiDatePicker1
            // 
            uiDatePicker1.DateCultureInfo = new System.Globalization.CultureInfo("");
            uiDatePicker1.FillColor = Color.White;
            uiDatePicker1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiDatePicker1.Location = new Point(121, 26);
            uiDatePicker1.Margin = new Padding(4, 5, 4, 5);
            uiDatePicker1.MaxLength = 10;
            uiDatePicker1.MinimumSize = new Size(63, 0);
            uiDatePicker1.Name = "uiDatePicker1";
            uiDatePicker1.Padding = new Padding(0, 0, 30, 2);
            uiDatePicker1.Size = new Size(150, 23);
            uiDatePicker1.SymbolDropDown = 61555;
            uiDatePicker1.SymbolNormal = 61555;
            uiDatePicker1.SymbolSize = 24;
            uiDatePicker1.TabIndex = 1;
            uiDatePicker1.TextAlignment = ContentAlignment.MiddleLeft;
            uiDatePicker1.Value = new DateTime(2026, 11, 12, 16, 34, 6, 602);
            uiDatePicker1.Watermark = "";
            // 
            // uiDatePicker2
            // 
            uiDatePicker2.DateCultureInfo = new System.Globalization.CultureInfo("");
            uiDatePicker2.FillColor = Color.White;
            uiDatePicker2.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiDatePicker2.Location = new Point(400, 26);
            uiDatePicker2.Margin = new Padding(4, 5, 4, 5);
            uiDatePicker2.MaxLength = 10;
            uiDatePicker2.MinimumSize = new Size(63, 0);
            uiDatePicker2.Name = "uiDatePicker2";
            uiDatePicker2.Padding = new Padding(0, 0, 30, 2);
            uiDatePicker2.Size = new Size(150, 23);
            uiDatePicker2.SymbolDropDown = 61555;
            uiDatePicker2.SymbolNormal = 61555;
            uiDatePicker2.SymbolSize = 24;
            uiDatePicker2.TabIndex = 3;
            uiDatePicker2.TextAlignment = ContentAlignment.MiddleLeft;
            uiDatePicker2.Value = new DateTime(2026, 11, 12, 16, 34, 6, 602);
            uiDatePicker2.Watermark = "";
            // 
            // uiLabel2
            // 
            uiLabel2.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiLabel2.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel2.Location = new Point(310, 26);
            uiLabel2.Name = "uiLabel2";
            uiLabel2.Size = new Size(100, 23);
            uiLabel2.TabIndex = 2;
            uiLabel2.Text = "结束时间:";
            // 
            // uiLabel3
            // 
            uiLabel3.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiLabel3.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel3.Location = new Point(610, 26);
            uiLabel3.Name = "uiLabel3";
            uiLabel3.Size = new Size(100, 23);
            uiLabel3.TabIndex = 4;
            uiLabel3.Text = "通道选择:";
            // 
            // uiComboBox1
            // 
            uiComboBox1.DataSource = null;
            uiComboBox1.FillColor = Color.White;
            uiComboBox1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiComboBox1.ItemHoverColor = Color.FromArgb(155, 200, 255);
            uiComboBox1.ItemSelectForeColor = Color.FromArgb(235, 243, 255);
            uiComboBox1.Location = new Point(704, 26);
            uiComboBox1.Margin = new Padding(4, 5, 4, 5);
            uiComboBox1.MinimumSize = new Size(63, 0);
            uiComboBox1.Name = "uiComboBox1";
            uiComboBox1.Padding = new Padding(0, 0, 30, 2);
            uiComboBox1.Size = new Size(150, 23);
            uiComboBox1.SymbolSize = 24;
            uiComboBox1.TabIndex = 5;
            uiComboBox1.Text = "请选择通道";
            uiComboBox1.TextAlignment = ContentAlignment.MiddleLeft;
            uiComboBox1.Watermark = "";
            // 
            // uiButton1
            // 
            uiButton1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiButton1.Location = new Point(897, 26);
            uiButton1.MinimumSize = new Size(1, 1);
            uiButton1.Name = "uiButton1";
            uiButton1.Size = new Size(100, 23);
            uiButton1.TabIndex = 6;
            uiButton1.Text = "查询";
            uiButton1.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            // 
            // formsPlot1
            // 
            formsPlot1.Dock = DockStyle.Fill;
            formsPlot1.Location = new Point(0, 79);
            formsPlot1.Name = "formsPlot1";
            formsPlot1.Size = new Size(1051, 534);
            formsPlot1.TabIndex = 1;
            // 
            // plotView
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1051, 613);
            Controls.Add(formsPlot1);
            Controls.Add(uiPanel1);
            Name = "plotView";
            Text = "plotView";
            Load += plotView_Load;
            uiPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Sunny.UI.UIPanel uiPanel1;
        private Sunny.UI.UIComboBox uiComboBox1;
        private Sunny.UI.UILabel uiLabel3;
        private Sunny.UI.UIDatePicker uiDatePicker2;
        private Sunny.UI.UILabel uiLabel2;
        private Sunny.UI.UIDatePicker uiDatePicker1;
        private Sunny.UI.UILabel uiLabel1;
        private Sunny.UI.UIButton uiButton1;
        private ScottPlot.WinForms.FormsPlot formsPlot1;
    }
}
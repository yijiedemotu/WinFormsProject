namespace WinFormsApp3
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            uiPanel1 = new Sunny.UI.UIPanel();
            uiLight1 = new Sunny.UI.UILight();
            uiSymbolButton4 = new Sunny.UI.UISymbolButton();
            uiSymbolButton3 = new Sunny.UI.UISymbolButton();
            uiSymbolButton2 = new Sunny.UI.UISymbolButton();
            uiSymbolButton1 = new Sunny.UI.UISymbolButton();
            uiPanel2 = new Sunny.UI.UIPanel();
            uiPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // uiPanel1
            // 
            uiPanel1.Controls.Add(uiLight1);
            uiPanel1.Controls.Add(uiSymbolButton4);
            uiPanel1.Controls.Add(uiSymbolButton3);
            uiPanel1.Controls.Add(uiSymbolButton2);
            uiPanel1.Controls.Add(uiSymbolButton1);
            uiPanel1.Dock = DockStyle.Top;
            uiPanel1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiPanel1.Location = new Point(2, 36);
            uiPanel1.Margin = new Padding(4, 5, 4, 5);
            uiPanel1.MinimumSize = new Size(1, 1);
            uiPanel1.Name = "uiPanel1";
            uiPanel1.Size = new Size(1051, 54);
            uiPanel1.TabIndex = 0;
            uiPanel1.Text = null;
            uiPanel1.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // uiLight1
            // 
            uiLight1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiLight1.Location = new Point(977, 11);
            uiLight1.MinimumSize = new Size(1, 1);
            uiLight1.Name = "uiLight1";
            uiLight1.Radius = 35;
            uiLight1.Size = new Size(35, 35);
            uiLight1.State = Sunny.UI.UILightState.Off;
            uiLight1.TabIndex = 4;
            uiLight1.Text = "uiLight1";
            // 
            // uiSymbolButton4
            // 
            uiSymbolButton4.Dock = DockStyle.Left;
            uiSymbolButton4.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiSymbolButton4.Location = new Point(516, 0);
            uiSymbolButton4.MinimumSize = new Size(1, 1);
            uiSymbolButton4.Name = "uiSymbolButton4";
            uiSymbolButton4.Size = new Size(172, 54);
            uiSymbolButton4.Symbol = 61546;
            uiSymbolButton4.TabIndex = 3;
            uiSymbolButton4.Text = "阈值设置";
            uiSymbolButton4.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            uiSymbolButton4.Click += uiSymbolButton4_Click;
            // 
            // uiSymbolButton3
            // 
            uiSymbolButton3.Dock = DockStyle.Left;
            uiSymbolButton3.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiSymbolButton3.Location = new Point(344, 0);
            uiSymbolButton3.MinimumSize = new Size(1, 1);
            uiSymbolButton3.Name = "uiSymbolButton3";
            uiSymbolButton3.Size = new Size(172, 54);
            uiSymbolButton3.Symbol = 61953;
            uiSymbolButton3.TabIndex = 2;
            uiSymbolButton3.Text = "趋势曲线";
            uiSymbolButton3.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            uiSymbolButton3.Click += uiSymbolButton3_Click;
            // 
            // uiSymbolButton2
            // 
            uiSymbolButton2.Dock = DockStyle.Left;
            uiSymbolButton2.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiSymbolButton2.Location = new Point(172, 0);
            uiSymbolButton2.MinimumSize = new Size(1, 1);
            uiSymbolButton2.Name = "uiSymbolButton2";
            uiSymbolButton2.Size = new Size(172, 54);
            uiSymbolButton2.Symbol = 61442;
            uiSymbolButton2.TabIndex = 1;
            uiSymbolButton2.Text = "查询统计";
            uiSymbolButton2.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            uiSymbolButton2.Click += uiSymbolButton2_Click;
            // 
            // uiSymbolButton1
            // 
            uiSymbolButton1.Dock = DockStyle.Left;
            uiSymbolButton1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiSymbolButton1.Location = new Point(0, 0);
            uiSymbolButton1.MinimumSize = new Size(1, 1);
            uiSymbolButton1.Name = "uiSymbolButton1";
            uiSymbolButton1.Size = new Size(172, 54);
            uiSymbolButton1.TabIndex = 0;
            uiSymbolButton1.Text = "数据测量";
            uiSymbolButton1.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            uiSymbolButton1.Click += uiSymbolButton1_Click;
            // 
            // uiPanel2
            // 
            uiPanel2.Dock = DockStyle.Fill;
            uiPanel2.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            uiPanel2.Location = new Point(2, 90);
            uiPanel2.Margin = new Padding(4, 5, 4, 5);
            uiPanel2.MinimumSize = new Size(1, 1);
            uiPanel2.Name = "uiPanel2";
            uiPanel2.Size = new Size(1051, 613);
            uiPanel2.TabIndex = 1;
            uiPanel2.Text = null;
            uiPanel2.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            AutoScaleMode = AutoScaleMode.None;
            AutoSize = true;
            ClientSize = new Size(1055, 705);
            Controls.Add(uiPanel2);
            Controls.Add(uiPanel1);
            HelpButton = true;
            Name = "Form1";
            Padding = new Padding(2, 36, 2, 2);
            Resizable = true;
            Text = "Form1";
            ZoomScaleRect = new Rectangle(22, 22, 800, 450);
            Load += Form1_Load;
            Shown += Form1_Shown;
            uiPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Sunny.UI.UIPanel uiPanel1;
        private Sunny.UI.UISymbolButton uiSymbolButton4;
        private Sunny.UI.UISymbolButton uiSymbolButton3;
        private Sunny.UI.UISymbolButton uiSymbolButton2;
        private Sunny.UI.UISymbolButton uiSymbolButton1;
        private Sunny.UI.UIPanel uiPanel2;
        private Sunny.UI.UILight uiLight1;
    }
}

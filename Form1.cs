using Sunny.UI;
using System.Windows.Forms;
using TICP.Helper;
using WinFormsApp3.View;

namespace WinFormsApp3
{
    //窗体的大框架
    public partial class Form1 : UIForm
    {
        // 页面延迟创建：启动时不一次 new 出全部 4 个页面，
        // 避免 plotView 的 FormsPlot(ScottPlot/SkiaSharp) 和 totalView 的数据库查询
        // 在 Form1 构造阶段就初始化 —— 任一环节抛异常都会让 Application.Run(new Form1()) 崩掉。
        private mainView? mainView;
        private totalView? totalView;
        private plotView? plotView;
        private yvzhiView? yvzhiView;

        public Form1()
        {
            InitializeComponent();
        }

        private void uiSymbolButton1_Click(object sender, EventArgs e) =>
            ShowPage(ref mainView, () => new mainView(uiLight1));

        private void Form1_Load(object sender, EventArgs e)
        {
            // 默认显示数据测量页；走统一兜底，页面初始化失败也只提示不崩窗体，
            // 之后再点导航按钮还能重试（原来这里 mainView 为 null 就直接 return 了）。
            ShowPage(ref mainView, () => new mainView(uiLight1));
        }

        private void Form1_Shown(object sender, EventArgs e) { }

        private void uiSymbolButton2_Click(object sender, EventArgs e) =>
            ShowPage(ref totalView, () => new totalView());

        private void uiSymbolButton3_Click(object sender, EventArgs e) =>
            ShowPage(ref plotView, () => new plotView());

        private void uiSymbolButton4_Click(object sender, EventArgs e) =>
            ShowPage(ref yvzhiView, () => new yvzhiView());

        /// <summary>统一兜底：任何页面初始化失败只提示并记日志，不让整个程序崩掉。</summary>
        private void ShowPage<T>(ref T? page, Func<T> factory) where T : UIPage
        {
            try
            {
                page ??= factory();
                FormShowHelper.formShow(page!, uiPanel2);
            }
            catch (Exception ex)
            {
                Log.Error($"打开 {typeof(T).Name} 失败", ex);
                MessageBox.Show($"打开页面失败：{ex.Message}\n详细堆栈已写入 Logs 目录。",
                    "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

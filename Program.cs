using System.Windows.Forms;
using TICP.Helper;
using WinFormsApp3;

namespace TICP
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // 全局兜底：GDI+ / 第三方控件偶发异常不再弹「未经处理的异常」卡住进程，
            // 而是写日志 + 友好提示，方便从 Logs 目录拿到完整堆栈定位问题。
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) => Report("界面线程异常", e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                Report("后台线程异常", e.ExceptionObject as Exception);

            Application.Run(new Form1());
        }

        private static void Report(string title, Exception? ex)
        {
            if (ex == null) return;
            Log.Error(title, ex);

            // PerMonitorV2 跨屏时 WinForms 重算字体偶发的 GDI+ 异常：
            // 控件会继续用原字体显示，不影响功能，只记日志不打断操作。
            string stack = ex.ToString();
            if (stack.Contains("WmDpiChangedBeforeParent") || stack.Contains("Font.ToHfont"))
                return;

            MessageBox.Show($"{title}：{ex.Message}\n\n详细堆栈已写入 Logs 目录。",
                "程序异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

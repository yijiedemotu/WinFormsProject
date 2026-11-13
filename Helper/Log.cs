namespace gkHelper
{
    /// <summary>
    /// 轻量文本日志。路径固定为程序基目录下的 Logs，按日期/级别分目录。
    /// 日志写入失败绝不能让业务崩溃，因此内部自行兜底。
    /// </summary>
    public static class Log
    {
        private static readonly string BaseLogDirectory =
            Path.Combine(AppContext.BaseDirectory, "Logs");
        private static readonly object LockObject = new object();

        public static void Info(string message) => Write("Info", message);

        public static void Warn(string message) => Write("Warn", message);

        public static void Error(string message) => Write("Error", message);

        public static void Error(string message, Exception ex) =>
            Write("Error", message + Environment.NewLine + ex);

        private static void Write(string logType, string message)
        {
            try
            {
                lock (LockObject)
                {
                    string dateFolder = DateTime.Now.ToString("yyyy-MM-dd");
                    string logDirectory = Path.Combine(BaseLogDirectory, dateFolder, logType);
                    Directory.CreateDirectory(logDirectory);

                    string fileName = $"log_{dateFolder}.txt";
                    string filePath = Path.Combine(logDirectory, fileName);
                    string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{logType}] " +
                                   message.Replace(Environment.NewLine, Environment.NewLine + "    ");

                    File.AppendAllText(filePath, entry + Environment.NewLine);
                }
            }
            catch
            {
                // 日志系统自身的故障不应影响主流程
            }
        }
    }
}

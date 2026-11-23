using System.IO.Ports;
using System.Text.Json;
using TICP.Helper;

namespace TICP.Common
{
    /// <summary>
    /// 通信与采集参数。以前这些值散落在代码里（9600/8/N/1、从站 1、间隔 1000ms），
    /// 换设备就要改源码重新编译；现在统一落到 appconfig.json，改配置即可。
    /// </summary>
    public class AppConfig
    {
        public string PortName { get; set; } = "";
        public int BaudRate { get; set; } = 9600;
        public int DataBits { get; set; } = 8;
        public string StopBits { get; set; } = nameof(System.IO.Ports.StopBits.One);
        public string Parity { get; set; } = nameof(System.IO.Ports.Parity.None);
        public byte SlaveId { get; set; } = 1;
        public ushort StartAddress { get; set; } = 0;
        public int ChannelCount { get; set; } = 8;
        public int PollIntervalMs { get; set; } = 1000;
        public int ReconnectIntervalMs { get; set; } = 3000;
        public bool AutoReconnect { get; set; } = true;

        /// <summary>把字符串安全地转成串口枚举，配置写错时回退到默认值。</summary>
        public StopBits ToStopBits() =>
            Enum.TryParse<StopBits>(StopBits, true, out var v) ? v : System.IO.Ports.StopBits.One;

        public System.IO.Ports.Parity ToParity() =>
            Enum.TryParse<System.IO.Ports.Parity>(Parity, true, out var v) ? v : System.IO.Ports.Parity.None;

        /// <summary>把明显非法的配置夹回合理区间，避免 0ms 轮询把串口打成死循环。</summary>
        public void Clamp()
        {
            BaudRate = BaudRate is >= 1200 and <= 115200 ? BaudRate : 9600;
            DataBits = DataBits is >= 5 and <= 8 ? DataBits : 8;
            SlaveId = SlaveId == 0 ? (byte)1 : SlaveId;
            ChannelCount = ChannelCount is >= 1 and <= 8 ? ChannelCount : 8;
            PollIntervalMs = Math.Clamp(PollIntervalMs, 100, 60000);
            ReconnectIntervalMs = Math.Clamp(ReconnectIntervalMs, 500, 60000);
        }
    }

    public static class AppConfigStore
    {
        public static readonly string JsonPath =
            Path.Combine(AppContext.BaseDirectory, "appconfig.json");

        public static AppConfig Current { get; private set; } = new AppConfig();

        public static void Load()
        {
            try
            {
                if (File.Exists(JsonPath))
                {
                    var cfg = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(JsonPath));
                    if (cfg != null) Current = cfg;
                }
            }
            catch (Exception ex)
            {
                Log.Error("读取 appconfig.json 失败，改用默认参数", ex);
            }
            Current.Clamp();

            if (!File.Exists(JsonPath))
            {
                try { Save(); }
                catch (Exception ex) { Log.Error("生成默认 appconfig.json 失败", ex); }
            }
        }

        public static void Save()
        {
            Current.Clamp();
            File.WriteAllText(JsonPath,
                JsonSerializer.Serialize(Current, new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}

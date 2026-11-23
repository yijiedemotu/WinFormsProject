using System.Text.Json;
using WinFormsApp3.Model;

namespace WinFormsApp3.Common
{
    /// <summary>阈值页的完整配置：8 个通道 + 4 组刀补。</summary>
    public class yuzhiConfig
    {
        public List<yuzhiModel> channels { get; set; } =
            Enumerable.Range(0, 8).Select(_ => new yuzhiModel()).ToList();
        public string[] knifeSet { get; set; } = new string[4];
        public string[] knifeCurrent { get; set; } = new string[4];
        public string knifeMaxUp { get; set; } = "";
        public string knifeMaxDown { get; set; } = "";
    }

    public static class yuzhiCommon
    {
        /// <summary>固定在程序基目录，避免调试/双击 exe 打开两个不同的文件。</summary>
        public static readonly string JsonPath =
            Path.Combine(AppContext.BaseDirectory, "yuzhi.json");

        public static yuzhiConfig Current { get; private set; } = new yuzhiConfig();

        public static void Load()
        {
            try
            {
                if (File.Exists(JsonPath))
                {
                    var cfg = JsonSerializer.Deserialize<yuzhiConfig>(File.ReadAllText(JsonPath));
                    if (cfg != null) Current = cfg;
                }
            }
            catch (Exception ex)
            {
                gkHelper.Log.Error("读取 yuzhi.json 失败，改用默认配置", ex);
            }

            // 无论文件内容如何，都保证 8 个通道、4 组刀补，避免索引越界
            Current.channels ??= new List<yuzhiModel>();
            while (Current.channels.Count < 8) Current.channels.Add(new yuzhiModel());
            if (Current.channels.Count > 8) Current.channels = Current.channels.Take(8).ToList();
            for (int i = 0; i < 8; i++)
            {
                Current.channels[i] ??= new yuzhiModel();
                Current.channels[i].Normalize();
            }
            Current.knifeSet = Pad(Current.knifeSet);
            Current.knifeCurrent = Pad(Current.knifeCurrent);

            // 首次运行生成一份默认配置，方便用户直接打开查看格式
            if (!File.Exists(JsonPath))
            {
                try { Save(); }
                catch (Exception ex) { gkHelper.Log.Error("生成默认 yuzhi.json 失败", ex); }
            }
        }

        public static void Save()
        {
            string json = JsonSerializer.Serialize(Current,
                new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(JsonPath, json);
        }

        private static string[] Pad(string[]? src)
        {
            var arr = new string[4];
            if (src != null) Array.Copy(src, arr, Math.Min(src.Length, 4));
            for (int i = 0; i < 4; i++) arr[i] ??= "";
            return arr;
        }
    }
}

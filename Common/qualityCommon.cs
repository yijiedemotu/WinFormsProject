using System.Globalization;
using TICP.Model;

namespace TICP.Common
{
    /// <summary>单个通道一次采样的判定结果（纯数据，不碰控件）。</summary>
    public sealed class ChannelQuality
    {
        public int Channel { get; init; }
        public ushort Raw { get; init; }
        public double Value { get; init; }
        public bool? Ok { get; init; }          // null = 未标定，不参与合格判定
        public bool Calibrated { get; init; }
    }

    /// <summary>一次完整采样的判定汇总。</summary>
    public sealed class QualityResult
    {
        public double[] Values { get; init; } = Array.Empty<double>();
        public bool?[] Ok { get; init; } = Array.Empty<bool?>();
        public string NgChannels { get; init; } = "";
        public string Result { get; init; } = "未标定";
        public bool HasNg => NgChannels.Length > 0;
    }

    /// <summary>
    /// 判定服务：原始寄存器值 → 两点标定 → 上下限比较 → 汇总。
    /// 完全不依赖 WinForms，可在控制台工程里直接跑测试。
    /// </summary>
    public static class QualityCommon
    {
        public static QualityResult Evaluate(ushort[] raw, yuzhiConfig config)
        {
            int count = Math.Min(raw.Length, config?.channels?.Count ?? 0);
            var values = new double[count];
            var ok = new bool?[count];
            var ng = new List<int>();
            bool anyUncalibrated = false;

            for (int i = 0; i < count; i++)
            {
                yuzhiModel m = config!.channels[i];
                values[i] = m.TryConvert(raw[i], out double v) ? v : raw[i];
                ok[i] = m.IsCalibrated ? m.IsOk(values[i]) : null;
                if (ok[i] == null) anyUncalibrated = true;
                if (ok[i] == false) ng.Add(i + 1);
            }

            // 优先级：只要有一个通道超差就判不合格；否则有未标定的判未标定；全合格才合格
            string result = ng.Count > 0 ? "不合格" : anyUncalibrated ? "未标定" : "合格";
            return new QualityResult
            {
                Values = values,
                Ok = ok,
                NgChannels = string.Join(",", ng),
                Result = result
            };
        }

        /// <summary>把 8 个通道的物理量转成可入库的字符串（不变文化，保证曲线页能解析）。</summary>
        public static string[] ToStrings(double[] values) => values
            .Select(v => v.ToString("F2", CultureInfo.InvariantCulture))
            .ToArray();
    }
}

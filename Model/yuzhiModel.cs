using System.Globalization;
using System.Text.Json.Serialization;

namespace TICP.Model
{
    /// <summary>
    /// 单个通道的标定参数与合格阈值。
    /// 数值以字符串存储（兼容既有 JSON），读写统一走不变文化，避免中文系统格式差异。
    /// </summary>
    public class yuzhiModel
    {
        public string liangjuUp { get; set; } = "";
        public string liangjuDown { get; set; } = "";
        public string jiaozhunUp { get; set; } = "";
        public string jiaozhunDown { get; set; } = "";
        public string celiangUp { get; set; } = "0";
        public string celiangDown { get; set; } = "0";

        [JsonIgnore]   // 只读计算属性，不要写进 yuzhi.json
        public bool IsCalibrated =>
            TryParse(liangjuUp, out _) && TryParse(liangjuDown, out _) &&
            TryParse(jiaozhunUp, out _) && TryParse(jiaozhunDown, out _);

        /// <summary>清洗脏配置：非法数值回退为空串/0，保证后续计算不抛异常。</summary>
        public void Normalize()
        {
            liangjuUp = Keep(liangjuUp);
            liangjuDown = Keep(liangjuDown);
            jiaozhunUp = Keep(jiaozhunUp);
            jiaozhunDown = Keep(jiaozhunDown);
            celiangUp = Keep(celiangUp, "0");
            celiangDown = Keep(celiangDown, "0");
        }

        /// <summary>两点线性标定：把原始计数映射到物理量。参数缺失或量程非法返回 false。</summary>
        public bool TryConvert(double raw, out double value)
        {
            value = 0;
            if (!TryParse(liangjuUp, out double rawHigh) ||
                !TryParse(liangjuDown, out double rawLow) ||
                !TryParse(jiaozhunUp, out double valueHigh) ||
                !TryParse(jiaozhunDown, out double valueLow))
                return false;

            double rawSpan = rawHigh - rawLow;
            double valueSpan = valueHigh - valueLow;
            if (rawSpan == 0 || valueSpan == 0) return false;

            value = valueLow + (raw - rawLow) * valueSpan / rawSpan;
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        /// <summary>合格判定：换算值是否落在测量上下限内。</summary>
        public bool IsOk(double value) =>
            TryParse(celiangDown, out double low) && TryParse(celiangUp, out double high)
            && value >= low && value <= high;

        private static bool TryParse(string? s, out double v) =>
            double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out v)
            && !double.IsNaN(v) && !double.IsInfinity(v);

        private static string Keep(string? s, string fallback = "") =>
            TryParse(s, out double v) ? v.ToString(CultureInfo.InvariantCulture) : fallback;
    }
}

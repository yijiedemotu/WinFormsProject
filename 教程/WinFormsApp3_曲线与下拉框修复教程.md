# WinFormsApp3 曲线、下拉框与 DPI 异常修复教程

> 对应 2026-11-22 ~ 11-23 已落盘的修复（11-23 按用户要求把 DPI 恢复为 `PerMonitorV2`，改用字体参数消除异常）。代码**已经改好**，本文按「现象 → 证据 → 原理 → 改法 → 练习」讲清楚每一处，方便你自己复现、理解和以后独立排同类问题。
>
> 工程：`WinFormsProject`（项目名/命名空间仍是 `WinFormsApp3`）。

---

## 0. 三个现象

| 现象 | 出现位置 |
|---|---|
| 弹窗「界面线程异常: Parameter is not valid.」 | 查询统计页，操作时偶发 |
| 下拉框点开是空的 | 数据测量页「串口」、查询统计页「产品名称 / 通道选择」、趋势曲线页「通道选择」 |
| 趋势曲线一片空白 | 趋势曲线页 |

---

## 1. 排查方法论：先看日志，别猜

上一轮在 `Program.cs` 加过全局异常钩子，异常会写进 `Logs/yyyy-MM-dd/Error/log_yyyy-MM-dd.txt`。这次直接拿到了完整堆栈：

```
System.ArgumentException: Parameter is not valid.
   at System.Drawing.Font.ToLogFont(LOGFONT& logFont, Graphics graphics)
   at System.Drawing.Font.ToHfont()
   at System.Windows.Forms.Control.get_FontHandle()
   at System.Windows.Forms.Control.SetWindowFont()
   at System.Windows.Forms.Control.OnFontChanged(EventArgs e)
   at System.Windows.Forms.Control.SetScaledFont(Font scaledFont, Boolean raiseOnFontChangedEvent)
   at System.Windows.Forms.Control.WmDpiChangedBeforeParent(Message& m)
   at System.Windows.Forms.Control.WndProc(Message& m)
   at System.Windows.Forms.UserControl.WndProc(Message& m)
```

**读堆栈的方法**：从下往上看调用来源，从上往下看崩溃点。这里最下面 `UserControl.WndProc` 收到 `WM_DPICHANGED` 系列消息，WinForms 要按新 DPI **重新缩放字体**（`SetScaledFont`），结果 `Font.ToHfont()` 创建 GDI+ 字体句柄失败 → `Parameter is not valid`。

**结论**：这不是数据库、不是查询 SQL、也不是 ScottPlot 数据问题，是 **DPI 缩放字体**。触发条件是进程处于 `PerMonitorV2`（逐显示器 DPI 感知）：窗口跨不同缩放的显示器、或系统缩放变化时，每个 `UserControl` 都会收到 DPI 变化消息并尝试重算字体。而本项目所有 Designer 里字体都是

```csharp
new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134)
```

带 `gdiCharSet = 134` 的 Point 字体，在这条缩放路径上容易创建失败。

> 顺带一提：`Application.Run(new Form1())` 被 VS 高亮，不代表异常就在这一行——GUI 消息循环里的未处理异常，调试器通常都归因到 `Application.Run`。

---

## 2. Bug A：`Parameter is not valid`

### 最终改法（2026-11-23 调整：恢复界面观感）

先试过把 `ApplicationHighDpiMode` 统一成 `SystemAware`，异常确实没了，但 **SystemAware 下组件布局比例失调、界面不美观**。所以最终改成三步：

**1. 恢复原来的 `PerMonitorV2`** —— csproj 还原成改动前的写法（含原有两项与注释），逐显示器 DPI 感知回来，组件比例恢复正常。

**2. 去掉字体里的 `gdiCharSet=134`** —— 6 个 Designer 共 114 处：

```csharp
// 改前
new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134)
// 改后
new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point)
```

`134` 是 `gdiCharSet`（GB2312 字符集）。WinForms 在 `SetScaledFont` 里按新 DPI 重建字体时，带这个参数的 Point 字体会让 `Font.ToHfont()` 创建 GDI+ 句柄失败；去掉后走 4 参数重载（默认字符集），中文显示不变，缩放路径正常。**这才是治本**：既保留想要的 DPI 模式，又消除异常触发条件。

**3. `Program.cs` 兜底再收一层** —— 对 `WmDpiChangedBeforeParent` / `Font.ToHfont` 这条已知无害的缩放异常，只写日志不弹窗（控件会继续用原字体显示），避免偶发打扰：

```csharp
string stack = ex.ToString();
if (stack.Contains("WmDpiChangedBeforeParent") || stack.Contains("Font.ToHfont"))
    return;   // 前面已经 Log.Error 记过了
```

### 原理

- `ApplicationHighDpiMode` 决定 `ApplicationConfiguration.Initialize()` 生成的 `Application.SetHighDpiMode(...)` 参数。
- **PerMonitorV2**：每个显示器独立 DPI，窗口跨屏时控件按实际显示器重算字体和布局 → 组件比例协调，但会发 `WM_DPICHANGED`。
- **SystemAware**：进程启动时按系统 DPI 缩放一次，之后不再逐显示器重算 → 不崩，但配合 `AutoScaleMode.None` + Point 字体，在 150% 缩放下容易出现「字放大、控件不放大、互相压住」的观感问题。
- 正确顺序是：**先恢复想要的 DPI 模式，再修触发异常的字体参数**，而不是为了躲异常去降级 DPI 模式。

> 本项目 `AutoScaleMode = None`、字体用 Point，在 150% 缩放下本来就有「字放大、控件不放大」的显示问题（见 `WinFormsApp3_字体固定教程.md`）。那是观感问题，和这次的崩溃是两回事，不要混在一起改。

### 保留的兜底

`Program.cs` 里的全局钩子不要删：

```csharp
Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
Application.ThreadException += (s, e) => Report("界面线程异常", e.Exception);
AppDomain.CurrentDomain.UnhandledException += (s, e) =>
    Report("后台线程异常", e.ExceptionObject as Exception);
```

它把任何漏网异常写进日志并弹友好提示。这次能定位根因，全靠它留下的堆栈。

---

## 3. Bug B：下拉框点开没有东西

### 根因

四个下拉框在设计器里都只有 `DataSource = null`，**从来没有 `Items.Add(...)`**：

| 控件 | 属于 | 应该显示什么 |
|---|---|---|
| `mainView.uiComboBox1` | 数据测量页 | 可用串口号 |
| `totalView.uiComboBox1` | 查询统计页 | 库里已有的产品名称 |
| `totalView.uiComboBox2` | 查询统计页 | 1~8 通道 |
| `plotView.uiComboBox1` | 趋势曲线页 | 1~8 通道 |

`Sunny.UI.UIComboBox.Items` 返回的是标准 `ListBox.ObjectCollection`（源码 `UIComboBox.cs:797`），所以 `Items.Add(object)` / `Items.Clear()` 直接可用。

### 改法 1：数据层先提供产品名与时间范围（`Helper/sqlHelper.cs`）

```csharp
/// <summary>库中最早 / 最晚检测时间，供日期控件设置默认区间。</summary>
public (DateTime? min, DateTime? max) GetTimeRange()
{
    using var connection = new SqliteConnection(_connectionString);
    connection.Open();
    var cmd = connection.CreateCommand();
    cmd.CommandText = "SELECT MIN(testTime), MAX(testTime) FROM DataModel";
    using var reader = cmd.ExecuteReader();
    if (!reader.Read()) return (null, null);
    DateTime? min = Parse(reader.IsDBNull(0) ? "" : reader.GetString(0));
    DateTime? max = Parse(reader.IsDBNull(1) ? "" : reader.GetString(1));
    return (min, max);
}

private static DateTime? Parse(string s) =>
    DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture,
        DateTimeStyles.None, out DateTime t) ? t : null;

/// <summary>查询库里出现过的产品名称，供查询页下拉框使用。</summary>
public List<string> GetProjectNames()
{
    var list = new List<string>();
    using var connection = new SqliteConnection(_connectionString);
    connection.Open();
    var cmd = connection.CreateCommand();
    cmd.CommandText =
        "SELECT DISTINCT projectName FROM DataModel " +
        "WHERE projectName IS NOT NULL AND TRIM(projectName) <> '' " +
        "ORDER BY projectName";
    using var reader = cmd.ExecuteReader();
    while (reader.Read()) list.Add(reader["projectName"] as string ?? "");
    return list;
}
```

`MIN/MAX(testTime)` 能对 TEXT 用，是因为库里时间格式统一为 `yyyy-MM-dd HH:mm:ss`——**字典序等于时间序**。这也是为什么之前反复强调时间格式必须统一。

### 改法 2：测量页串口（`View/mainView.cs`）

```csharp
private void FillPorts()
{
    try
    {
        uiComboBox1.Items.Clear();
        foreach (string port in gkHelper.ModbusRtuClient.GetAvailablePorts())
            uiComboBox1.Items.Add(port);
        if (uiComboBox1.Items.Count > 0) uiComboBox1.SelectedIndex = 0;
    }
    catch (Exception ex)
    {
        // 枚举串口失败不能拖垮页面加载（如无串口驱动、非 Windows 运行时）
        gkHelper.Log.Error("枚举串口失败", ex);
    }
}

private void mainView_Load(object sender, EventArgs e)
{
    FillPorts();
    // ... 原来创建 8 个 simpleView 的代码
}
```

两个要点：
- `GetAvailablePorts()`（`ModbusRtuClient.cs:584`）内部是 `SerialPort.GetPortNames()`，可能抛 `PlatformNotSupportedException` / `UnauthorizedAccessException`。**放在 `Load` 里的代码一定要 try/catch**，否则一个串口异常就让整个窗体打不开。
- 点「连接」时补空值校验，避免出现日志里那种 `The PortName cannot be empty`：

```csharp
string comName = uiComboBox1.Text.Trim();
if (string.IsNullOrEmpty(comName))
{
    MessageBox.Show("请先选择串口号", "提示",
        MessageBoxButtons.OK, MessageBoxIcon.Warning);
    return;
}
```

### 改法 3：查询页两个下拉框（`View/totalView.cs`）

```csharp
private void FillComboBoxes()
{
    uiComboBox2.Items.Clear();
    for (int i = 1; i <= 8; i++) uiComboBox2.Items.Add(i.ToString());

    uiComboBox1.Items.Clear();
    uiComboBox1.Items.Add("");                 // 空 = 不限产品名称
    try
    {
        foreach (string name in mainCommon.sqlHelper.GetProjectNames())
            uiComboBox1.Items.Add(name);
    }
    catch (Exception ex)
    {
        gkHelper.Log.Error("加载产品名称下拉框失败", ex);
    }
}
```

在构造函数里 `FillComboBoxes();` 之后调用。空字符串那一项很关键：`GetByConditions` 会把空字符串当成「不加这个条件」，用户不选产品名时才能查到全部。

顺手把查询按钮的两个坑也修了：

```csharp
const string f = "yyyy-MM-dd";
string start = uiDatePicker1.Value.ToString(f, CultureInfo.InvariantCulture);
string end   = uiDatePicker2.Value.ToString(f, CultureInfo.InvariantCulture);
var datas = mainCommon.sqlHelper.GetByConditions(start, end, uiComboBox1.Text.Trim());

// int.TryParse 代替 Convert.ToInt16：下拉框里出现非数字时不会抛 FormatException
if (int.TryParse(uiComboBox2.Text.Trim(), out int ch) && ch >= 1 && ch <= 8)
    ShowOnlySpecifiedColumns(dataGridView1, "Column1", "Column2", "Column3", $"Column{ch + 3}");
else
    ShowAllColumns(dataGridView1);
```

只传 10 位日期是有意为之：`SqlHelper.NormalizeTime` 会把起始补成 `00:00:00`、结束补成 `23:59:59`，这样「结束日期当天」的数据不会被漏掉（之前踩过 `testTime <= '2026-11-13'` 把当天全排除的坑）。

---

## 4. Bug C：趋势曲线一片空白

这一条其实是 **四个原因叠加**，逐个解决才有曲线：

| # | 原因 | 修复 |
|---|---|---|
| 1 | `plotView.Designer.cs` 里「查询」按钮**没有绑定 Click**，`plotView.cs` 是空壳 | 构造函数里 `uiButton1.Click += uiButton1_Click;` |
| 2 | 通道下拉框没有 Items，且 `Text = "请选择通道"` | 填 1~8 并 `SelectedIndex = 0` |
| 3 | 两个 `UIDatePicker` 默认值写死 `2026-11-12`，库里数据是 11-13 以后 | `SetDefaultDateRange()` 用 `GetTimeRange()` 设成真实数据范围 |
| 4 | 从来没有把数据库数据交给 `formsPlot1`，也没调用 `Refresh()` | `LoadPlot()` 查库 → 转数组 → `Plot.Add.Scatter` → `Refresh()` |

### 完整 `View/plotView.cs` 关键部分

```csharp
public plotView()
{
    InitializeComponent();

    uiComboBox1.Items.Clear();
    for (int i = 1; i <= 8; i++) uiComboBox1.Items.Add(i.ToString());
    uiComboBox1.SelectedIndex = 0;
    SetDefaultDateRange();

    uiButton1.Click += uiButton1_Click;         // Designer 没绑，这里补
}

// 首次打开页面就画一次，避免曲线区一片空白
private void plotView_Load(object sender, EventArgs e) => LoadPlot();
private void uiButton1_Click(object? sender, EventArgs e) => LoadPlot();

private void SetDefaultDateRange()
{
    try
    {
        var (min, max) = mainCommon.sqlHelper.GetTimeRange();
        if (min != null) uiDatePicker1.Value = min.Value.Date;
        if (max != null) uiDatePicker2.Value = max.Value.Date;
    }
    catch (Exception ex) { gkHelper.Log.Error("设置曲线默认日期失败", ex); }
}

private void LoadPlot()
{
    try
    {
        const string f = "yyyy-MM-dd";
        string start = uiDatePicker1.Value.ToString(f, CultureInfo.InvariantCulture);
        string end   = uiDatePicker2.Value.ToString(f, CultureInfo.InvariantCulture);
        var rows = mainCommon.sqlHelper.GetByConditions(start, end, "");

        if (!int.TryParse(uiComboBox1.Text.Trim(), out int ch) || ch < 1 || ch > 8) ch = 1;

        var xs = new List<double>();
        var ys = new List<double>();
        foreach (var r in rows)
        {
            if (!DateTime.TryParseExact(r.testTime, "yyyy-MM-dd HH:mm:ss",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime t)) continue;
            string raw = ChannelValue(r, ch);
            if (!double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out double v)) continue;
            xs.Add(t.ToOADate());       // ScottPlot 时间轴用 OLE 自动化日期数值
            ys.Add(v);
        }

        formsPlot1.Plot.Clear();
        if (xs.Count > 0)
        {
            var line = formsPlot1.Plot.Add.Scatter(xs.ToArray(), ys.ToArray());
            line.LegendText = $"通道{ch}";            // 5.x 里 Label 已过时
            formsPlot1.Plot.Axes.DateTimeTicksBottom();
        }
        formsPlot1.Plot.Title(xs.Count == 0 ? "该条件下无数据" : $"通道{ch} 趋势（{xs.Count} 个点）");
        formsPlot1.Plot.XLabel("检测时间");
        formsPlot1.Plot.YLabel("测量值");
        formsPlot1.Refresh();
        gkHelper.Log.Info($"趋势曲线绘制完成：通道{ch}，{xs.Count} 个点，区间 {start} ~ {end}");
    }
    catch (Exception ex)
    {
        // 曲线是可选功能：失败只记日志 + 标题提示，不弹窗打断其它页面
        gkHelper.Log.Error("绘制趋势曲线失败", ex);
        try { formsPlot1.Plot.Title("绘制失败：" + ex.Message); formsPlot1.Refresh(); }
        catch { }
    }
}

private static string ChannelValue(dataModel m, int ch) => ch switch
{
    1 => m.data1, 2 => m.data2, 3 => m.data3, 4 => m.data4,
    5 => m.data5, 6 => m.data6, 7 => m.data7, 8 => m.data8,
    _ => ""
};
```

### 曲线的数据流（记住这条链）

```
UIDatePicker(日期) + UIComboBox(通道)
        ↓
GetByConditions(start, end, "")   ← 复用查询页同一套归一化
        ↓  List<dataModel>
按通道取 dataN → double.TryParse(InvariantCulture)
时间 → DateTime.TryParseExact → ToOADate()
        ↓  xs[] / ys[]
Plot.Clear() → Plot.Add.Scatter(xs, ys) → Axes.DateTimeTicksBottom()
        ↓
formsPlot1.Refresh()   ← 不调用它，界面永远不更新
```

三个容易忘的点：
- **`Refresh()` 必须调用**，ScottPlot 不会自动重绘；
- `data1..data8` 在 `dataModel` 里是 **string**，必须 `double.TryParse` 才能给图表；
- 一切数字/日期解析都用 `CultureInfo.InvariantCulture`，否则中文系统的逗号小数点、斜杠日期会把数据悄悄丢掉。

---

## 5. 改动清单

| 文件 | 改动 |
|---|---|
| `WinFormsApp3.csproj` | **恢复改动前的 `PerMonitorV2`**（含原有两项与注释），界面观感还原 |
| 6 个 `*.Designer.cs` | 114 处 `new Font(..., 134)` 去掉 `gdiCharSet` 参数 |
| `Program.cs` | `Report()` 对已知 DPI 字体缩放异常只记日志、不弹窗 |
| `Helper/sqlHelper.cs` | 新增 `GetTimeRange()`、`GetProjectNames()`、`Parse()` |
| `View/mainView.cs` | 新增 `FillPorts()`（带 try/catch）、串口空值校验 |
| `View/totalView.cs` | 新增 `FillComboBoxes()`、`SetDefaultDateRange()`；查询改传 10 位日期；`int.TryParse` 代替 `Convert.ToInt16` |
| `View/plotView.cs` | 通道下拉框、默认日期、按钮事件、`LoadPlot()` 完整绘制逻辑 |
| `教程/WinFormsApp3_曲线与下拉框修复教程.md` | 本文档 |

`Program.cs`、`Form1.cs`、`yvzhiView.cs` 等上一轮的改动保持不变。

---

## 6. 验证步骤（自己动手）

1. `dotnet build` → 应 **0 错误**（只剩 6 个 `NU1701`，是 OpenTK / SkiaSharp 第三方包的目标框架提示，不影响运行）。
2. F5 启动，依次点 4 个导航按钮，不应再弹「界面线程异常」。
3. 数据测量页点开串口下拉框 → 应看到 `COM1`、`COM2`…；不选直接点「连接」→ 弹「请先选择串口号」而不是报错。
4. 查询统计页 → 产品名称下拉框应列出库里出现过的值（外加一个空项），通道选择应有 1~8；日期默认应覆盖库里数据。
5. 趋势曲线页 → 打开就有折线；换通道、改日期再点「查询」，曲线应变化。
6. 看 `Logs/yyyy-MM-dd/Info/` 里应有 `趋势曲线绘制完成：通道N，X 个点...`；`Error/` 里不应有「绘制趋势曲线失败」。

> 若之前那个卡在异常弹窗的调试进程还开着，先关掉再 F5，否则 `WinFormsApp3.exe` 被占用，build 会报 `MSB3027`。

---

## 7. 练习题

1. **理解 DPI**：现在已是 `PerMonitorV2`。把窗口从主屏拖到不同缩放的副屏（或改系统缩放），确认组件比例正常且 `Logs/Error/` 不再出现 `Font.ToHfont` 堆栈；再把某个 Designer 的字体临时加回 `, 134)` 复现异常，体会 `gdiCharSet` 才是触发条件。
2. **下拉框**：在 `GetProjectNames()` 里把 `TRIM(projectName) <> ''` 去掉，观察空字符串会不会混进下拉框；想想为什么查询页要额外 `Items.Add("")`。
3. **曲线**：把 `formsPlot1.Refresh()` 注释掉，改日期点「查询」，观察图表是否更新——亲身体会"数据变了但界面没重绘"。
4. **格式**：故意把某条记录的 `testTime` 改成 `2026/11/13 10:00:00`，看 `TryParseExact` 会不会跳过这条（应该跳过并在日志里看到点数变少）。
5. **进阶**：把 `LoadPlot()` 里的 `channel` 从下拉框改成 8 条曲线叠加（循环 1~8 各 `Add.Scatter` 一次，设不同 `LegendText`），做成八通道对比曲线。

---

## 8. 仍然遗留

- 阈值设置页右半区 `uiPanel4→13→14→15` 仍是四层 `Dock=Fill` 嵌套、控件坐标重叠，功能已绑定但视觉上互相遮挡，需要在设计器里理顺。
- `yuzhi.json` 未加入 `.gitignore`；导出 Excel 按钮仍是空的。
- `NU1701` 警告来自 `OpenTK` / `SkiaSharp.Views.WindowsForms` 这类 .NET Framework 包，若想让 build 完全 0 警告，可在 csproj 里加 `<NoWarn>NU1701</NoWarn>` 并注明原因。

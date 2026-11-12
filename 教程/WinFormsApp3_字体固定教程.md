# WinFormsApp3 字体「运行后变大」修改教程

> ⚠️ **2026-11-12 14:18 更新：用户决定放弃修复，工程已全部还原到最初状态。**
> 本文仅作为排查过程与原理记录保留；第 11 节「已写入工程」不再成立，**不要按它改代码**。

> 适用工程：`D:\上位机\项目学习-墨竹\手敲项目\WinFormsApp3`
> 实测日期：2026-11-12（本机 Windows 显示缩放 150% / 144 DPI）
> 本教程只讲原理 + 给可照抄的改法，**没有改动工程里的任何源码**，改不改、改哪条路线由你决定。

---

## 0. 先说结论（一定要先读）

**代码里的字号从头到尾都没有变过。** `12F / 10F / 9F` 在构造函数里是这些数，窗口显示完、改尺寸、最大化之后还是这些数。

变的是**字的物理像素大小**：

| 位置 | DPI | `12F` 实际渲染 | `10F` 实际渲染 | `9F` 实际渲染 |
|---|---|---|---|---|
| VS 设计器（被强制 96 DPI） | 96 | 16 px | 13.3 px | 12 px |
| 程序运行（系统 150%） | 144 | **24 px** | **20 px** | **18 px** |

比例恒为 `144 / 96 = 1.5`。也就是说，你看到的「变化后的字体大小」= **24 / 20 / 18 物理像素**，
换算成设计器 96 DPI 下的点数就是 **18pt / 15pt / 13.5pt**。

> 所以严格讲：**不存在一个「运行后被改成的字号数字」可以抄回 Designer.cs**，
> 因为 `Font.Size` 根本没被改。要消除这个变化，本质是
> **让运行时字体的物理像素回到设计时的大小**。

## 0.1 实测修正（请优先看这一节）

初稿给的「路线 ②：改成 `GraphicsUnit.Pixel`」和「`AutoScaleMode.Dpi`」在本工程上都**实测失败**，不要照抄：

| 试验 | 实测结果 |
|---|---|
| 三份 Designer 共 31 处改成 `24F / 20F / 18F Pixel` | 字反而更大（约 36px），「产品名称:」只剩一个「产」，按钮文字重叠 |
| `AutoScaleMode = Dpi` + `AutoScaleDimensions = 96` | 启动时不触发缩放，窗体仍 1055×705、字号仍 12F，画面无变化 |
| `UIStyles.DPIScale = true` | **成功**：12F→8F、10F→6.67F、9F→6F，标签完整、表头不再竖排换行 |

原因：`GraphicsUnit.Pixel` 并不能让 WinForms 字体绕开 PerMonitorV2 的 DPI 缩放，
在 144 DPI 下会被再放大一次；`AutoScaleMode.Dpi` 只在**运行中** DPI 变化时才生效，
启动时不会重新布局。单独测 `Font.GetHeight()` 看不出这一点，必须截图对比才能发现。

**最终采用的修复（已写入工程）**：`Program.cs` 打开 SunnyUI 官方 DPI 字体补偿，
`csproj` 把设计器固定回 96 DPI，三个 `.Designer.cs` 一个字号都不改。详见第 11 节。

---

## 1. 实测证据（我是怎么确认的）

用一个临时探针程序引用本工程、真实调用 `Program.Main` 启动，再递归打印每个控件的 `Font`：

```
--- 启动后静态值 ---
UIStyles.DPIScale        = False
UIStyles.ZoomScale       = False
UIDPIScale.SystemDPIScale = 1.5
Graphics.FromHwnd(0).DpiX = 144

=== B. 显示后(Shown 完成) ===
Form: Size=1055x705  DeviceDpi=144  AutoScaleMode=None  Font=宋体/12.00Point
  UIPanel uiPanel2          Font=宋体/12.00Point
    mainView                Font=宋体/12.00Point
      UITitlePanel          Font=宋体/12.00Point
        DataGridView        Font=宋体/12.00Point
      UITableLayoutPanel    Font=宋体/12.00Point
        simpleView          UITextBox1..6 = 宋体/10.00Point, uiTextBox7 = 宋体/12.00Point
  UIPanel uiPanel1          UISymbolButton1..4 = 宋体/12.00Point, TipsFont=宋体/9.00Point

=== C. 改成 1400x900 后 ===   Font 全部不变
=== D. 最大化 2560x1368 后 === Font 全部不变
```

再把 `ApplicationConfiguration.Initialize()` 的生成代码导出来看，它只有三行：

```csharp
Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);
Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
```

**没有任何 `SetDefaultFont`，也没有任何自动缩放字体的调用。**

### 附带查出的三个 csproj 问题

`WinFormsApp3.csproj` 第 8~15 行：

```xml
<ApplicationHighDpiMode>SystemAware</ApplicationHighDpiMode>   <!-- 第 8 行，被第 11 行覆盖 -->
<ForceDesignerDpiUnaware>true</ForceDesignerDpiUnaware>        <!-- 第 9 行 -->
<ApplicationHighDpiMode>PerMonitorV2</ApplicationHighDpiMode>  <!-- 第 11 行，最终生效 -->
<ForceDesignerDPIUnaware>true</ForceDesignerDPIUnaware>        <!-- 第 13 行，和第 9 行是同一个属性 -->
<ApplicationAutoScaleMode>Dpi</ApplicationAutoScaleMode>       <!-- 第 15 行，WinForms 不认识，完全无效 -->
```

- MSBuild 属性名**不区分大小写**，所以第 9、13 行是同一个 `ForceDesignerDPIUnaware` 的重复赋值；
  微软文档明确：这个属性**只影响 VS 设计器**（把它固定在 96 DPI），**不影响程序运行**。
- `ApplicationHighDpiMode` 写了两次，最后一次 `PerMonitorV2` 生效（生成代码已证实）。
- `ApplicationAutoScaleMode` 不是合法属性，生成代码里完全没有它，注释里"关闭旧的字体自动缩放"是无效的。

**这就是"设计器里字小、运行起来字大"的根因：设计器被锁在 100%，程序跑在 150%。**
另外 `AutoScaleMode = None`（三个窗体都设了）表示 WinForms 不再按字体做布局缩放，
所以不会有任何"代码字号"被改写。

---

## 2. 原理：为什么 12pt 会渲染成两种大小

`Font` 用 `GraphicsUnit.Point` 时，字号是**物理长度单位**（1 pt = 1/72 英寸）：

```
实际像素 = 字号(pt) × 当前 DPI / 72
12pt @ 96DPI  = 12 × 96/72  = 16 px
12pt @ 144DPI = 12 × 144/72 = 24 px   ← 你运行时看到的
```

实测 `Font.GetHeight()`（行高，含 leading）：

```
12pt : GetHeight(96)=18.25   GetHeight(144)=27.38     → 随 DPI 放大 1.5 倍
24px : GetHeight(96)=27.38   GetHeight(144)=27.38     → 用 Pixel 单位后完全不随 DPI 变
```

用 `GraphicsUnit.Pixel` 建字体，字体被钉死在物理像素上，任何 DPI 下都是同一个大小。
这就是"把变化后的字体大小写死"的技术手段。

顺带澄清两个常被误会的点：

- `ZoomScaleRect = new Rectangle(22, 22, 800, 450)`（Form1.Designer.cs:131）**目前不起作用**。
  SunnyUI 的 `UIForm.SetZoomScale()` 开头就判断 `!UIStyles.DPIScale || !UIStyles.ZoomScale` 直接返回，
  而这两个静态开关默认都是 `false`（实测值见第 1 节）。所以改窗口大小、最大化都不会改字体。
- SunnyUI 的 `UIBaseForm.OnShown()` 会调用 `SetDPIScale()`，但条件是
  `UIStyles.DPIScale && (SystemDPIScale > 1 || UIStyles.GlobalFont)`，默认 `DPIScale=false` → 一样直接返回。

---

## 3. 三条路线，先选一条

| 路线 | 你要的结果 | 改动量 | 代价 |
|---|---|---|---|
| **① 对齐设计器 DPI** | 保留现在运行时的样子，让设计器也显示这么大；字号一个都不用改 | 只删 csproj 两行 | 设计器换成 150% 渲染后，可能按新 DPI 重新序列化控件坐标；且运行时裁切问题仍在 |
| **② 像素单位锁死** | 把 24/20/18 px 写进 `.Designer.cs` | 三个 Designer 共 31 处替换 | **实测不可用**：字被二次放大，画面更糟，见 0.1 节 |
| **③ 打开 SunnyUI 官方 DPI 补偿**（已采用） | 运行时字号 ÷1.5，物理大小回到设计时的 16/13.3/12 px，布局恢复正常 | Program.cs 加 1 行 + csproj 加 1 行 | 运行时字比原来小；`TipsFont` 与控件内部子字体不参与补偿 |

> 只有 ③ 经截图验证能真正修好裁切。② 已作废，① 只是让设计器跟着变形、不解决溢出。
> **② 和 ③ 不要同时用**：③ 会把 ② 写死的像素字号再除以 1.5。
> 如果坚持保留"放大后的大字"，唯一正确做法是**把所有控件的 Location/Size 同步 ×1.5**，
> 让布局和字一起放大（改动量大，本工程未采用）。

---

## 4. 路线 ①：让设计器也按系统 DPI 渲染（零字体改动）

### 第 1 步：备份

```
copy Form1.Designer.cs       Form1.Designer.cs.bak
copy View\mainView.Designer.cs   View\mainView.Designer.cs.bak
copy View\simpleView.Designer.cs View\simpleView.Designer.cs.bak
```

### 第 2 步：改 `WinFormsApp3.csproj`

把第 8~15 行整段替换成：

```xml
		<!-- 运行时：逐显示器 DPI 感知（只写一次） -->
		<ApplicationHighDpiMode>PerMonitorV2</ApplicationHighDpiMode>
		<!-- 不再强制设计器 96 DPI：让 VS 设计器跟随系统缩放，
		     设计视图与运行效果一致（VS 2022 17.8+ / VS 18 支持） -->
```

即：删掉 `SystemAware` 那行、删掉两行 `ForceDesigner...`、删掉无效的 `ApplicationAutoScaleMode`。

### 第 3 步：重启设计器

关闭所有窗体设计器标签页 → 重新生成 → 重新打开 `Form1`。
此时设计器按 150% 渲染，字体视觉大小 = 运行时的大小，`12F / 10F / 9F` 一个字都不用改。

### 注意

- 若重新打开设计器后发现 `.Designer.cs` 里的 `Location / Size` 被 VS 自动改了，用备份还原，
  并且不要在设计器里拖动控件（这是 `ForceDesignerDPIUnaware` 原本要防的问题）。
- 这个属性只影响 VS 设计器，导出的生成代码仍然是 `PerMonitorV2`，运行行为不变。

---

## 5. 路线 ②：把「变化后的字号」写成像素单位（逐行清单）

### 5.1 换算表（背下这三行就够）

| Designer 里的原值 | 运行后实际大小 | 改成（DPI 无关） |
|---|---|---|
| `12F ... GraphicsUnit.Point` | 24 px | `24F ... GraphicsUnit.Pixel` |
| `10F ... GraphicsUnit.Point` | 20 px | `20F ... GraphicsUnit.Pixel` |
| `9F ... GraphicsUnit.Point`  | 18 px | `18F ... GraphicsUnit.Pixel` |

`FontStyle.Bold` / `gdiCharSet = 134` 全部原样保留，只改**数字 + 单位**。

### 5.2 三个文件的替换清单（共 31 处）

| 文件 | 原字符串 | 新字符串 | 处数 |
|---|---|---|---|
| `Form1.Designer.cs` | `new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134)` | `new Font("宋体", 24F, FontStyle.Regular, GraphicsUnit.Pixel, 134)` | 6 |
| `Form1.Designer.cs` | `new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134)` | `new Font("宋体", 18F, FontStyle.Regular, GraphicsUnit.Pixel, 134)` | 4（都是 `TipsFont`） |
| `View\mainView.Designer.cs` | `new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134)` | `new Font("宋体", 24F, FontStyle.Regular, GraphicsUnit.Pixel, 134)` | 8 |
| `View\mainView.Designer.cs` | `new Font("宋体", 12F, FontStyle.Bold, GraphicsUnit.Point, 134)` | `new Font("宋体", 24F, FontStyle.Bold, GraphicsUnit.Pixel, 134)` | 3（三个 `UILabel`） |
| `View\mainView.Designer.cs` | `new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134)` | `new Font("宋体", 18F, FontStyle.Regular, GraphicsUnit.Pixel, 134)` | 3（都是 `TipsFont`） |
| `View\simpleView.Designer.cs` | `new Font("宋体", 10F, FontStyle.Regular, GraphicsUnit.Point, 134)` | `new Font("宋体", 20F, FontStyle.Regular, GraphicsUnit.Pixel, 134)` | 6（`uiTextBox1`~`uiTextBox6`） |
| `View\simpleView.Designer.cs` | `new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134)` | `new Font("宋体", 24F, FontStyle.Regular, GraphicsUnit.Pixel, 134)` | 1（`uiTextBox7`） |

**不需要改的地方**：`dataGridView1`、`uiLedBulb1/2`、`uiTableLayoutPanel1` 都没有显式设 `Font`，
它们继承父容器的字体，父容器一改就跟着改。

### 5.3 例子（改前 / 改后）

`Form1.Designer.cs` 第 93~104 行：

```csharp
// ---------- 改前 ----------
uiSymbolButton1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
...
uiSymbolButton1.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);

// ---------- 改后 ----------
uiSymbolButton1.Font = new Font("宋体", 24F, FontStyle.Regular, GraphicsUnit.Pixel, 134);
...
uiSymbolButton1.TipsFont = new Font("宋体", 18F, FontStyle.Regular, GraphicsUnit.Pixel, 134);
```

`View\simpleView.Designer.cs` 第 45 行：

```csharp
// 改前
uiTextBox1.Font = new Font("宋体", 10F, FontStyle.Regular, GraphicsUnit.Point, 134);
// 改后
uiTextBox1.Font = new Font("宋体", 20F, FontStyle.Regular, GraphicsUnit.Pixel, 134);
```

### 5.4 容器自身字体（可选，建议一起加）

三个窗体/页面自己没有写 `Font`，用的是 SunnyUI 默认 `宋体 12pt Point`，
它会继续随 DPI 变。为了让继承链彻底统一，在各自的 `// Form1` / `// mainView` / `// simpleView`
属性块里补一行：

```csharp
Font = new Font("宋体", 24F, FontStyle.Regular, GraphicsUnit.Pixel, 134);
```

### 5.5 控件高度微调（视效果决定）

96 DPI 的设计器里 24px 的字放进 28~29px 高的按钮/文本框会偏挤，按需要加高：

| 控件 | 现在高 | 建议 |
|---|---|---|
| `uiButton1/2/3`（mainView） | 28 | 36~40 |
| `uiTextBox1` / `uiComboBox1`（mainView） | 29 | 36~40 |
| `uiTextBox1~6`（simpleView） | 23 | 30~32 |
| `uiLabel1~3`（mainView） | 24~28 | 32 |

> 改 `Size` 会同时改变运行时布局，改完务必运行一次看整体效果；
> 若只想锁字号、不想动布局，可以跳过这一节，仅在设计器里接受文字偏挤。

### 5.6 csproj 保持

路线 ② 不需要动 csproj（但建议按第 6 节做一次"清理重复项"，把两行 `ForceDesigner...` 合并成一行并删掉无效属性）。

---

## 6. 路线 ③：打开 SunnyUI 的 DPI 字体补偿

`Program.cs` 改成：

```csharp
using Sunny.UI;                       // 新增

namespace WinFormsApp3
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            UIStyles.DPIScale = true; // 新增：窗体 Shown 时把字号除以系统 DPI 比例
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}
```

实测（150% 缩放）加这一行之后：

```
12F → 8.00F      10F → 6.67F      9F → 6.00F
TipsFont 9F → 仍是 9.00F（SunnyUI 不补偿它）
字体 GetHeight ≈ 18.25px，回到 100% DPI 时的观感
```

**注意**：这是让字变**小**，不是保留你现在看到的大字；`TipsFont`、以及 `UITextBox` 内部自动创建的
`UISymbolButton / UIScrollBar` 不参与补偿，会出现大小不一致，需要的话手动单独设。

---

## 7. 公共清理：csproj 去重（三条路线都建议做）

不管选哪条，把 `WinFormsApp3.csproj` 的 PropertyGroup 整理成不重复、无无效项的形态：

```xml
<PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <UseWindowsForms>true</UseWindowsForms>
    <ImplicitUsings>enable</ImplicitUsings>

    <!-- 运行时 DPI 模式，只写一次 -->
    <ApplicationHighDpiMode>PerMonitorV2</ApplicationHighDpiMode>

    <!-- 路线 ① 删掉下面这行；路线 ②/③ 保留（只影响 VS 设计器，不影响程序运行） -->
    <ForceDesignerDPIUnaware>true</ForceDesignerDPIUnaware>

    <!-- ApplicationAutoScaleMode 不是合法属性，整行删除 -->
</PropertyGroup>
```

---

## 8. 验证：自己动手测一次

### 8.1 临时诊断代码（用完删掉）

把 `Form1.cs` 里空的 `Form1_Shown` 临时改成：

```csharp
private void Form1_Shown(object sender, EventArgs e)
{
    var lines = new List<string>
    {
        $"DeviceDpi = {DeviceDpi}",
        $"AutoScaleMode = {AutoScaleMode}",
        $"UIStyles.DPIScale = {Sunny.UI.UIStyles.DPIScale}",
        $"UIStyles.ZoomScale = {Sunny.UI.UIStyles.ZoomScale}"
    };

    void Walk(Control c, int depth)
    {
        lines.Add($"{new string(' ', depth * 2)}{c.GetType().Name} {c.Name} : "
                  + $"{c.Font.Name} {c.Font.Size:F2}{c.Font.Unit}");
        foreach (Control child in c.Controls) Walk(child, depth + 1);
    }
    Walk(this, 0);

    File.WriteAllLines(Path.Combine(AppContext.BaseDirectory, "fontcheck.txt"), lines);
}
```

`dotnet run` 之后到 `bin\Debug\net8.0-windows\fontcheck.txt` 看结果。
（SunnyUI 的 `OnShown` 先于事件处理器执行，所以这里读到的就是最终字体。）

### 8.2 验收清单

- [ ] `dotnet build` → 0 错误（NU1701 是第三方包提示，与本改动无关）
- [ ] `fontcheck.txt` 里 `DeviceDpi = 144`（本机 150% 缩放）
- [ ] 路线 ②：`Font` 全部是 `24.00Pixel / 20.00Pixel / 18.00Pixel`
- [ ] 路线 ③：`Font` 全部是 `8.00Point / 6.67Point / 6.00Point`
- [ ] 把 Windows 显示缩放临时切到 100% / 125% / 200% 各跑一次，路线 ② 的字号数值与像素高度都不变
- [ ] 设计器截图与运行截图对比：字与按钮的比例不再"跳变"
- [ ] 验证完记得删掉 `Form1_Shown` 里的诊断代码

---

## 9. 常见坑

1. **「我把 12F 改成 18F 不就行了吗？」**
   不行。`18pt @ 144DPI = 36px`，运行时会比现在再大 1.5 倍。
   要么用 `GraphicsUnit.Pixel`（路线 ②），要么配合 `UIStyles.DPIScale = true` 让 SunnyUI 在运行时除回去
   （18/1.5 = 12pt @144 = 24px，但 `TipsFont` 和内部子控件不跟着除，容易不一致）。

2. **`TipsFont` 是什么？**
   SunnyUI 按钮/文本框的提示气泡字体，和 `Font` 是两个独立属性。
   路线 ② 里要单独替换（清单里已列出 7 处）；路线 ③ 里它不会被自动补偿。

3. **为什么 `Form1.Font` 是 `gdi1`，控件是 `gdi134`？**
   窗体没显式设字体，用了 WinForms 默认字体（GDI 字符集 1）；
   控件都是 Designer 显式 `new Font(..., 134)`（GB2312）。不影响字号换算，保留即可。

4. **`ZoomScaleRect` 要不要删？**
   当前 `UIStyles.ZoomScale = false`，它完全不生效，留着无害。
   只有把 `UIStyles.ZoomScale` 和 `UIStyles.DPIScale` 都设成 `true` 时，
   窗体尺寸变化才会按 `Width / ZoomScaleRect.Width` 等比缩放控件和字体 —— 那时字体才真的会"随窗口变"。

5. **为什么 `dataGridView1` 没写 Font 却也跟着变？**
   WinForms 字体是环境属性（Ambient Property）：控件没显式赋值就继承父容器。
   `dataGridView1` 的父容器是 `uiTitlePanel1`，改它即可。

---

## 10. 动手练习

1. 按第 8 节加诊断代码，先记录**没改之前**的 `fontcheck.txt`（留作对照）。
2. 只做第 7 节 csproj 清理，重新生成，确认运行效果完全不变 —— 体会"哪些属性真的起作用"。
3. 在 `Program.cs` 临时加 `UIStyles.DPIScale = true`（路线 ③），看 `fontcheck.txt` 从 12F 变 8F，然后删掉。
4. 选路线 ②，按第 5.2 节表格做 31 处替换，验证 `24.00Pixel`，再把缩放切到 100% 跑一次，确认不再变化。
5. 思考题：如果换到 200% 缩放的显示器（192 DPI），路线 ① 和路线 ② 各自会看到什么？
   （提示：① 的 12pt 会变成 32px；② 的 24px 还是 24px。）

---

## 附录 A：探针实测原始结论

| 检查项 | 值 |
|---|---|
| 系统 DPI / 缩放 | 144 DPI / 150% |
| 生成代码里的 DPI 模式 | `HighDpiMode.PerMonitorV2` |
| `UIStyles.DPIScale` / `ZoomScale` | `False` / `False` |
| `UIDPIScale.SystemDPIScale` | `1.5` |
| 构造后 → Shown 后 → 改尺寸 → 最大化 | `Font.Size` 全程 `12.00 / 10.00 / 9.00`，无变化 |
| `12pt` `Font.GetHeight(96)` / `(144)` | `18.25` / `27.38` |
| `24px` `Font.GetHeight(96)` / `(144)` | `27.38` / `27.38`（不随 DPI 变） |
| 打开 `UIStyles.DPIScale = true` 后 | `12F→8F`、`10F→6.67F`、`9F→6F`；`TipsFont` 保持 `9F` |

---

## 11. 实际修复记录（2026-11-12 14:12 已写入工程）

### 11.1 具体原因（一句话定位到行）

| 位置 | 问题 |
|---|---|
| `WinFormsApp3.csproj`（原第 9、13 行）`ForceDesignerDPIUnaware=true` | 把 VS 设计器固定在 96 DPI，控件尺寸按 96 DPI 排版 |
| `WinFormsApp3.csproj`（原第 11 行）`ApplicationHighDpiMode=PerMonitorV2` | 程序运行时按系统 144 DPI（150% 缩放）渲染 |
| 三个 `.Designer.cs` 全部 `GraphicsUnit.Point` + `AutoScaleMode = None` | Point 字体随 DPI 放大 ×1.5，但控件尺寸**不随 DPI 缩放**（AutoScaleMode.None 禁止了布局重算） |

三者叠加 = **字放大 1.5 倍、控件不放大**：「产品名称:」被截成「产品」，
「当前连接:」被截成「当前连」，DataGridView 表头竖排换行。这才是你看到的"字体大小变化"。
原第 8 行的 `SystemAware` 被第 11 行覆盖，第 15 行 `ApplicationAutoScaleMode` 不是合法属性（无效）。

### 11.2 已做的两处改动（`.Designer.cs` 全部保持原样）

**① `Program.cs`**（文件本来就有 `using Sunny.UI;`，只加 3 行注释 + 1 行代码）：

```csharp
[STAThread]
static void Main()
{
    // 修复字体随 DPI 放大：SunnyUI 在窗体 Shown 时按系统 DPI 比例把字号除回去。
    // 150% 缩放(144 DPI) 下 12F -> 8F，渲染仍是 16 px，与 96 DPI 设计器一致。
    UIStyles.DPIScale = true;

    ApplicationConfiguration.Initialize();
    Application.Run(new Form1());
}
```

**② `WinFormsApp3.csproj`**（去重后只保留两个有效属性）：

```xml
<ApplicationHighDpiMode>PerMonitorV2</ApplicationHighDpiMode>
<ForceDesignerDPIUnaware>true</ForceDesignerDPIUnaware>
```

为什么把设计器锁回 96 DPI：设计器 `12F @96 = 16px`，运行时被补偿成
`8F @144 = 16px`，两边物理大小完全相同，字体不再"变化"。
（你之前删掉这行是想让设计器也按 150% 渲染，但那样设计器里的字也会撑破控件。）

### 11.3 修复前后对照（真实截图）

| | 修复前 | 修复后 |
|---|---|---|
| 运行时 `Font.Size` | `12F / 10F / 9F` | `8F / 6.67F / 6F` |
| 渲染物理大小 | 24 / 20 / 18 px | 16 / 13.3 / 12 px |
| 「产品名称:」 | 被截成「产品」 | 完整显示 |
| 「当前连接:」 | 被截成「当前连」 | 完整显示 |
| DataGridView 表头 | 每个字竖排换行 | 单行/双行正常排版 |
| `dotnet build` | 0 错误 | 0 错误（6 个 NU1701 是第三方包的历史提示） |

### 11.4 你自己复现/排查的步骤（学会这一套就能解决同类问题）

1. **先证明字号有没有被代码改**：在 `Form1_Shown` 里递归打印 `c.Name / c.Font.Size / c.Font.Unit`
   到 `bin\Debug\net8.0-windows\fontcheck.txt`（代码见第 8.1 节）。
   本例打印结果全进程不变 → 排除"代码改字号"这条思路。
2. **打印 DPI**：`DeviceDpi` 和 `Graphics.FromHwnd(IntPtr.Zero).DpiX`。本例 144。
3. **查 csproj / 生成的 `ApplicationConfiguration.g.cs`**：确认 `ApplicationHighDpiMode`
   和 `ForceDesignerDPIUnaware`，用
   `dotnet build -t:Rebuild -p:EmitCompilerGeneratedFiles=true -p:CompilerGeneratedFilesOutputPath=<临时目录>`
   把生成代码导出来看，就能发现 `ApplicationAutoScaleMode` 根本没生效。
4. **套公式**：`像素 = 字号 × DPI ÷ 72`。144 DPI 下 12pt=24px，而控件还是按 96 DPI 的
   16px 排的 → 定位到"字放大、控件不放大"。
5. **选修复方向**：要么把字缩回去（SunnyUI `DPIScale`，一行，本次采用），
   要么把控件尺寸一起 ×1.5（改动量大）。二者选一，不要混。
6. **必须截图验收**：只打印 `Font.Size` 会被 `GraphicsUnit.Pixel` 骗到（见 0.1 节），
   一定要看真实画面有没有裁切。

### 11.5 备份与回滚

改动前的四个文件备份在 `%TEMP%\wb_fontprobe\backup\`：
`Form1.Designer.cs.bak`、`mainView.Designer.cs.bak`、`simpleView.Designer.cs.bak`、
`WinFormsApp3.csproj.bak`（三个 Designer 最终未改动，备份仅用于试验回滚）。

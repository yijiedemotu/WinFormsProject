# WinFormsProject

.NET 8 WinForms 手敲练习工程：用 SunnyUI 做界面，练「主窗体嵌入 UIPage」的多页面布局，并逐步接入 Modbus RTU 采集，向「八通道尺寸测量上位机」演进。

## 运行

- 环境：Windows + .NET 8 SDK
- 命令行：`dotnet restore` → `dotnet build` → `dotnet run`
- 或用 Visual Studio 打开 `WinFormsApp3.slnx`

> **串口前提**：`Form1` 构造时会调用 `mainCommon.modbusRtuClient.Connect("com2")`。本机没有 COM2 时连接会失败，只是指示灯不亮，界面仍可正常打开；要换串口改 `Form1.cs` 即可。
> 当前编译结果：0 错误；6 个 `NU1701` 是第三方包声明的目标框架与 net8 不一致的还原提示，不影响运行。

> **命名说明**：仓库/目录叫 `WinFormsProject`，但项目文件、解决方案和代码命名空间仍是 `WinFormsApp3`（沿用练习时的命名，未改名，改动会牵连 `.slnx` 与全部 `using`）。`Helper/ModbusRtuClient.cs`、`Helper/Log.cs` 则沿用 `gkHelper` 命名空间。

## 结构

| 文件 | 作用 |
|---|---|
| `Program.cs` | 入口，`ApplicationConfiguration.Initialize()` 后启动 `Form1` |
| `Form1.cs` / `.Designer.cs` | 主窗体 `UIForm`；4 个导航按钮把页面嵌入 `uiPanel2`；启动时连接 COM2，成功则点亮 `uiLight1` |
| `Helper/FormShowHelper.cs` | `formShow()` 把 `UIPage` 嵌入 `UIPanel`（`Parent` + `Dock=Fill` + `Show()`）；另提供反射开启双缓冲 |
| `Common/mainCommon.cs` | 全局静态 `ModbusRtuClient` 实例，供各页面共享通信对象 |
| `Helper/ModbusRtuClient.cs` | Modbus RTU 通信封装：连接/断开、线圈与各类寄存器读写、四种字节序转换、`GetAvailablePorts` / `TestConnection` |
| `Helper/Log.cs` | 按 `Logs/日期/级别/` 分目录的轻量文本日志，写入失败自行兜底，不影响主流程 |
| `View/mainView.cs` | **数据测量页**：`Load` 时创建 8 个 `simpleView`；`timer1` 在已连接时读保持寄存器 `0..7` 并逐个刷新 |
| `View/simpleView.cs` | 单通道卡片，`refreshData(ushort)` 把数值写进文本框 |
| `View/totalView.cs` | **查询统计页**：日期区间 / 条件下拉 / `UIDataGridView`（界面骨架，业务逻辑待补） |
| `View/plotView.cs` | **趋势曲线页**：日期与条件选择（界面骨架，待接 ScottPlot） |
| `View/yvzhiView.cs` | **阈值设置页**：多面板 + 表格式布局（界面骨架，业务逻辑待补） |
| `Properties/Resources.*` | 资源文件 |
| `教程/WinFormsApp3_字体固定教程.md` | 150% / 144 DPI 下字体被放大的实测与原理分析 |

## 依赖

`SunnyUI` 3.9.9 · `ScottPlot.WinForms` 5.1.59 · `NModbus` 3.0.83 · `NModbus.Serial` 3.0.83 · `DH.NModbus` 3.91.2024.1031 · `Microsoft.IdentityModel.Logging` 8.23.0

## 需要注意

- 目前只有 `mainView` 接通了采集与刷新；`totalView` / `plotView` / `yvzhiView` 仍是界面骨架。
- `教程/` 里那份文档记录的 DPI 修复**已全部回退**，工程保持最初状态；文档仅作为排查过程与原理留存，第 11 节「已写入工程」不再成立，不要照抄改代码。
- `bin/`（含 NuGet 运行时副本）、`obj/`、`.vs/`、`*.user`、`Logs/` 由 `.gitignore` 排除，不入库。

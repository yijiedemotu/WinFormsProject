# WinFormsProject

.NET 8 WinForms 手敲练习工程：用 SunnyUI 做界面，练「主窗体嵌入 UIPage」的多页面布局，并逐步接入 Modbus RTU 采集、异步刷新与 SQLite 存储，向「八通道尺寸测量上位机」演进。

## 运行

- 环境：Windows + .NET 8 SDK
- 命令行：`dotnet restore` → `dotnet build` → `dotnet run`
- 或用 Visual Studio 打开 `WinFormsApp3.slnx`

> **串口前提**：串口不再写死。启动后在「数据测量」页的下拉框选端口号，点连接按钮；连接成功会点亮主窗体的指示灯（`Form1` 在 `Load` 时把 `uiLight1` 传给 `mainView`，属于跨页面访问控件）。没有串口时界面仍可正常打开，只是不采集。
> 当前编译结果：0 错误；历史 `NU1701`（第三方包目标框架与 net8 不一致）若干，另有 2 处 `CS8618`（`Form1.cs` 的 `mainView`、`mainView.cs` 的 `readTask` 都是装载时才赋值的字段），均不影响运行。

> **命名说明**：仓库/目录叫 `WinFormsProject`，但项目文件、解决方案和代码命名空间仍是 `WinFormsApp3`（沿用练习时的命名，未改名，改动会牵连 `.slnx` 与全部 `using`）。`Helper/ModbusRtuClient.cs`、`Helper/Log.cs` 则沿用 `gkHelper` 命名空间。

## 结构

| 文件 | 作用 |
|---|---|
| `Program.cs` | 入口，`ApplicationConfiguration.Initialize()` 后启动 `Form1` |
| `Form1.cs` / `.Designer.cs` | 主窗体 `UIForm`；4 个导航按钮把页面嵌入 `uiPanel2`；`Load` 时把 `uiLight1` 注入 `mainView` |
| `Helper/FormShowHelper.cs` | `formShow()` 把 `UIPage` 嵌入 `UIPanel`（`Parent` + `Dock=Fill` + `Show()`），带 null 参数校验；另提供反射开启双缓冲 |
| `Common/mainCommon.cs` | 全局静态 `ModbusRtuClient` 实例，供各页面共享通信对象 |
| `Helper/ModbusRtuClient.cs` | Modbus RTU 通信封装：连接/断开、线圈与各类寄存器读写、四种字节序转换、`GetAvailablePorts` / `TestConnection` |
| `Helper/Log.cs` | 按 `Logs/日期/级别/` 分目录的轻量文本日志，写入失败自行兜底，不影响主流程 |
| `Helper/sqlHelper.cs` | SQLite（`Microsoft.Data.Sqlite`）封装：自动建表、旧时间格式一次性迁移、增删改查与参数化条件查询，统一时间格式 `yyyy-MM-dd HH:mm:ss` |
| `Model/dataModel.cs` | 8 通道记录模型：`testTime` / `projectName` / `data1..data8` |
| `View/mainView.cs` | **数据测量页**：8 个 `simpleView`；用 `Task.Run` + `CancellationTokenSource` 每秒读保持寄存器 `0..7`（替代会卡顿的 `timer1`）；`BindingList<dataModel>` 绑定 `DataGridView`；连接与保存按钮 |
| `View/simpleView.cs` | 单通道卡片，`refreshData(ushort)` 里用 `this.Invoke` 把赋值切回 UI 线程 |
| `View/totalView.cs` | **查询统计页**：日期区间 / 条件下拉 / `UIDataGridView`（界面骨架，业务逻辑待补） |
| `View/plotView.cs` | **趋势曲线页**：日期与条件选择（界面骨架，待接 ScottPlot） |
| `View/yvzhiView.cs` | **阈值设置页**：多面板 + 表格式布局（界面骨架，业务逻辑待补） |
| `Properties/Resources.*` | 资源文件 |
| `教程/WinFormsApp3_字体固定教程.md` | 150% / 144 DPI 下字体被放大的实测与原理分析 |

## 依赖

`SunnyUI` 3.9.9 · `ScottPlot.WinForms` 5.1.59 · `NModbus` 3.0.83 · `NModbus.Serial` 3.0.83 · `DH.NModbus` 3.91.2024.1031 · `Microsoft.IdentityModel.Logging` 8.23.0 · `Microsoft.Data.Sqlite` 8.0.33

## 需要注意

- 采集链路已通（异步读取 → 回 UI 线程 → 8 通道显示）；`SqlHelper` 已备好，但保存按钮目前只把记录加进内存 `BindingList`，**尚未写库**。
- `totalView` / `plotView` / `yvzhiView` 仍是界面骨架。
- `教程/` 里那份文档记录的 DPI 修复**已全部回退**，工程保持最初状态；文档仅作为排查过程与原理留存，第 11 节「已写入工程」不再成立，不要照抄改代码。
- `bin/`（含 NuGet 运行时副本）、`obj/`、`.vs/`、`*.user`、`Logs/`、`*.db` 由 `.gitignore` 排除，不入库。

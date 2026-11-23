# WinFormsProject

.NET 8 WinForms 手敲练习工程：用 SunnyUI 做界面，练「主窗体嵌入 UIPage」的多页面布局，并逐步接入 Modbus RTU 采集、异步刷新与 SQLite 存储，向「八通道尺寸测量上位机」演进。

## 运行

- 环境：Windows + .NET 8 SDK
- 命令行：`dotnet restore` → `dotnet build` → `dotnet run`
- 或用 Visual Studio 打开 `WinFormsApp3.slnx`

> **串口前提**：串口不再写死。启动后在「数据测量」页的下拉框选端口号，点「连接」；波特率、校验位、从站地址、轮询与重连间隔都在 `appconfig.json` 里改。连接成功会点亮主窗体指示灯，掉线后按配置自动重连；点「断开」可停止重连。没有串口时界面仍能打开，只是不采集，可用 Modbus Slave 模拟从站验证。
> **闭环链路**：原始寄存器值 → 两点标定 → 物理量 → 与阈值比较 → 合格/不合格判定 → 卡片灯色 + 日志报警 → 随采样写入 SQLite → 查询页合格率统计与 CSV 导出。
> 当前编译结果：0 错误、6 条 `NU1701` 警告（OpenTK / SkiaSharp 等第三方包目标框架与 net8 不一致），无 C# 编译警告，均不影响运行。

> **命名说明**：仓库/目录叫 `WinFormsProject`，项目文件已改名为 `TICP.csproj`（解决方案文件仍叫 `WinFormsApp3.slnx`）。`Helper` / `Model` / `Common` / `Properties` / `Program` 已迁到 `TICP.*` 命名空间；`View` 下的页面与 `Form1` 仍沿用 `WinFormsApp3.*`，因为它们的 Designer 与 `.resx` 资源名绑定在这个命名空间上。改命名时务必同时改 `using`，否则会报 `CS0234`。

## 结构

| 文件 | 作用 |
|---|---|
| `Program.cs` | 入口，`ApplicationConfiguration.Initialize()` 后启动 `Form1` |
| `Form1.cs` / `.Designer.cs` | 主窗体 `UIForm`；4 个导航按钮把页面嵌入 `uiPanel2`；`Load` 时把 `uiLight1` 注入 `mainView` |
| `Helper/FormShowHelper.cs` | `formShow()` 把 `UIPage` 嵌入 `UIPanel`（`Parent` + `Dock=Fill` + `Show()`），带 null 参数校验；另提供反射开启双缓冲 |
| `Common/mainCommon.cs` | 全局静态 `ModbusRtuClient` 实例，供各页面共享通信对象；`oriData` 存原始计数、`data` 存标定后物理量 |
| `Common/appConfig.cs` | 串口/采集参数（`appconfig.json`）：波特率、校验位、从站地址、轮询/重连间隔，带取值夹取与持久化 |
| `Common/qualityCommon.cs` | 判定服务：两点线性标定 + 上下限比较 + 整条记录汇总，纯逻辑不依赖 WinForms |
| `Helper/ModbusRtuClient.cs` | Modbus RTU 通信封装：连接/断开、线圈与各类寄存器读写、四种字节序转换、`GetAvailablePorts` / `TestConnection` |
| `Helper/Log.cs` | 按 `Logs/日期/级别/` 分目录的轻量文本日志，写入失败自行兜底，不影响主流程 |
| `Helper/sqlHelper.cs` | SQLite（`Microsoft.Data.Sqlite`）封装：自动建表、旧时间格式一次性迁移、增删改查与参数化条件查询，统一时间格式 `yyyy-MM-dd HH:mm:ss` |
| `Model/dataModel.cs` | 8 通道记录模型：`testTime` / `projectName` / `data1..data8` / `result` / `ngChannels` |
| `View/mainView.cs` | **数据测量页**：8 个 `simpleView`；`Task.Run` + `CancellationTokenSource` 按配置周期读寄存器；判定接入采集；断线自动重连；连接/断开、保存按钮 |
| `View/simpleView.cs` | 单通道卡片，`refreshQuality()` 通过 `PostToUi()`（BeginInvoke + 句柄校验）显示物理量、合格/不合格与灯色 |
| `View/totalView.cs` | **查询统计页**：日期/产品/通道条件查询、合格率统计标签、判定列、CSV 导出 |
| `View/plotView.cs` | **趋势曲线页**：日期与条件选择（界面骨架，待接 ScottPlot） |
| `View/yvzhiView.cs` | **阈值设置页**：多面板 + 表格式布局（界面骨架，业务逻辑待补） |
| `Properties/Resources.*` | 资源文件 |
| `教程/WinFormsApp3_字体固定教程.md` | 150% / 144 DPI 下字体被放大的实测与原理分析 |
| `教程/WinFormsApp3_曲线与下拉框修复教程.md` | 查询/趋势页空白、下拉框无数据、DPI 异常的排查记录 |
| `教程/通道编号_隐藏按钮_曲线中文_修复教程.md` | 通道编号、隐藏通道按钮不响应、曲线中文变方框的定位方法与练习题 |
| `教程/业务闭环_阈值判定_自动重连_统计导出教程.md` | 标定判定、断线重连、判定入库与旧库迁移、合格率统计与 CSV 导出的完整实现 |

## 依赖

`SunnyUI` 3.9.9 · `ScottPlot.WinForms` 5.1.59 · `NModbus` 3.0.83 · `NModbus.Serial` 3.0.83 · `DH.NModbus` 3.91.2024.1031 · `Microsoft.IdentityModel.Logging` 8.23.0 · `Microsoft.Data.Sqlite` 8.0.33

## 需要注意

- 采集链路已闭环：异步读取 → 标定/判定 → 回 UI 线程 → 8 通道显示 → 判定随采样写 SQLite（`data.db` 的 `DataModel` 表）→ 查询页合格率统计与 CSV 导出。写库失败会提示且表格里不留假记录。
- 采集后台任务只在页面 `Disposed` 时取消；切页只是换 `Parent`，期间继续更新全局数据但不碰控件。单次读失败只跳过本轮，并按 `appconfig.json` 的间隔自动重连（用户点「断开」后停止）。
- 阈值页必须先标定（量具上下限 + 校准上下限），未标定的通道显示「未标定」灰色，不会误报不合格。
- 尚不属于本技术栈闭环的功能：Modbus TCP / OPC UA / PLC 协议、WPF+MVVM 重构、权限/配方/MES、单元测试项目。
- `教程/` 里那份文档记录的 DPI 修复**已全部回退**，工程保持最初状态；文档仅作为排查过程与原理留存，第 11 节「已写入工程」不再成立，不要照抄改代码。
- `bin/`（含 NuGet 运行时副本）、`obj/`、`.vs/`、`*.user`、`Logs/`、`*.db` 由 `.gitignore` 排除，不入库。

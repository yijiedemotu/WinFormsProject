# WinFormsProject

.NET 8 WinForms 手敲练习工程，用 SunnyUI 做界面、练习「主窗体嵌入 UIPage」的多页面布局方式；另附一份高 DPI 字体缩放问题的排查记录。

## 运行

- 环境：Windows + .NET 8 SDK
- 命令行：`dotnet restore` → `dotnet build` → `dotnet run`
- 或用 Visual Studio 打开 `WinFormsApp3.slnx`

> 命名说明：仓库/目录叫 `WinFormsProject`，但项目文件、解决方案和代码命名空间仍是 `WinFormsApp3`（沿用练习时的命名，未改名，改动会牵连 `.slnx` 与全部 `using`）。

## 结构

| 文件 | 作用 |
|---|---|
| `Program.cs` | 入口，`ApplicationConfiguration.Initialize()` 后启动 `Form1` |
| `Form1.cs` / `.Designer.cs` | 主窗体 `UIForm`，右侧 `uiPanel2` 作为页面容器 |
| `Helper/FormShowHelper.cs` | `formShow()` 把 `UIPage` 嵌入 `UIPanel`（`Parent` + `Dock=Fill` + `Show()`）；另提供反射开启双缓冲 |
| `View/mainView.cs` | 主页面，`Load` 时循环创建 8 个 `simpleView` 放进 `uiTableLayoutPanel1` |
| `View/simpleView.cs` | 单个通道卡片（目前是占位页面） |
| `Properties/Resources.*` | 资源文件 |
| `教程/WinFormsApp3_字体固定教程.md` | 150% / 144 DPI 下字体被放大的实测与原理分析 |

## 依赖

- `SunnyUI` 3.9.9
- `ScottPlot.WinForms` 5.1.59

## 需要注意

- `教程/` 里那份文档记录的 DPI 修复**已全部回退**，工程保持最初状态；文档仅作为排查过程与原理留存，第 11 节「已写入工程」不再成立，不要照抄改代码。
- `bin/`（含 NuGet 运行时副本）、`obj/`、`.vs/`、`*.user` 由 `.gitignore` 排除，不入库。

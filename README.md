# 无缝拼图 · 紧凑联动版

<img src="assets/seamless-puzzle.png" width="96" height="96" alt="无缝拼图图标">

中文 Windows 拼图工具。基于 [Dont-know-code/SeamlessImage_Stitcher](https://github.com/Dont-know-code/SeamlessImage_Stitcher) 改进，增加紧凑窗口、置顶、剪贴板读取和轻图联动。

**[下载 Windows 版本](https://github.com/UNO1742/seamless-puzzle-compact/releases/latest)** · [配套轻图](https://github.com/UNO1742/clipslim)

当前版本 **1.3.1**：修复“无法切回 Edge，未发送”，Windows 下载包附带轻图 **1.7.2**。详见 [更新记录](CHANGELOG.md)。

## 功能

- 横向、竖向、四宫格和九宫格拼图。
- 紧凑中文界面，置顶开关，读取剪贴板按钮与 Ctrl+V。
- 图片缩略图、名称和尺寸；拖动排序，选择参与拼接的图片。
- 自动生成拼图预览，保存使用完整尺寸。
- “压缩并发送到表格”：交给轻图压缩，再插入桌面 WPS 或自动切回 Edge 金山表格粘贴。
- 修复截图透明度、跨线程图像解码、单图缩略图和拼图预览问题。

## 使用

下载发布 ZIP，完整解压，运行 **启动无缝拼图.exe**；启动器会创建或更新桌面“无缝拼图”快捷方式。普通拼图功能不需要轻图。

表格联动需要根目录中的 `ClipSlim-v1.7.2.exe`。带轻图的发布包已附带；自行构建时可从[轻图发布页](https://github.com/UNO1742/clipslim/releases/latest)下载后放入，或构建时使用 `-ClipSlimExe` 参数。

在轻图中选择目标模式和压缩参数，再使用拼图按钮：

- **桌面 WPS**：先选中单元格，压缩后居中插入。
- **金山在线表格**：首次在 Edge 中设置 **插入 → 图片 → 从外部粘贴图片时 → 默认为单元格图片**；在网页点选目标格并停留约一秒，再点击拼图按钮。轻图会核对目标、切回网页并自动粘贴，保持 Edge 的最大化或全屏状态。

在线“已发送”表示粘贴请求发出，网页上传结果需在表格中确认；最终存储体积由金山文档决定。无法识别目标或焦点时会停止自动操作并提示。

### 恢复已保存的未发送拼图

更新不会自动迁移旧进程中尚未保存的图片队列，请先保存再关闭旧窗口。若已有完整拼图 PNG，可显式恢复为一张图片：

```powershell
$image = 'C:\path\to\unsent-collage.png'
$encoded = [Convert]::ToBase64String([Text.Encoding]::UTF8.GetBytes($image))
& '.\启动无缝拼图.exe' --recover $encoded
```

该入口仅加载指定文件；普通启动不自动读取历史图片。若修复版已打开，请先保存并关闭该窗口再使用恢复入口。

## 从源码构建

需要 Windows 10/11、.NET 8 SDK，以及编译启动器所需的 .NET Framework 4.8。

```powershell
# 自包含发布：接收端无需另外安装 .NET 8
./build.ps1

# 同时放入配套轻图
./build.ps1 -ClipSlimExe C:\path\to\ClipSlim-v1.7.2.exe

# 较小的依赖运行时版本：接收端需要 .NET 8 Desktop Runtime x64
./build.ps1 -FrameworkDependent
```

输出到 `dist/seamless-puzzle-compact-win-x64/`。请分发整个目录，而不只是一个 EXE。

## 项目结构

- `src/SeamlessPuzzle/`：WPF 应用、ImageSharp 拼接服务、剪贴板与联动客户端。
- `src/PortableLauncher/`：便携启动器与桌面快捷方式更新。
- `third-party/`：上游依赖及附带轻图的许可信息。

## 来源与许可

上游 [SeamlessImage_Stitcher](https://github.com/Dont-know-code/SeamlessImage_Stitcher)，Copyright © 2025 Dont-know-code，采用 MIT 许可。此仓库保留原许可证，是独立维护的衍生版本。

本仓库修改部分同样采用 MIT。第三方组件各自遵循其许可证，见 [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md)；不能将整个依赖分发包简单视为只有 MIT。

图标为本项目生成的资源。仓库不包含用户图片、账号、具体在线工作表链接或本机绝对路径。

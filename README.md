# 未知伤亡解除新手教程限制mod

解除《未知伤亡》（Casualties Unknown）新手教程必须到达新层才能解锁的限制，可直接进入急救、制作等教程。教程仍需按正常流程完成。

**版本：v1.0.0 ｜ 适用游戏：Demo v7.0.1（Windows）｜ 前置：BepInEx 5**

## 下载和安装

**[下载 Mod 安装包：TutorialLayerUnlock-v1.0.0.zip](https://github.com/ychdhtnl/Casualties-Unknown-Tutorial-Unlock/raw/refs/heads/main/TutorialLayerUnlock-v1.0.0.zip)**

1. 退出游戏。
2. 确保已安装 [BepInEx 5.4.23.5 Windows x64](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.5)。已安装包含 BepInEx 5 的联机 Mod 环境时，无需重复安装框架。
3. 将安装包中的 `BepInEx` 文件夹复制到包含 `CasualtiesUnknown.exe` 的游戏根目录，合并同名文件夹。
4. 确认文件位置：

```text
游戏根目录/
└─ BepInEx/
   └─ plugins/
      └─ TutorialLayerUnlock/
         └─ TutorialLayerUnlock.dll
```

5. 启动游戏，进入教程选择界面。

安装包仅包含本 Mod，不包含游戏本体、联机 Mod 或 BepInEx 框架。请下载上方的安装包；仓库的“Download ZIP”下载的是项目源码。

## 修改范围

- 仅跳过教程选择菜单和开始课程时的层数条件。
- 不写入存档中的最深到达层数，不自动完成教程。
- 不修改其他玩法的解锁条件，也不调用成就解锁接口。
- 不覆盖原版游戏程序集。

## 卸载

退出游戏，删除 `BepInEx/plugins/TutorialLayerUnlock` 文件夹，再重新启动游戏。教程会重新根据真实探索层数判断是否解锁。

## 验证与兼容性

已检查 Demo v7.0.1 的教程方法，确认开始课程有一处、菜单有两处对应检查。通过 9 项检查，包括实际 Harmony 补丁在模拟教程方法上的解锁效果、其他层数读取不变、移除补丁后恢复原限制，以及原游戏程序集未被修改。

**尚未进行游戏内操作实测，也未实测与全部联机 Mod 的组合。** 若联机 Mod 本身限制进入教程，本补丁不会移除该限制。不宣称兼容正式版或其他游戏版本。

如果游戏或其他 Mod 改变了对应检查的结构，补丁在检查数量不匹配时会停止启用，并向 `BepInEx/LogOutput.log` 写入错误。

成功启用时的日志：

```text
Tutorial layer requirements removed. Save progression is unchanged.
```

## 源码和构建

- `Plugin.cs`：BepInEx 插件入口。
- `TutorialUnlockPatch.cs`：只替换两个教程方法内对 `deepestlayer` 的读取。
- `build.ps1`：使用本机游戏及 BepInEx 提供的程序集构建。
- `Verify.cs`、`verification.txt`：验证代码与结果。

在 Windows PowerShell 中运行：

```powershell
.\build.ps1 -GameDirectory "你的游戏根目录"
```

输出为 `TutorialLayerUnlock.dll`。构建需游戏已安装 BepInEx 5，使用 Windows 自带的 .NET Framework C# 编译器；仓库不附带游戏代码或第三方依赖程序集。

## 作者

**循此苦旅 直抵群星** · [B 站主页](https://space.bilibili.com/1620269886)

本 Mod 为非官方插件，与游戏开发者无隶属关系。游戏、BepInEx 和 Harmony 的权利归各自作者所有。

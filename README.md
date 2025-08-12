## Source 2 Vmat Files Generator

这是一个用于生成Source 2游戏材质文件（.vmat）的WPF程序。它可以匹配材质文件的后缀以自动生成适用于Source 2引擎的.vmat文件。

### 支持的贴图类型：

|   功能   | 支持 |
|:------:|:--:|
| 漫反射贴图  | ✔️ |
|  法线贴图  | ✔️ | 
| 粗糙度贴图  | ✔️ |
|  AO贴图  | ✔️ |
| 自发光贴图  | ✔️ |
| 金属度贴图  | ✔️ |
| Mask贴图 | ❌  |
| 透明度贴图  | ✔️  |

### 注意事项
该程序是在sbox引擎基础上进行开发和测试的，CS2没测试过，但理论上应该也可以使用。

### 使用方法
1. 将材质放到materials文件夹下。
- 目录结构示例：
```
materials/
├── metal01/
│   ├── metal01_diff.png
│   ├── metal01_roughness.png
│   └── metal01_normal.png
├── Wood/
│   ├── Wood01/
│       ├── Wood01_diff.png
│       ├── Wood01_roughness.png
│       └── Wood01_normal.png
│   ├── Wood02/
│       ├── Wood02_diff.png
│       ├── Wood02_roughness.png
│       └── Wood02_normal.png
```
2. 打开程序，选择游戏和shader，点击创建材质按钮。
3. 等待生成完成，这时会在materials文件夹下生成vmat文件。
4. 将生成的vmat文件放到游戏的材质目录下。
5. 打开游戏编辑器，材质自动编译后就能使用了。

### 系统要求
- net9.0或更高版本。
- windows10.0.17763.0 或更高版本。

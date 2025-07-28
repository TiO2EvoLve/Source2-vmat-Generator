## Source 2 Vmat Files Generator
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
2. 选择游戏种类和shader，点击创建材质按钮。
3. 等待生成完成，这时会在materials文件夹下生成vmat文件。
4. 将生成的vmat文件放到游戏的材质目录下。
5. 打开游戏编辑器，材质自动编译后就能使用了。
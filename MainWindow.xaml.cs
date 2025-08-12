using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialsCreate.Manage;
using Newtonsoft.Json;
using Tommy;
using Wpf.Ui.Controls;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;

namespace MaterialsCreate;

public partial class MainWindow :  INotifyPropertyChanged
{
    private bool ssr;
    public bool SSR
    {
        get => ssr;
        set
        {
            if (ssr != value)
            {
                ssr = value;
                OnPropertyChanged(nameof(SSR));
            }
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        LoadComboBoxItems();
    }

    //加载下拉框数据
    private void LoadComboBoxItems()
    {
        var configPath = "Config/config.toml";
        if (File.Exists(configPath))
        {
            TextReader reader = new StreamReader(configPath);
            var table = TOML.Parse(reader);

            foreach (var key in table.Keys)
            {
                GameComboBox.Items.Add(new ComboBoxItem { Content = key });
            }
        }else
        {
            Message.ShowSnack("错误", "配置文件被删除！。", ControlAppearance.Danger,
                new SymbolIcon(SymbolRegular.ErrorCircle20), 3);
        }
    }

    private Dictionary<string, List<string>>? pbrMap
    {
        get
        {
            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(File.ReadAllText("Config/FileType.json"));
            }
            catch (Exception ex)
            {
                
                Message.ShowSnack("错误", $"读取或解析 FileType.json 失败：{ex.Message}", ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.ErrorCircle20), 3);
                LogManage.AddLog($"解析配置文件FileType.json失败,请检查格式是否正确{ex.Message}");
                LogManage.AddLog($"加载默认配置");
                //加载默认配置
                return new Dictionary<string, List<string>>
                {
                    ["BaseColor"] = ["_albedo", "_diffuse", "_basecolor", "_color", "_col", "_bc", "_diff"],
                    ["AO"] = ["_ao", "_ambientocclusion", "_occlusion", "_ambocc", "_aoc", "_occl"],
                    ["Normal"] = ["_normal", "_nor", "_norm", "_nrm", "_normalmap", "_nml", "_bump", "_n"],
                    ["Roughness"] = ["_roughness", "_rou", "_rgh", "_gloss", "_gls", "_rough", "_specular"],
                    ["Metal"] = ["_metallic", "_met", "_metal", "_mtl", "_metalness", "_metall"],
                    ["SelfIllum"] = ["_selfillum", "_illum", "_glowmask", "_emit", "_emissive", "_light"],
                    ["Opacity"] = ["_translucent", "_trans", "_opacity", "_opa", "_alpha"],
                };
            }
        }
    }
    // 创建 vmat 文件
    private void CreateVmat(object sender, RoutedEventArgs e)
    {
        // 获取 materials 文件夹下所有子文件夹
        LogManage.AddLog("开始创建 vmat 文件...");
        foreach (string folder in Directory.EnumerateDirectories("materials", "*", SearchOption.AllDirectories))
        {
            //判断是叶子文件夹
            if (Directory.GetDirectories(folder).Length == 0 && Directory.GetFiles(folder).Length > 0)
            {
                LogManage.AddLog($"正在处理文件夹：{folder}");
                string TextureColorPath = String.Empty;
                string TextureAmbientOcclusionPath = String.Empty;
                string TextureNormalPath = String.Empty;
                string TextureRoughnessPath = String.Empty;
                string TextureMetalnessPath = String.Empty;
                string TextureSelfIlluminationPath = String.Empty;
                string TextureOpacityPath = String.Empty;
                //遍历所有文件
                foreach (string file in Directory.EnumerateFiles(folder, "*.*", SearchOption.TopDirectoryOnly))
                {
                    // 获取文件名
                    var filename = Path.GetFileName(file);
                    // 获取文件类型
                    var type = MatchType(filename);
                    if (type == null) continue;
                    switch (type)
                    {
                        case "BaseColor": TextureColorPath = file; break;
                        case "AO": TextureAmbientOcclusionPath = file; break;
                        case "Normal": TextureNormalPath = file; break;
                        case "Roughness": TextureRoughnessPath = file; break;
                        case "Metal": TextureMetalnessPath = file; break;
                        case "SelfIllum": TextureSelfIlluminationPath = file; break;
                        case "Opacity": TextureOpacityPath = file; break;
                        default: continue;
                    }
                }
                //必须存在漫反射贴图
                if (TextureColorPath == "")
                {
                    Message.ShowMessageBox("警告",$"{folder}文件夹没有检测到漫反射贴图,请检查或加入配置中");   
                    LogManage.AddLog($"警告：{folder}文件夹没有检测到漫反射贴图,请检查或加入配置中");
                    continue;
                }
                //创建 vmat 文件
                var filePath = Path.Combine(folder, $"{Path.GetFileName(folder)}.vmat");
                using (var writer = new StreamWriter(filePath, false))
                {
                    //遍历所有文件
                    // 写入文件
                    writer.WriteLine("Layer0");
                    writer.WriteLine("{");
                    writer.WriteLine($"\tshader \"{ShaderComboBox.Text}.vfx\"");
                    writer.WriteLine("");
                    if (TextureMetalnessPath != "")
                    {
                        writer.WriteLine("\tF_METALNESS_TEXTURE 1\n");
                        writer.WriteLine("\tF_SPECULAR 1\n"); 
                    }

                    if (TextureSelfIlluminationPath !="")
                    {
                        writer.WriteLine("\tF_SELF_ILLUM 1\n");
                    }
                    if (TextureOpacityPath != "")
                    {
                        writer.WriteLine("\tF_TRANSLUCENT 1\n");
                    }
                    writer.WriteLine("\t//---- Ambient Occlusion ----");
                    writer.WriteLine("\tg_flAmbientOcclusionDirectDiffuse \"0.000\"");
                    writer.WriteLine("\tg_flAmbientOcclusionDirectSpecular \"0.000\"");
                    if (TextureAmbientOcclusionPath != "")
                    {
                        writer.WriteLine($"\tg_flAmbientOcclusionDirectDiffuse \"{TextureAmbientOcclusionPath}\"");
                    }
                    else
                    {
                        writer.WriteLine("\tTextureAmbientOcclusion \"materials/default/default_ao.tga\"");  
                    }
                    writer.WriteLine("");
                    writer.WriteLine("\t//---- Color ----");
                    writer.WriteLine("\tg_flModelTintAmount \"1.000\"");
                    writer.WriteLine("\tg_vColorTint \"[1.000000 1.000000 1.000000 0.000000]\"");
                    writer.WriteLine($"\tTextureColor \"{TextureColorPath}\"");
                    writer.WriteLine("");
                    writer.WriteLine("\t//---- Fade ----");
                    writer.WriteLine("\tg_flFadeExponent \"1.000\"");
                    writer.WriteLine("");
                    writer.WriteLine("\t//---- Fog ----");
                    writer.WriteLine("\tg_bFogEnabled \"1\"");
                    writer.WriteLine("");
                    writer.WriteLine("\t//---- Lighting ----");
                    writer.WriteLine("\tg_flDirectionalLightmapMinZ \"0.050\"");
                    writer.WriteLine("\tg_flDirectionalLightmapStrength \"1.000\"");
                    writer.WriteLine("");
                    writer.WriteLine("\t//---- Metalness ----");
                    if (TextureMetalnessPath != "")
                    {
                        writer.WriteLine($"\tTextureMetalness \"{TextureMetalnessPath}\"");
                    }
                    else
                    {
                        writer.WriteLine("\tg_flMetalness \"0.000\"");
                    }
                    writer.WriteLine("");
                    writer.WriteLine("\t//---- Normal ----");
                    if (TextureNormalPath != "")
                    {
                        writer.WriteLine($"\tTextureNormal \"{TextureNormalPath}\"");
                    }
                    else
                    {
                        writer.WriteLine("\tTextureNormal \"materials/default/default_normal.tga\"");
                    }
                    writer.WriteLine("");
                    writer.WriteLine("\t//---- Roughness ----");
                    if (TextureRoughnessPath != "")
                    {
                        writer.WriteLine($"\tTextureRoughness \"{TextureRoughnessPath}\"");
                    }
                    else
                    {
                        writer.WriteLine("\tTextureRoughness \"materials/default/default_rough.tga\"");
                    }

                    if (TextureSelfIlluminationPath != "")
                    {
                        writer.WriteLine("\t//---- Self Illumination ----");
                        writer.WriteLine($"\tg_flSelfIllumAlbedoFactor \"1.000\"");
                        writer.WriteLine($"\tg_flSelfIllumBrightness \"0.000\"");
                        writer.WriteLine($"\tg_flSelfIllumScale \"1.000\"");
                        writer.WriteLine($"\tg_vSelfIllumScrollSpeed \"[0.000 0.000]\"");
                        writer.WriteLine($"\tg_vSelfIllumTint \"[1.000000 1.000000 1.000000 0.000000]\"");
                        writer.WriteLine($"\tTextureSelfIllumMask \"{TextureSelfIlluminationPath}\"");
                    }

                    
                    writer.WriteLine("");
                    writer.WriteLine("\t//---- Texture Coordinates ----");
                    writer.WriteLine("\tg_nScaleTexCoordUByModelScaleAxis \"0\"");
                    writer.WriteLine("\tg_nScaleTexCoordVByModelScaleAxis \"0\"");
                    writer.WriteLine("\tg_vTexCoordOffset \"[0.000 0.000]\"");
                    writer.WriteLine("\tg_vTexCoordScale \"[1.000 1.000]\"");
                    writer.WriteLine("\tg_vTexCoordScrollSpeed \"[0.000 0.000]\"");
                    
                    if (TextureOpacityPath != "")
                    {
                        writer.WriteLine($"\tg_flOpacityScale \"1.000\"");
                        writer.WriteLine($"\tTextureTranslucency \"{TextureOpacityPath}\"");
                    }
                    
                    writer.WriteLine("}");
                }
            }
        }
        Message.ShowSnack();
        LogManage.AddLog("所有 vmat 文件已创建完成。");

        
    }
    //匹配文件类型
    string MatchType(string filename)
    {
        var fileName = Path.GetFileNameWithoutExtension(filename.ToLower());
        foreach (var (type, suffixes) in pbrMap)
        {
            foreach (var suffix in suffixes)
            {
                if (fileName.Contains(suffix))
                    return type;
            }
        }
        return null;
    }
    // 清除日志
    private void ClearLog(object sender, MouseButtonEventArgs e)
    {
        LogManage.Clear();
    }
    // 打开设置窗口
    private void OpenSetting(object sender, MouseButtonEventArgs e)
    {
        var settingWindow = new Setting();
        settingWindow.ShowDialog();
    }
    // 打开 materials 文件夹
    private void OpenDir(object sender, RoutedEventArgs e)
    {
       // 打开 materials 文件夹
        var folderPath = "materials";
        if (Directory.Exists(folderPath))
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = folderPath,
                UseShellExecute = true
            });
        }
        else
        {
            LogManage.AddLog("materials 文件夹不存在，请先创建。");
            Message.ShowSnack("错误", "materials 文件夹不存在，请先创建。", ControlAppearance.Danger,
                new SymbolIcon(SymbolRegular.ErrorCircle20), 3);
        }
    }
    //下拉框选择
    private void GameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (GameComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content != null)
        {
            var selectGame = selectedItem.Content.ToString() ?? throw new InvalidOperationException();
            // 读取配置文件
            try
            {
                var configPath = "Config/config.toml";
                if (!File.Exists(configPath)) return;

                TextReader reader = new StreamReader(configPath);
                var table = TOML.Parse(reader);

                if (table[selectGame] is TomlTable gameSection &&
                    gameSection["Shader"] is TomlArray shaderArray)
                {
                    ShaderComboBox.Items.Clear();

                    foreach (var shader in shaderArray)
                    {
                        ShaderComboBox.Items.Add(new ComboBoxItem
                        {
                            Content = shader.ToString()
                        });
                    }
                    ShaderComboBox.SelectedIndex = 0;
                }
                SSR = table[selectGame]["SSR"];
            }
            catch (Exception exception)
            {
                LogManage.AddLog($"读取配置文件失败: {exception.Message}");
                Message.ShowSnack("错误", "读取配置文件失败，请检查文件格式。", ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.ErrorCircle20), 3);
            }
            
        }
    }

    private void ClearDir(object sender, RoutedEventArgs e)
    {
        //先弹出二次确认页面
        var result = Message.ShowMessageBox("确认", "是否清除 materials 文件夹下的所有 vmat 文件？");
        if (result.Result != MessageBoxResult.Primary) return;
        //清除 materials 文件夹下的所有 vmat 文件
        var folderPath = "materials";
        if (Directory.Exists(folderPath))
        {
            var vmatFiles = Directory.GetFiles(folderPath, "*.vmat", SearchOption.AllDirectories);
            foreach (var file in vmatFiles)
            {
                try
                {
                    File.Delete(file);
                    LogManage.AddLog($"已删除文件: {file}");
                }
                catch (Exception ex)
                {
                    LogManage.AddLog($"删除文件失败: {file}, 错误: {ex.Message}");
                }
            }
            Message.ShowSnack("提示", "已清除所有 vmat 文件。", ControlAppearance.Success,
                new SymbolIcon(SymbolRegular.CheckmarkCircle20), 3);
        }
        else
        {
            LogManage.AddLog("materials 文件夹不存在");
            Message.ShowSnack("错误", "materials 文件夹不存在", ControlAppearance.Danger,
                new SymbolIcon(SymbolRegular.ErrorCircle20), 3);
        }
    }
}
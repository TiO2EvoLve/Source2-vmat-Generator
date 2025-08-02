using System.IO;
using System.Windows;
using MaterialsCreate.Manage;
using MaterialsCreate.ViewModel;
using Newtonsoft.Json;

namespace MaterialsCreate;

public partial class Setting
{
    private const string ConfigPath = "Config/FileType.json";
    private TextureKeywordViewModel _viewModel = new();
    public Setting()
    {
        InitializeComponent();
        DataContext = _viewModel;
        LoadConfig();
    }
    private void LoadConfig()
    {
        if (File.Exists(ConfigPath))
        {
            LogManage.AddLog("加载配置文件: " + ConfigPath);
            // 读取配置文件
            try
            {
                var json = File.ReadAllText(ConfigPath);
                var dict = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
                _viewModel.Load(dict);
            }
            catch(Exception ex)
            {
                LogManage.AddLog("加载配置文件失败: " + ex.Message);
                MessageBox.Show("加载配置文件失败，请检查文件格式。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else
        {
            LogManage.AddLog("配置文件不存在，使用默认配置。");
            // 默认结构
            var dict = new Dictionary<string, List<string>>
            {
                ["BaseColor"] = ["_albedo", "_diffuse", "_basecolor", "_color", "_col", "_bc", "_diff"],
                ["AO"] = ["_ao", "_ambientocclusion", "_occlusion", "_ambocc", "_aoc", "_occl"],
                ["Normal"] = ["_normal", "_nor", "_norm", "_nrm", "_normalmap", "_nml", "_bump", "_n"],
                ["Roughness"] = ["_roughness", "_rou", "_rgh", "_gloss", "_gls", "_rough", "_specular"],
                ["Metal"] = ["_metallic", "_met", "_metal", "_mtl", "_metalness", "_metall"],
                ["SelfIllum"] = ["_selfillum", "_illum", "_glowmask", "_emit", "_emissive", "_light"],
                ["Opacity"] = ["_translucent", "_trans", "_opacity", "_opa", "_alpha"],
            };
            _viewModel.Load(dict);
        }
    }

    private void OnSaveClicked(object sender, RoutedEventArgs e)
    {
        try
        {
            var dict = _viewModel.GetDict();
            var json = JsonConvert.SerializeObject(dict, Formatting.Indented);
            File.WriteAllText(ConfigPath, json);
            MessageBox.Show("配置已保存", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception exception)
        {
            LogManage.AddLog("保存配置文件失败: " + exception.Message);
            MessageBox.Show("保存配置文件失败，请确保格式正确。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        
    }
}
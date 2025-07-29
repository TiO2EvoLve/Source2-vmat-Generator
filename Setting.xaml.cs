using System.IO;
using System.Windows;
using MaterialsCreate.ViewModel;
using Newtonsoft.Json;

namespace MaterialsCreate;

public partial class Setting : Window
{
    private const string ConfigPath = "Config/FileType.json";
    private TextureKeywordViewModel _viewModel = new();
    public Setting()
    {
        InitializeComponent();
        this.DataContext = _viewModel;
        LoadConfig();
    }
    private void LoadConfig()
    {
        if (File.Exists(ConfigPath))
        {
            var json = File.ReadAllText(ConfigPath);
            var dict = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
            _viewModel.Load(dict);
        }
        else
        {
            Console.WriteLine("配置文件不存在，使用默认配置。");
            // 默认结构
            var dict = new Dictionary<string, List<string>>
            {
                ["BaseColor"] = new() { "_albedo", "_diffuse", "_basecolor", "_color", "_col", "_bc", "_diff" },
                ["AO"] = new() { "_ao", "_ambientocclusion", "_occlusion", "_ambocc", "_aoc", "_occl" },
                ["Normal"] = new() { "_normal", "_nor", "_norm", "_nrm", "_normalmap", "_nml", "_bump", "_n" },
                ["Roughness"] = new() { "_roughness", "_rou", "_rgh", "_gloss", "_gls", "_rough", "_specular" },
                ["Metal"] = new() { "_metallic", "_met", "_metal", "_mtl", "_metalness", "_metall" },
                ["SelfIllum"] = new() { "_selfillum", "_illum", "_glowmask", "_emit", "_emissive", "_light" },
                ["Opacity"] = new() { "_translucent", "_trans", "_opacity", "_opa", "_alpha" },
            };
            _viewModel.Load(dict);
        }
    }

    private void OnSaveClicked(object sender, RoutedEventArgs e)
    {
        var dict = _viewModel.GetDict();
        var json = JsonConvert.SerializeObject(dict, Formatting.Indented);
        File.WriteAllText(ConfigPath, json);
        MessageBox.Show("配置已保存", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
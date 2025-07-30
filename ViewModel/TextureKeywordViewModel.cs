using System.ComponentModel;

namespace MaterialsCreate.ViewModel;

public class TextureKeywordViewModel : INotifyPropertyChanged
{
    private Dictionary<string, List<string>> _dict;

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Load(Dictionary<string, List<string>> dict)
    {
        _dict = dict;
        OnPropertyChanged("");
    }

    public Dictionary<string, List<string>> GetDict() => _dict;

    private string Get(string key) =>
        _dict.TryGetValue(key, out var list) ? string.Join(",", list) : "";

    private void Set(string key, string value)
    {
        _dict[key] = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        OnPropertyChanged(key);
    }

    public string BaseColor
    {
        get => Get("BaseColor");
        set => Set("BaseColor", value);
    }

    public string Normal
    {
        get => Get("Normal");
        set => Set("Normal", value);
    }

    public string Roughness
    {
        get => Get("Roughness");
        set => Set("Roughness", value);
    }

    public string AO
    {
        get => Get("AO");
        set => Set("AO", value);
    }

    public string Metal
    {
        get => Get("Metal");
        set => Set("Metal", value);
    }

    public string SelfIllum
    {
        get => Get("SelfIllum");
        set => Set("SelfIllum", value);
    }

    public string Opacity
    {
        get => Get("Opacity");
        set => Set("Opacity", value);
    }

    protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
namespace MaterialsCreate.Manage;

public class LogManage
{
    static LogManage()
    {
        var rccWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        if (rccWindow != null) richTextBox = rccWindow.log_text;
    }

    private static RichTextBox richTextBox { get; }

    public static void Clear()
    {
        richTextBox.Document.Blocks.Clear();
        var paragraph = new Paragraph
        {
            LineHeight = 5,
            FontFamily = new FontFamily("Microsoft YaHei"),
            FontSize = 12
        };

        richTextBox.Document.Blocks.Add(paragraph);
    }

    public static void AddLog(string log)
    {
        var now = DateTime.Now;
        log = $"[{now:HH:mm:ss}] {log}";
        richTextBox.AppendText(log + "\n");
        richTextBox.ScrollToEnd();
    }
}
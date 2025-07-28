using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Controls;
using MessageBox = Wpf.Ui.Controls.MessageBox;

namespace MaterialsCreate.Manage;

public class Message
{
     static Message()
    {
        // 获取 RCC 窗口的实例
        var rccWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        if (rccWindow != null)
        {
            // 访问 RootContentDialog 组件
            var contentDialog = rccWindow.RootContentDialog;
            var SnackbarPresenter = rccWindow.SnackbarPresenter;
            dialogService = new ContentDialogService();
            snackbarService = new SnackbarService();

            dialogService.SetDialogHost(contentDialog);
            snackbarService.SetSnackbarPresenter(SnackbarPresenter);
        }
    }

    private static ISnackbarService snackbarService { get; }
    private static IContentDialogService dialogService { get; }

    // 显示MessageBox消息框
    public static void ShowMessageBox()
    {
        var uiMessageBox = new MessageBox
        {
            Title = "成功",
            Content = "文件已保存到桌面",
            CloseButtonText = "确定"
        };
        uiMessageBox.ShowDialogAsync();
    }

    // 显示MessageBox消息框重载
    public static void ShowMessageBox(string Title, string Content)
    {
        var uiMessageBox = new MessageBox
        {
            Title = Title,
            Content = Content,
            CloseButtonText = "确定"
        };
        uiMessageBox.ShowDialogAsync();
    }

    // 显示Snackbar消息框
    public static void ShowSnack()
    {
        snackbarService.Show("成功",
            "文件已保存到桌面",
            ControlAppearance.Success,
            new SymbolIcon(SymbolRegular.Checkmark20), TimeSpan.FromSeconds(3));
    }

    // 显示Snackbar消息框重载
    public static void ShowSnack(string Title, string Message, ControlAppearance ControlAppearance,
        SymbolIcon SymbolIcon, int Seconds)
    {
        try
        {
            snackbarService.Show(Title,
                Message,
                ControlAppearance,
                SymbolIcon, TimeSpan.FromSeconds(Seconds));
        }
        catch (Exception e)
        {
            ShowMessageBox("错误", e.Message);
        }
    }
}
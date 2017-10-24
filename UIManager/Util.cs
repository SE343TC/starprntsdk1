using System;
using System.Windows;

namespace StarPRNTSDK
{
    public static class Util
    {
        public static void GoNextPage(Uri uri)
        {
            MainWindow mainWindow = GetMainWindow();

            mainWindow.NavigateNextPage(uri);
        }

        public static void GoBackPage()
        {
            MainWindow mainWindow = GetMainWindow();

            if (mainWindow.CanGoBack)
            {
                mainWindow.GoBack();
            }
        }

        public static void GoBackMainPage()
        {
            MainWindow mainWindow = GetMainWindow();

            mainWindow.NavigateNextPage(new Uri("MainPage.xaml", UriKind.Relative));

            mainWindow.RemoveBackEntry();
        }

        public static MainWindow GetMainWindow()
        {
            return (MainWindow)Application.Current.MainWindow; ;
        }

        public static void FocusMainWindow()
        {
            MainWindow mainWindow = GetMainWindow();

            mainWindow.WindowState = WindowState.Normal;
            mainWindow.Focus();
        }

        public static void NotifyAllReceiptsIsRegisteredStatusChanged()
        {
            SharedInformationManager.AllReceiptsInfoManager.NotifyIsRegisteredPropertyChanged();
        }

        public static void ShowMessage(string caption, string message)
        {
            SelectSettingWindow dialog = new SelectSettingWindow(caption, message, Visibility.Visible, Visibility.Collapsed);

            dialog.Owner = GetMainWindow();
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            dialog.ShowDialog();
        }

        public static bool ShowConfirmMessage(string caption, string message)
        {
            SelectSettingWindow confirmWindow = new SelectSettingWindow(caption, message);
            confirmWindow.Owner = GetMainWindow();
            confirmWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            return (bool)confirmWindow.ShowDialog(); ;
        }
    }
}

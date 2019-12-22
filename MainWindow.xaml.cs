using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;

namespace AvaloniaToastNotifications
{
    public class MainWindow : Window
    {
        private readonly string _appUserModelID;
        private readonly string _shortcutName;

        public MainWindow()
        {
            InitializeComponent();
            _appUserModelID = $"{Application.Current.Name}.App";
            _shortcutName = Application.Current.Name;

            SetCurrentProcessExplicitAppUserModelID(_appUserModelID);

            var path = Assembly.GetExecutingAssembly().Location;
            using var shortcut = new ShellLink
            {
                TargetPath = path,
                Arguments = string.Empty,
                AppUserModelID = _appUserModelID
            };

            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var startMenuPath = System.IO.Path.Combine(appData, @"Microsoft\Windows\Start Menu\Programs");
            var shortcutFile = System.IO.Path.Combine(startMenuPath, $"{_shortcutName}.lnk");

            shortcut.Save(shortcutFile);
        }

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern void SetCurrentProcessExplicitAppUserModelID(
            [MarshalAs(UnmanagedType.LPWStr)] string AppID);

        [UsedImplicitly]
        public void Click(object sender, RoutedEventArgs args)
        {
            const string title = "The current time is";
            var timeString = $"{DateTime.Now:HH:mm:ss}";
            const string thomasImage = "https://www.thomasclaudiushuber.com/thomas.jpg";

            var toastXmlString =
                $@"<toast><visual>
            <binding template='ToastGeneric'>
            <text>{title}</text>
            <text>{timeString}</text>
            <image src='{thomasImage}'/>
            </binding>
        </visual></toast>";

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(toastXmlString);

            var toastNotification = new ToastNotification(xmlDoc);

            var toastNotifier = ToastNotificationManager.CreateToastNotifier(_appUserModelID);
            toastNotifier.Show(toastNotification);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
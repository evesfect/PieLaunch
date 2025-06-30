using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using PieLaunch.Configuration;
using Application = System.Windows.Application;

namespace PieLaunch
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private NotifyIcon? notifyIcon;
        private MainWindow? mainWindow;
        public static ConfigManager ConfigManager { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize configuration manager
            ConfigManager = new ConfigManager();

            // Create system tray icon
            notifyIcon = new NotifyIcon()
            {
                Icon = SystemIcons.Application, // Using default system icon for now
                Visible = true,
                Text = "PieLaunch"
            };

            // Handle tray icon click
            notifyIcon.Click += NotifyIcon_Click;

            // Create context menu for tray icon
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Show", null, Show_Click);
            contextMenu.Items.Add("Exit", null, Exit_Click);
            notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void NotifyIcon_Click(object? sender, EventArgs e)
        {
            ShowMainWindow();
        }

        private void Show_Click(object? sender, EventArgs e)
        {
            ShowMainWindow();
        }

        private void ShowMainWindow()
        {
            if (mainWindow == null)
            {
                mainWindow = new MainWindow();
                mainWindow.Closed += MainWindow_Closed;
            }

            mainWindow.Show();
            mainWindow.WindowState = WindowState.Normal;
            mainWindow.Activate();
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            mainWindow = null;
        }

        private void Exit_Click(object? sender, EventArgs e)
        {
            notifyIcon?.Dispose();
            Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            notifyIcon?.Dispose();
            base.OnExit(e);
        }
    }
}
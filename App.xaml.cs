using System.Configuration;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using PieLaunch.Configuration;
using PieLaunch.Hotkeys;
using Application = System.Windows.Application;

namespace PieLaunch
{
    public partial class App : Application
    {
        private NotifyIcon? notifyIcon;
        private MessageWindow? messageWindow;
        private MainWindow? mainWindow;
        private Mutex? mutex;
        private HotkeyManager? hotkeyManager;
        public static ConfigManager ConfigManager { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            // Single instance check
            mutex = new Mutex(true, "PieLaunch_SingleInstance", out bool isNewInstance);

            if (!isNewInstance)
            {
                // Another instance is already running
                Shutdown();
                return;
            }

            base.OnStartup(e);

            // Initialize configuration manager
            ConfigManager = new ConfigManager();

            // Create system tray icon
            notifyIcon = new NotifyIcon()
            {
                Icon = SystemIcons.Application,
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

            // Start hotkey manager
            hotkeyManager = new HotkeyManager();
            if (!hotkeyManager.Start())
            {
                System.Windows.MessageBox.Show("Failed to start hotkey manager. Hotkeys will not be available.",
                                "PieLaunch", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            // Create invisible message window for hotkeys
            messageWindow = new MessageWindow();
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
            hotkeyManager?.Stop();
            notifyIcon?.Dispose();
            mutex?.Dispose();
            Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            hotkeyManager?.Stop();
            messageWindow?.Close();
            notifyIcon?.Dispose();
            mutex?.Dispose();
            base.OnExit(e);
        }
    }
}
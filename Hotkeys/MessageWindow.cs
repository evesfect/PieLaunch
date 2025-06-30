using System;
using System.Windows;
using System.Windows.Interop;

namespace PieLaunch.Hotkeys
{
    public class MessageWindow : Window
    {
        public const int WM_PIELAUNCH_COMMAND = 0x8001;

        public MessageWindow()
        {
            // Make this window invisible and non-interactive
            Width = 0;
            Height = 0;
            WindowStyle = WindowStyle.None;
            ShowInTaskbar = false;
            ShowActivated = false;
            Visibility = Visibility.Hidden;

            // Need to show it once to create the handle
            Show();
            Hide();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var source = PresentationSource.FromVisual(this) as HwndSource;
            source?.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_PIELAUNCH_COMMAND)
            {
                var commandCode = wParam.ToInt32();
                var hotkeys = App.ConfigManager.OrderedHotkeys;

                if (commandCode > 0 && commandCode <= hotkeys.Count)
                {
                    var action = hotkeys[commandCode - 1].Key;
                    HotkeyActions.ExecuteAction(action);
                }

                handled = true;
                return IntPtr.Zero;
            }
            return IntPtr.Zero;
        }
    }
}
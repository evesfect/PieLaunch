using System;
using System.Windows;
using System.Windows.Interop;
using PieLaunch.Hotkeys;

namespace PieLaunch.Commands
{
    public class WindowMessageHandler
    {
        public const int WM_PIELAUNCH_COMMAND = 0x8001;
        private HwndSource? hwndSource;

        public void Initialize(Window window)
        {
            var helper = new WindowInteropHelper(window);
            hwndSource = HwndSource.FromHwnd(helper.Handle);
            hwndSource.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_PIELAUNCH_COMMAND)
            {
                var commandCode = wParam.ToInt32();
                var hotkeys = App.ConfigManager.OrderedHotkeys;

                // Command codes are 1-based, array is 0-based
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

        public void Cleanup()
        {
            hwndSource?.RemoveHook(WndProc);
            hwndSource?.Dispose();
        }
    }
}
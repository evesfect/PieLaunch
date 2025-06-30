using System;
using PieLaunch.Windows;

namespace PieLaunch.Hotkeys
{
    public static class HotkeyActions
    {
        public static void ExecuteAction(string action)
        {
            var activeWindow = WindowManager.GetForegroundWindow();
            if (activeWindow == IntPtr.Zero)
                return;

            switch (action.ToLower())
            {
                case "snap-left":
                    WindowManager.SnapWindowToLeftHalf(activeWindow);
                    break;
                case "snap-right":
                    WindowManager.SnapWindowToRightHalf(activeWindow);
                    break;
                case "snap-top":
                    WindowManager.SnapWindowToTopHalf(activeWindow);
                    break;
                case "snap-bottom":
                    WindowManager.SnapWindowToBottomHalf(activeWindow);
                    break;
                case "snap-topleft":
                    WindowManager.SnapWindowToTopLeft(activeWindow);
                    break;
                case "snap-topright":
                    WindowManager.SnapWindowToTopRight(activeWindow);
                    break;
                case "snap-bottomleft":
                    WindowManager.SnapWindowToBottomLeft(activeWindow);
                    break;
                case "snap-bottomright":
                    WindowManager.SnapWindowToBottomRight(activeWindow);
                    break;
                case "maximize":
                    WindowManager.MaximizeWindow(activeWindow);
                    break;
                case "minimize":
                    WindowManager.MinimizeWindow(activeWindow);
                    break;
                case "restore":
                    WindowManager.RestoreWindow(activeWindow);
                    break;
                // Future actions can be added here
                default:
                    Console.WriteLine($"Unknown action: {action}");
                    break;
            }
        }
    }
}
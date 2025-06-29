using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace PieLaunch.Windows
{
    public class WindowManager
    {
        #region Win32 API Declarations
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll")]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_APPWINDOW = 0x00040000;
        private const int SW_RESTORE = 9;
        private const int SW_MAXIMIZE = 3;
        private const int SW_SHOW = 5;
        private const int SW_MINIMIZE = 6;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_SHOWNORMAL = 1;
        private const uint GW_OWNER = 4;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_SHOWWINDOW = 0x0040;
        private const int MONITOR_DEFAULTTONEAREST = 2;

        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public RECT rcNormalPosition;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }
        #endregion

        public class WindowInfo
        {
            public IntPtr Handle { get; set; }
            public string Title { get; set; } = string.Empty;
            public uint ProcessId { get; set; }
            public string ProcessName { get; set; } = string.Empty;
            public string ExecutablePath { get; set; } = string.Empty;
        }

        public static List<WindowInfo> GetTaskbarVisibleWindows()
        {
            var windows = new List<WindowInfo>();
            var seenProcessIds = new HashSet<uint>();

            EnumWindows((hWnd, lParam) =>
            {
                if (IsTaskbarWindow(hWnd))
                {
                    var windowInfo = GetWindowInfo(hWnd);
                    if (windowInfo != null &&
                        !string.IsNullOrWhiteSpace(windowInfo.Title) &&
                        !IsSystemOrUWPWindow(windowInfo))
                    {
                        // For UWP apps that might have multiple windows, only keep the first one
                        if (!seenProcessIds.Contains(windowInfo.ProcessId) ||
                            !windowInfo.ExecutablePath.Contains("ApplicationFrameHost"))
                        {
                            windows.Add(windowInfo);
                            seenProcessIds.Add(windowInfo.ProcessId);
                        }
                    }
                }
                return true;
            }, IntPtr.Zero);

            // Remove duplicate UWP entries (keep only the actual app, not the frame host)
            var filteredWindows = new List<WindowInfo>();
            var processGroups = windows.GroupBy(w => w.Title);

            foreach (var group in processGroups)
            {
                if (group.Count() > 1)
                {
                    // If we have multiple windows with the same title, prefer the one that's NOT ApplicationFrameHost
                    var preferredWindow = group.FirstOrDefault(w => !w.ExecutablePath.Contains("ApplicationFrameHost"))
                                        ?? group.First();
                    filteredWindows.Add(preferredWindow);
                }
                else
                {
                    filteredWindows.Add(group.First());
                }
            }

            return filteredWindows;
        }

        private static bool IsTaskbarWindow(IntPtr hWnd)
        {
            if (!IsWindowVisible(hWnd))
                return false;

            // Get window style
            int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);

            // Skip tool windows
            if ((exStyle & WS_EX_TOOLWINDOW) != 0)
                return false;

            // Check if window has no owner or has WS_EX_APPWINDOW style
            IntPtr owner = GetWindow(hWnd, GW_OWNER);
            return owner == IntPtr.Zero || (exStyle & WS_EX_APPWINDOW) != 0;
        }

        private static bool IsSystemOrUWPWindow(WindowInfo windowInfo)
        {
            var executablePath = windowInfo.ExecutablePath.ToLower();

            // List of system processes and paths to exclude
            var excludedPaths = new[]
            {
                "applicationframehost.exe",
                "systemsettings.exe",
                "textinputhost.exe",
                "shellexperiencehost.exe",
                "searchhost.exe",
                "startmenuexperiencehost.exe",
                "lockapp.exe"
            };

            // Check if it's a system process
            foreach (var excluded in excludedPaths)
            {
                if (executablePath.Contains(excluded))
                    return true;
            }

            // Additional checks for system directories
            if (executablePath.Contains(@"\windows\immersivecontrolpanel\") ||
                executablePath.Contains(@"\windows\system32\") &&
                !executablePath.Contains("cmd.exe") &&
                !executablePath.Contains("powershell.exe") &&
                !executablePath.Contains("notepad.exe"))
            {
                return true;
            }

            return false;
        }

        private static WindowInfo? GetWindowInfo(IntPtr hWnd)
        {
            try
            {
                // Get window title
                int length = GetWindowTextLength(hWnd);
                if (length == 0)
                    return null;

                var titleBuilder = new StringBuilder(length + 1);
                GetWindowText(hWnd, titleBuilder, titleBuilder.Capacity);

                // Get process info
                uint processId;
                GetWindowThreadProcessId(hWnd, out processId);

                var process = Process.GetProcessById((int)processId);

                return new WindowInfo
                {
                    Handle = hWnd,
                    Title = titleBuilder.ToString(),
                    ProcessId = processId,
                    ProcessName = process.ProcessName,
                    ExecutablePath = process.MainModule?.FileName ?? string.Empty
                };
            }
            catch
            {
                return null;
            }
        }

        public static void LaunchOrFocusApp(string executablePath)
        {
            // First, check if the app is already running
            var windows = GetTaskbarVisibleWindows();
            var existingWindow = windows.FirstOrDefault(w =>
                string.Equals(w.ExecutablePath, executablePath, StringComparison.OrdinalIgnoreCase));

            if (existingWindow != null)
            {
                // App is already running, bring it to foreground
                FocusWindow(existingWindow.Handle);
            }
            else
            {
                // App is not running, launch it
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = executablePath,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to launch application: {ex.Message}", ex);
                }
            }
        }

        public static void FocusWindow(IntPtr hWnd)
        {
            if (IsIconic(hWnd))
            {
                // Window is minimized, restore it
                ShowWindow(hWnd, SW_RESTORE);
            }
            else
            {
                // Window is not minimized, show it
                ShowWindow(hWnd, SW_SHOW);
            }

            SetForegroundWindow(hWnd);
        }

        public static void MaximizeWindow(IntPtr hWnd)
        {
            ShowWindow(hWnd, SW_SHOWMAXIMIZED);
            SetForegroundWindow(hWnd);
        }

        public static void MinimizeWindow(IntPtr hWnd)
        {
            ShowWindow(hWnd, SW_MINIMIZE);
        }

        public static void RestoreWindow(IntPtr hWnd)
        {
            ShowWindow(hWnd, SW_SHOWNORMAL);
        }

        public static bool IsWindowMaximized(IntPtr hWnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(hWnd, ref placement);
            return placement.showCmd == SW_SHOWMAXIMIZED;
        }

        public static bool IsWindowMinimized(IntPtr hWnd)
        {
            return IsIconic(hWnd);
        }
    }
}
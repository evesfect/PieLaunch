using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PieLaunch.Hotkeys
{
    public class HotkeyManager
    {
        private Process? ahkProcess;
        private readonly string ahkExecutablePath;
        private readonly string ahkScriptPath;

        public HotkeyManager()
        {
            ahkExecutablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Resources", "AutoHotkey", "AutoHotkey64.exe");
            ahkScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PieLaunch.ahk");
        }

        public bool Start()
        {
            try
            {
                GenerateAhkScript();

                if (!File.Exists(ahkExecutablePath))
                {
                    throw new FileNotFoundException($"AutoHotkey executable not found at: {ahkExecutablePath}");
                }

                ahkProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = ahkExecutablePath,
                        Arguments = $"\"{ahkScriptPath}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WorkingDirectory = Path.GetDirectoryName(ahkExecutablePath)
                    }
                };

                ahkProcess.Start();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start AutoHotkey: {ex.Message}");
                return false;
            }
        }

        public void Stop()
        {
            if (ahkProcess != null && !ahkProcess.HasExited)
            {
                ahkProcess.Kill();
                ahkProcess.Dispose();
                ahkProcess = null;
            }
        }

        private void GenerateAhkScript()
        {
            var hotkeys = App.ConfigManager.OrderedHotkeys;
            var sb = new StringBuilder();

            sb.AppendLine("; PieLaunch Hotkey Script - Auto-generated");
            sb.AppendLine("; Do not edit manually - changes will be overwritten");
            sb.AppendLine();
            sb.AppendLine("; Constants");
            sb.AppendLine("WM_PIELAUNCH_COMMAND := 0x8001");
            sb.AppendLine();
            sb.AppendLine("; Send command function");
            sb.AppendLine("SendPieLaunchCommand(commandCode) {");
            sb.AppendLine("    hwnd := WinExist(\"PieLaunch Message Window ahk_class PieLaunchMessageWindow\")");
            sb.AppendLine("    if (hwnd) {");
            sb.AppendLine("        PostMessage(WM_PIELAUNCH_COMMAND, commandCode, 0, , \"ahk_id \" . hwnd)");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("; Hotkeys");

            // Generate hotkeys with auto-assigned codes
            for (int i = 0; i < hotkeys.Count; i++)
            {
                var hotkey = hotkeys[i];
                var ahkHotkey = ConvertToAhkHotkey(hotkey.Value);
                var commandCode = i + 1; // 1-based indexing
                sb.AppendLine($"; {hotkey.Key}");
                sb.AppendLine($"{ahkHotkey}::SendPieLaunchCommand({commandCode})");
                sb.AppendLine();
            }

            File.WriteAllText(ahkScriptPath, sb.ToString());
        }

        private string ConvertToAhkHotkey(string hotkey)
        {
            // Just do simple replacements for common modifiers
            // Users can use AHK syntax directly if they want
            return hotkey
                .Replace("Win+", "#")
                .Replace("Alt+", "!")
                .Replace("Ctrl+", "^")
                .Replace("Shift+", "+");
        }
    }
}
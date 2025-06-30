using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tomlyn;
using Tomlyn.Model;

namespace PieLaunch.Configuration
{
    public class ConfigManager
    {
        private readonly string _configFilePath;
        private const string ConfigFileName = "config.toml";
        public List<KeyValuePair<string, string>> OrderedHotkeys { get; private set; } = new();

        public ConfigManager()
        {
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _configFilePath = Path.Combine(exeDirectory, ConfigFileName);
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            try
            {
                if (!File.Exists(_configFilePath))
                {
                    CreateDefaultConfig();
                    return;
                }

                string configContent = File.ReadAllText(_configFilePath);
                var tomlModel = Toml.ToModel(configContent);

                // Parse hotkeys maintaining order
                OrderedHotkeys.Clear();
                if (tomlModel.TryGetValue("hotkeys", out var hotkeysObj) && hotkeysObj is TomlTable hotkeysTable)
                {
                    foreach (var kvp in hotkeysTable)
                    {
                        OrderedHotkeys.Add(new KeyValuePair<string, string>(kvp.Key, kvp.Value?.ToString() ?? ""));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration: {ex.Message}");
                CreateDefaultConfig();
            }
        }

        private void CreateDefaultConfig()
        {
            var defaultConfig = @"# PieLaunch Configuration

[hotkeys]
# Window snapping hotkeys
snap-left = ""Win+Alt+Left""
snap-right = ""Win+Alt+Right""
snap-top = ""Win+Alt+Up""
snap-bottom = ""Win+Alt+Down""
snap-topleft = ""Win+Shift+1""
snap-topright = ""Win+Shift+2""
snap-bottomleft = ""Win+Shift+3""
snap-bottomright = ""Win+Shift+4""

# Window control hotkeys
maximize = ""Win+Alt+M""
minimize = ""Win+Alt+N""
restore = ""Win+Alt+R""

# Pie menu hotkeys (for future use)
# show-pie = ""Alt+Tab""
# save-preset = ""Alt+Shift+Tab""
";

            File.WriteAllText(_configFilePath, defaultConfig, Encoding.UTF8);
            LoadConfiguration(); // Reload after creating
        }
    }
}
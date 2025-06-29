using System;
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

        public TomlTable Configuration { get; private set; }

        public ConfigManager()
        {
            // Get the directory where the executable is located
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _configFilePath = Path.Combine(exeDirectory, ConfigFileName);

            Configuration = new TomlTable();
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
                Configuration = Toml.ToModel(configContent);
            }
            catch (Exception ex)
            {
                // If config is corrupt or cannot be parsed, create a new default config
                Console.WriteLine($"Error loading configuration: {ex.Message}");
                CreateDefaultConfig();
            }
        }

        private void CreateDefaultConfig()
        {
            Configuration = new TomlTable();
            SaveConfiguration();
        }

        public void SaveConfiguration()
        {
            try
            {
                string tomlString = Toml.FromModel(Configuration);
                File.WriteAllText(_configFilePath, tomlString, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving configuration: {ex.Message}");
                throw;
            }
        }

        public T GetValue<T>(string section, string key, T defaultValue = default!)
        {
            try
            {
                if (Configuration.TryGetValue(section, out var sectionObj) &&
                    sectionObj is TomlTable sectionTable &&
                    sectionTable.TryGetValue(key, out var value))
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
            }
            catch
            {
                // Return default value if conversion fails
            }

            return defaultValue;
        }

        public void SetValue(string section, string key, object value)
        {
            if (!Configuration.ContainsKey(section))
            {
                Configuration[section] = new TomlTable();
            }

            if (Configuration[section] is TomlTable sectionTable)
            {
                sectionTable[key] = value;
            }
        }
    }
}
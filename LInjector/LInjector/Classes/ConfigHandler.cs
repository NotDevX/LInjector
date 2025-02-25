﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace LInjector.Classes
{
    public class ConfigHandler
    {
        private static readonly string ConfigPath = ".\\config.json";

        public static bool topmost = false;
        public static bool autoattach = false;
        public static bool splashscreen { get; set; }
        public static bool sizable = false;
        public static bool debug = false;
        public static bool discord_rpc = true;
        public static bool options_collapsed = false;
        public static bool script_list = false;
        public static bool safe_mode = false;


        public static void DoConfig()
        {
            if (!File.Exists(ConfigPath))
            {
                var config = new Dictionary<string, object>
                {
                    { "autoattach", false },
                    { "splashscreen", true },
                    { "debug", false },
                    { "topmost", false },
                    { "discord_rpc", true },
                    { "safe_mode", false }
                };

                string jsonString = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.Create(ConfigPath).Close();
                File.WriteAllText(ConfigPath, jsonString);
            }
            else if (File.Exists(ConfigPath))
            {
                string jsonString = File.ReadAllText(ConfigPath);
                var config = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);

                if (config.TryGetValue("autoattach", out object autoAttachValue) && (bool)autoAttachValue)
                {
                    autoattach = true;
                }

                if (config.TryGetValue("splashscreen", out object splashscreenValue) && (bool)splashscreenValue)
                {
                    splashscreen = true;
                }

                if (config.TryGetValue("debug", out object debugValue) && (bool)debugValue)
                {
                    ConsoleManager.Initialize();
                    ConsoleManager.ShowConsole();
                    debug = true;
                }

                if (config.TryGetValue("topmost", out object topMostValue) && (bool)topMostValue)
                {
                    topmost = true;
                }

                if (config.TryGetValue("discord_rpc", out object discord_rpc) && (bool)discord_rpc)
                {
                    RPCManager.isEnabled = true;
                    discord_rpc = true;
                }

                if (config.TryGetValue("safe_mode", out object safe_mode) && (bool)safe_mode)
                {
                    safe_mode = true;
                }
            }
        }

        public static void SetConfigValue(string Name, bool Value)
        {
            try
            {
                string jsonContent = File.ReadAllText(ConfigPath);
                var configDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonContent);

                if (configDict.ContainsKey(Name))
                {
                    configDict[Name] = Value;
                }
                else
                {
                    CustomCw.Cw($"The value '{Name}' doesn't exist in the config", false, "error");
                    return;
                }

                string updatedJson = JsonConvert.SerializeObject(configDict, Formatting.Indented);
                File.WriteAllText(ConfigPath, updatedJson);
            }
            catch { }
        }
    }
}
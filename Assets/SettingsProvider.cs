using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;

public class SettingsProvider : MonoBehaviour
{
    [Serializable]
    public struct RectSize
    {
        public int width, height;
        public static explicit operator Vector2(RectSize self) {
            return new Vector2(self.width, self.height);
        }
    }
    [Serializable]
    public struct Settings
    {
        public int vrFontSize;
        public RectSize vrWindowSize;
    }

    public static Settings settings = new Settings
    {
        vrFontSize = 24,
        vrWindowSize = new RectSize
        {
            width = 640,
            height = 360
        }
    };

    public UnityEvent onSettingsLoaded;

    const string FILE_NAME = "settings.json";

#if UNITY_EDITOR
    string settingsFilePath = $"Assets/{FILE_NAME}";
#elif UNITY_STANDALONE_WIN
    string settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\'), FILE_NAME);
#endif

    void Awake()
    {
        if (!File.Exists(settingsFilePath)) {
            Debug.Log("The settings file was not found. Creating...");
            Save();
        }
        Load();
    }

    void Load()
    {
        if (!File.Exists(settingsFilePath))
        {
            Debug.Log("The settings file was not found.");
            return;
        }

        try
        {
            using (StreamReader sr = new StreamReader(settingsFilePath))
            {
                settings = JsonConvert.DeserializeObject<Settings>(sr.ReadToEnd());
            }
            Debug.Log("Settings loaded successfully.");
            onSettingsLoaded.Invoke();
        }
        catch (Exception ex)
        {
            Debug.LogError("Loading settings failed: " + ex.Message);
        }
    }

    void Save()
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(settingsFilePath))
            {
                string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                sw.Write(json);
            }
            Debug.Log("Settings saved successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Saving settings failed: " + ex.Message);
        }
    }
}

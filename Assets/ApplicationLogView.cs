using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class ApplicationLogView : MonoBehaviour
{
    public GameObject DebugText;
    public GameObject DebugWindow;

    string log = "";
    string logFileName = "log.txt"; // Awakeにて起動日時に変更されます

    public string logFilePath
    {
        get
        {
#if UNITY_EDITOR
            string directory = "Temp/logs/";
#elif UNITY_STANDALONE_WIN
            string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\'), "logs/");
#endif
            return Path.Combine(directory, logFileName);
        }
    }

    void Awake()
    {
        Application.logMessageReceived += OnLog;  // ログ出力時のコールバックを登録
        logFileName = DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt";
    }

    public void OnLog(string logstr, string stacktrace, LogType type)
    {
        log += logstr + "\n";
        DebugText.GetComponent<Text>().text = log;
        // 常にTextの最下部（最新）を表示するように強制スクロール
        DebugWindow.GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
    }

    private void OnApplicationQuit()
    {
        SaveLog(logFilePath);
    }

    void SaveLog(string path)
    {
        // フォルダがなければ作成
        if (!Directory.Exists(Path.GetDirectoryName(path))) Directory.CreateDirectory(Path.GetDirectoryName(path));
        
        // ログをファイルに保存
        try
        {
            FileStream f = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
            using (StreamWriter writer = new StreamWriter(f))
            {
                writer.Write(log);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Saving log failed: " + ex.Message);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationLogView : MonoBehaviour
{
    public GameObject DebugText;
    public GameObject DebugWindow;

    void Awake()
    {
        Application.logMessageReceived += OnLog;  // ログ出力時のコールバックを登録
    }

    public void OnLog(string logstr, string stacktrace, LogType type)
    {
        DebugText.GetComponent<Text>().text += logstr;
        DebugText.GetComponent<Text>().text += "\n";
        // 常にTextの最下部（最新）を表示するように強制スクロール
        DebugWindow.GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
    }
}

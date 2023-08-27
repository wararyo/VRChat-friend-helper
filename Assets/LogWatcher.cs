using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.IO;
using System.Text.RegularExpressions;

public class LogWatcher : MonoBehaviour
{
    const string LOG_DIRECTORY = "%appdata%/../LocalLow/VRChat/VRChat/";
    const string LOG_FILTER = "output_log_*.txt";

    FileSystemWatcher watcher;

    [Serializable] public class StringEvent : UnityEvent<string> { }

    public StringEvent onUserJoined;
    public StringEvent onUserLeft;
    public StringEvent onWorldLoaded;
    public StringEvent onFriendRequestAccepted;
    public StringEvent onFriendRequestSent;

    // 今追跡しているログファイルと、そのリーダー
    private string currentLogFile = "";
    private FileStream file = null;
    private StreamReader reader = null;

    const string PATTERN_WORLD_LOADED = @"([0-9\.]+ [0-9:]+).+Joining or Creating Room: (.+)";
    const string PATTERN_USER_JOINED = @"([0-9\.]+ [0-9:]+).+\[Behaviour\] Initialized PlayerAPI ""(.+)"" is remote";
    const string PATTERN_USER_LEFT = @"([0-9\.]+ [0-9:]+).+\[Behaviour\] OnPlayerLeft (.+)";
    const string PATTERN_FRIEND_REQUEST_ACCEPTED = @"([0-9\.]+ [0-9:]+).+AcceptNotification for notification:.+ username:([^,]+),.*type: friendRequest.*";
    const string PATTERN_FRIEND_REQUEST_SENT = @"([0-9\.]+ [0-9:]+).+Send notification:.+ username:([^,]+),.*type:friendRequest,.+$";
    private Regex worldLoadedRegex, userJoinedRegex, userLeftRegex, friendRequestAcceptedRegex, friendRequestSentRegex;

    // ファイル変更イベント内でUnity関連の処理を実行すると動作が停止してしまう
    // Updateのタイミングでイベントを発行する
    private Queue<KeyValuePair<StringEvent, string>> eventQueue = new Queue<KeyValuePair<StringEvent, string>>();
    public void Enqueue(StringEvent e, string arg) { eventQueue.Enqueue(new KeyValuePair<StringEvent, string>(e, arg)); }

    void Start()
    {
        // 正規表現の初期化
        worldLoadedRegex = new Regex(PATTERN_WORLD_LOADED, RegexOptions.Compiled);
        userJoinedRegex = new Regex(PATTERN_USER_JOINED, RegexOptions.Compiled);
        userLeftRegex = new Regex(PATTERN_USER_LEFT, RegexOptions.Compiled);
        friendRequestAcceptedRegex = new Regex(PATTERN_FRIEND_REQUEST_ACCEPTED, RegexOptions.Compiled);
        friendRequestSentRegex = new Regex(PATTERN_FRIEND_REQUEST_SENT, RegexOptions.Compiled);

        // ファイル監視の初期化
        string directory = Path.GetFullPath(Environment.ExpandEnvironmentVariables(LOG_DIRECTORY));
        watcher = new FileSystemWatcher(directory, LOG_FILTER)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Attributes | NotifyFilters.FileName,
            IncludeSubdirectories = false,
            EnableRaisingEvents = true
        };

        watcher.Changed += new FileSystemEventHandler(OnFileChanged);
        watcher.Created += new FileSystemEventHandler(OnFileChanged);

        Debug.Log("Watching log files in : " + directory);
    }

    void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        // 今追跡しているファイルと違うファイルが変更されたときは、そちらを追跡する
        if (currentLogFile != e.FullPath)
        {
            if (reader != null) reader.Close();
            file = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            reader = new StreamReader(file);
            currentLogFile = e.FullPath;

            Debug.Log("Watching log file: " + e.FullPath);
        }

        // ログの追加された部分を読み、処理する
        // なぜかreader.Peek()は機能しないため、一旦全て読み、その後1行ずつ処理する
        StringReader log = new StringReader(reader.ReadToEnd());
        while (log.Peek() > 0) HandleLogLine(log.ReadLine());
    }

    // ログの1行を読んで、各種イベントを発行する
    void HandleLogLine(string line)
    {
        if (line == "") return;
        if (worldLoadedRegex.IsMatch(line)) // ワールド読み込み
        {
            Match match = worldLoadedRegex.Match(line);
            Enqueue(onWorldLoaded, match.Groups[2].ToString());
        }
        else if (userJoinedRegex.IsMatch(line)) // ユーザー参加
        {
            Match match = userJoinedRegex.Match(line);
            Enqueue(onUserJoined, match.Groups[2].ToString());
        }
        else if (userLeftRegex.IsMatch(line)) // ユーザー退出
        {
            Match match = userLeftRegex.Match(line);
            Enqueue(onUserLeft, match.Groups[2].ToString());
        }
        else if (friendRequestAcceptedRegex.IsMatch(line)) // フレンド申請の許可
        {
            Match match = friendRequestAcceptedRegex.Match(line);
            Enqueue(onFriendRequestAccepted, match.Groups[2].ToString());
        }
        else if (friendRequestSentRegex.IsMatch(line)) // フレンド申請を送信
        {
            Match match = friendRequestSentRegex.Match(line);
            Enqueue(onFriendRequestSent, match.Groups[2].ToString());
        }
    }

    void Update()
    {
        while (eventQueue.Count > 0)
        {
            KeyValuePair<StringEvent, string> e = eventQueue.Dequeue();
            e.Key.Invoke(e.Value);
        }
    }

    void OnApplicationQuit()
    {
        if (reader != null) reader.Close();
    }
}

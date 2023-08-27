using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// friends.csvからフレンドを読み込む
/// </summary>
public class FriendLoader : MonoBehaviour
{
    public List<Friend> friends { get; } = new List<Friend>();
    public string friendsFilePath
    {
        get
        {
#if UNITY_EDITOR
            return "Assets/friends.csv";
#elif UNITY_STANDALONE_WIN
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\'), "friends.csv");
#endif
        }
    }

    public string currentWorldName { get; private set; } = "";

    void Start()
    {
        LoadFriends();
    }

    void LoadFriends()
    {
        Debug.Log("Loading friends at " + friendsFilePath);

        try
        {
            using (StreamReader sr = new StreamReader(friendsFilePath))
            {
                while (0 <= sr.Peek())
                {
                    var line = sr.ReadLine();
                    if (line is null || line == "" || line.StartsWith("#")) continue;
                    var item = line.Split(',');
                    if (item is null || item.Length < 1) continue;
                    friends.Add(new Friend(item[0], item[1] != null ? item[1] : ""));
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Loading log failed: " + ex.Message);
        }

        Debug.Log(friends.Count + " friends has been loaded.");
    }

    public void OnWorldLoaded(string worldName)
    {
        currentWorldName = worldName;
    }

    public void AppendFriend(string userName)
    {
        string date = DateTime.Now.ToString("yyyyMMdd");
        try
        {
            using (StreamWriter writer = new StreamWriter(friendsFilePath, true))
            {
                writer.WriteLine(userName + "," + date + " " + currentWorldName);
            }
            Debug.Log(userName + " successfully added to friends.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Loading log failed: " + ex.Message);
        }
    }
}

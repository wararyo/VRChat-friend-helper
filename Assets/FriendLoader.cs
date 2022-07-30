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

    void Start()
    {
        LoadFriends();
    }

    void LoadFriends()
    {
#if UNITY_EDITOR
        string friendsFilePath = "Assets/friends.csv";
#elif UNITY_STANDALONE_WIN
        string friendsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\'), "friends.csv");
#endif
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
}

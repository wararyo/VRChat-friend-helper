using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FriendLoader))]
public class FriendDisplay : MonoBehaviour
{
    public Text text;
    public string currentWorldName { get; private set; } = "";
    public List<Friend> friendsInInstance { get; } = new List<Friend>();

    private List<Friend> friends { get { return GetComponent<FriendLoader>().friends; } }

    void Start()
    {
        
    }

    void Refresh()
    {
        text.text = "Friends in " + currentWorldName + "\n\n"
            + string.Join("\n", friendsInInstance.Select(x => x.name + ": " + x.description));
    }

    public void OnWorldLoaded(string worldName)
    {
        Debug.Log("\n" + "World: " + worldName);
        currentWorldName = worldName;
        friendsInInstance.Clear();
        Refresh();
    }
    public void OnUserJoined(string userName)
    {
        Friend f = friends.Find(x => x.name == userName );

        if (f == null)
        {
            Debug.Log(userName);
        }
        else
        {
            friendsInInstance.Add(f);
            Debug.Log(f.name + ": " + f.description);
            Refresh();
        }

    }
    public void OnUserLeft(string username)
    {
        friendsInInstance.RemoveAll(x => x.name == username);
        Debug.Log(username + " left.");
        Refresh();
    }
}

using System;

[Serializable]
public class Friend
{
    public string name;
    public string description;

    public Friend(string name, string description)
    {
        this.name = name;
        this.description = description;
    }
}

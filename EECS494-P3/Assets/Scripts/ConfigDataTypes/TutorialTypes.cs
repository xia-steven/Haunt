using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageList : Savable
{
    public List<MessageSet> allMessages;

    public MessageList()
    {
        allMessages = new List<MessageSet>();
    }

    public override string ToString()
    {
        string output = "{";
        for(int a = 0; a < allMessages.Count; ++a)
        {
            output += allMessages[a].ToString() + " ";
        }
        output += "}";
        return output;
    }
}

[System.Serializable]
public class MessageSet
{
    public List<string> messages;

    public MessageSet()
    {
        messages = new List<string>();
    }

    public override string ToString()
    {
        string output = "[";
        for (int a = 0; a < messages.Count; ++a)
        {
            output += messages[a] + " ";
        }
        output += "]";
        return output;
    }
}

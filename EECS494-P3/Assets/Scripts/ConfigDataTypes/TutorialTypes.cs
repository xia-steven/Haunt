using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMessages : Savable
{
    public List<TutorialMessage> allMessages;

    public TutorialMessages()
    {
        allMessages = new List<TutorialMessage>();
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
public class TutorialMessage
{
    public List<string> messages;

    public TutorialMessage()
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

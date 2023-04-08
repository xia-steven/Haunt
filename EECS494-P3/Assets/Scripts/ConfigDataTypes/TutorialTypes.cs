using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MessageList : Savable {
    public List<MessageSet> allMessages;
    public List<string> initialTutorial;

    public MessageList() {
        allMessages = new List<MessageSet>();
        initialTutorial = new List<string>();
    }

    public override string ToString() {
        var output = "{";

        foreach (var msg in allMessages) {
            output += msg + " ";
        }

        output += "}";
        return output;
    }
}

[System.Serializable]
public class MessageSet {
    public List<string> messages;

    public MessageSet() {
        messages = new List<string>();
    }

    public override string ToString() {
        var output = "[";
        foreach (var msg in messages) {
            output += msg + " ";
        }

        output += "]";
        return output;
    }
}
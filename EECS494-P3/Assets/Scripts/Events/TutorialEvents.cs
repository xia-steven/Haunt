using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The TutorialTriggerEvent is sent whenever the tutorial needs to send information 
/// from one script to another.
/// </summary>
public class TutorialTriggerEvent
{
    public int UUID;

    public TutorialTriggerEvent(int UUID_in)
    {
        UUID = UUID_in;
    }
}


/// <summary>
/// The TutorialMessageEvent is sent whenever there is a message from the tutorial NPC
/// to display to the player.
/// </summary>
public class TutorialMessageEvent
{
    public int messageID;

    public TutorialMessageEvent(int messageID_in)
    {
        messageID = messageID_in;
    }
}
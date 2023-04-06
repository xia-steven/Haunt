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
    public KeyCode keyToWaitFor;
    public int senderInstanceID;
    public bool unpauseBeforeFade;
    public bool pauseTime;

    public TutorialMessageEvent(int messageID_in, int senderInstanceID_in, bool pauseTime_in, KeyCode keyToWaitFor_in = KeyCode.Mouse0,
        bool unpauseBeforeFade_in = false)
    {
        messageID = messageID_in;
        keyToWaitFor = keyToWaitFor_in;
        senderInstanceID = senderInstanceID_in;
        unpauseBeforeFade = unpauseBeforeFade_in;
        pauseTime = pauseTime_in;
    }
}

/// <summary>
/// The TutorialLockCameraEvent is sent whenever the camera needs to be fixed for 
/// a tutorial sequence.
/// </summary>
public class TutorialLockCameraEvent
{
    public Vector3 cameraLockedLocation;
    
    public TutorialLockCameraEvent(Vector3 location)
    {
        cameraLockedLocation = location;
    }
}

/// <summary>
/// The TutorialLockCameraEvent is sent whenever the camera is unlocked 
/// a tutorial sequence.
/// </summary>
public class TutorialUnlockCameraEvent
{
}


/// <summary>
/// The TutorialDodgeStartEvent is sent whenever the tutorial system
/// needs to manually trigger a dodge
/// </summary>
public class TutorialDodgeStartEvent
{
    public Vector3 direction;

    public TutorialDodgeStartEvent(Vector3 direction_in)
    {
        direction = direction_in;
    }
}

/// <summary>
/// The TutorialDodgeEndEvent is sent whenever the tutorial system
/// needs to stop a manually triggered a dodge
/// </summary>
public class TutorialDodgeEndEvent
{
    
}

/// <summary>
/// The SpritePromptEvent is sent when a script wants to display
/// a sprite next to the player.  Usually the sprites will be tutorial
/// keys or interact prompts.
/// </summary>
public class SpritePromptEvent
{
    public Sprite sprite;
    public KeyCode dismissKey;
    public bool cancelPrompt = false;

    public SpritePromptEvent(Sprite sprite_in, KeyCode dismissKey_in)
    {
        sprite = sprite_in;
        dismissKey = dismissKey_in;
    }
}
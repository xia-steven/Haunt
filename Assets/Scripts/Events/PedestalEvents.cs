using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The pedestal destroyed event is sent when the player destroys a pedestal. <br/>
/// The pedestal's UUID must be specified in the constructor.
/// </summary>
class PedestalDestroyedEvent {
    public int pedestalUUID;
    public Vector3 pedestalLocation;

    public PedestalDestroyedEvent(int UUID, Vector3 location) {
        pedestalUUID = UUID;
        pedestalLocation = location;
    }

    public override string ToString() {
        return "Pedestal Destroyed Event Sent: Pedestal " + pedestalUUID + " died at location " + pedestalLocation + ".";
    }
}

/// <summary>
/// The pedestal repaired event is sent when the enemies repair a pedestal. <br/>
/// The pedestal's UUID must be specified in the constructor.
/// </summary>
class PedestalRepairedEvent {
    public int pedestalUUID;
    public Vector3 pedestalLocation;

    public PedestalRepairedEvent(int UUID, Vector3 location) {
        pedestalUUID = UUID;
        pedestalLocation = location;
    }

    public override string ToString() {
        return "Pedestal Repaired Event Sent: Pedestal " + pedestalUUID + " repaired at location " + pedestalLocation + ".";
    }
}


/// <summary>
/// The pedestal partial event is sent when the enemies start repairing a pedestal. <br/>
/// The pedestal's UUID must be specified in the constructor.
/// </summary>
class PedestalPartialEvent
{
    public int pedestalUUID;
    public bool turnOn = false;

    public PedestalPartialEvent(int UUID, bool turnOn_in)
    {
        pedestalUUID = UUID;
        turnOn = turnOn_in;
    }

    public override string ToString()
    {
        return "Pedestal Partial Event Sent: Pedestal " + pedestalUUID + " repaired.";
    }
}
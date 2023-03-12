using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The pedestal destroyed event is sent when a pedestal dies. <br/>
/// The pedestal's UUID must be specified in the constructor.
/// </summary>
class PedestalDestroyedEvent
{
    public int pedestalUUID;

    public PedestalDestroyedEvent(int UUID) { pedestalUUID = UUID; }

    public override string ToString()
    {
        return "Pedestal Destroyed Event Sent: Pedestal " + pedestalUUID + " died.";
    }
}

/// <summary>
/// The pedestal repaired event is sent when a pedestal is repaired. <br/>
/// The pedestal's UUID must be specified in the constructor.
/// </summary>
class PedestalRepairedEvent
{
    public int pedestalUUID;

    public PedestalRepairedEvent(int UUID) { pedestalUUID = UUID; }

    public override string ToString()
    {
        return "Pedestal Repaired Event Sent: Pedestal " + pedestalUUID + " repaired.";
    }
}
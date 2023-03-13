using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The GameStart event is to be broadcast when the game is started.
/// </summary>
class GameStartEvent {
    public override string ToString() {
        return "Game Start Event Sent";
    }
}

/// <summary>
/// The GameLoss event is broadcast when the game is lost.
/// </summary>
class GameLossEvent {
    public override string ToString() {
        return "Game Loss Event Sent";
    }
}

/// <summary>
/// The GameWin event is broadcast when the game is won.
/// </summary>
class GameWinEvent {
    public override string ToString() {
        return "Game Win Event Sent";
    }
}

/// <summary>
/// The GamePause event is broadcast when the game is paused.
/// </summary>
class GamePauseEvent {
    public override string ToString() {
        return "Game Pause Event Sent";
    }
}

/// <summary>
/// The GamePlay event is broadcast when the game is unpaused.
/// </summary>
class GamePlayEvent {
    public override string ToString() {
        return "Game Play Event Sent";
    }
}

/// <summary>
/// The NightBegin event can be broadcast to begin the night. Valid will be set true iff <br/>
/// the night is not already in progress.<bf/>
/// NOTE: Check valid after receiving a NightBegin event or risk processing an invalid event.
/// </summary>
class NightBeginEvent {
    public bool valid = false;

    public NightBeginEvent() {
        if (!GameControl.IsNight) valid = true;
    }

    public override string ToString() {
        return (valid ? "V" : "Inv") + "alid Night Begin Event Sent";
    }
}

/// <summary>
/// The NightEnd event can be broadcast to end the night. Valid will be set true iff <br/>
/// the night is already in progress.<bf/>
/// NOTE: Check valid after receiving a NightEnd event or risk processing an invalid event.
/// </summary>
class NightEndEvent {
    public bool valid = false;

    public NightEndEvent() {
        if (GameControl.IsNight) valid = true;
    }

    public override string ToString() {
        return (valid ? "V" : "Inv") + "alid Night End Event Sent";
    }
}

/// <summary>
/// The WaveBeganEvent is published after a new wave is released, and should only be published
/// by the GameControl class. There is no currently supported way to force a wave to start.
/// </summary>
class WaveBeganEvent {
    public override string ToString() {
        return "Wave Began Event Sent";
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The GameStart event is to be broadcast when the game is started.
/// </summary>
class GameStartEvent
{
    public override string ToString()
    {
        return "Game Start Event Sent";
    }
}

/// <summary>
/// The GameLoss event is broadcast when the game is lost.
/// </summary>
class GameLossEvent
{
    public override string ToString()
    {
        return "Game Loss Event Sent";
    }
}

/// <summary>
/// The GameWin event is broadcast when the game is won.
/// </summary>
class GameWinEvent
{
    public override string ToString()
    {
        return "Game Win Event Sent";
    }
}

/// <summary>
/// The GamePause event is broadcast when the game is paused.
/// </summary>
class GamePauseEvent
{
    public override string ToString()
    {
        return "Game Pause Event Sent";
    }
}

/// <summary>
/// The GamePlay event is broadcast when the game is win.
/// </summary>
class GamePlayEvent
{
    public override string ToString()
    {
        return "Game Play Event Sent";
    }
}

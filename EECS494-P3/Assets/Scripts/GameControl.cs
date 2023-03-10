using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class GameControl
{
    private static bool gameActive = false;
    /// <summary>
    /// Tracks whether the game is active. The game should be active after the <br/>
    /// game has been started and before the game has been won or lost.
    /// </summary>
    public static bool GameActive {
        get { return gameActive; }
        set { Debug.LogError("GameControl.GameActive is read only"); }
    }

    private static bool gamePaused = false;
    /// <summary>
    /// Tracks whether the game is paused or not.
    /// </summary>
    public static bool GamePaused {
        get { return gamePaused; }
        set { Debug.LogError("GameControl.GamePaused is read only");}
    }

    /// <summary>
    /// Starts the game via the event bus.
    /// </summary>
    public static void StartGame()
    {
        EventBus.Publish(new GameStartEvent());
    }

    static Subscription<GameStartEvent> startSub = EventBus.Subscribe<GameStartEvent>(Start);
    public static void Start(GameStartEvent e)
    {
        gameActive = true;
    }



    /// <summary>
    /// Ends the game with a loss via the event bus.
    /// </summary>
    public static void LoseGame()
    {
        EventBus.Publish(new GameLossEvent());
    }

    static Subscription<GameLossEvent> lossSub = EventBus.Subscribe<GameLossEvent>(Lose);
    public static void Lose(GameLossEvent e)
    {
        gameActive = false;
    }



    /// <summary>
    /// Ends the game with a win via the event bus.
    /// </summary>
    public static void WinGame()
    {
        EventBus.Publish(new GameWinEvent());
    }

    static Subscription<GameWinEvent> winSub = EventBus.Subscribe<GameWinEvent>(Win);
    private static void Win(GameWinEvent e)
    {
        gameActive = false;
    }



    /// <summary>
    /// Pauses the game via the event bus.
    /// </summary>
    public static void PauseGame() 
    {
        EventBus.Publish(new GamePauseEvent());
    }
    
    static Subscription<GamePauseEvent> pauseSub = EventBus.Subscribe<GamePauseEvent>(Pause);
    private static void Pause(GamePauseEvent e)
    {
        gamePaused = true;
    }



    /// <summary>
    /// Unpauses the game via the event bus.
    /// </summary>
    public static void PlayGame()
    {
        EventBus.Publish(new GamePlayEvent());
    }

    static Subscription<GamePlayEvent> playSub = EventBus.Subscribe<GamePlayEvent>(Play);
    private static void Play(GamePlayEvent e)
    {
        gamePaused = false;
    }
}

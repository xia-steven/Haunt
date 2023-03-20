using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This file contains (mostly) static code pertaining to public attributes of the GameControl class.
 * For all private methods, which control much of the game, see GameControl.cs
 */
partial class GameControl : MonoBehaviour {
    /* ------ Public Properties ------ */

    /// <summary>
    /// Tracks whether the game is active. The game should be active after the <br/>
    /// game has been started and before the game has been won or lost.
    /// </summary>
    public static bool GameActive {
        get { return instance.gameActive; }
        private set { }
    }

    /// <summary>
    /// Tracks whether the game is paused or not.
    /// </summary>
    public static bool GamePaused {
        get { return instance.gamePaused; }
        private set { }
    }

    /// <summary>
    /// Tracks whether it is night or day.
    /// </summary>
    public static bool IsNight {
        get { return isNight; }
        private set { }
    }

    /// <summary>
    /// Tracks when the night is ending and the exit teleporter is active
    /// </summary>
    public static bool NightEnding {
        get { return instance.nightEnding; }
        private set { }
    }

    /// <summary>
    /// Returns the current day. Note that the day increments after each night starts.
    /// </summary>
    public static int Day {
        get { return day; }
        private set { }
    }



    /// <summary>
    /// Tracks remaining time in the night. If it is not night, NightTimeRemaining is set to -1
    /// </summary>
    public static float NightTimeRemaining {
        get {
            if (isNight && !instance.nightEnding)
                return nightLength - (Time.time - instance.nightStartTime);
            else
                return -1;
        }
    }


    /* ------ Event Methods ------ */

    /// <summary>
    /// Starts the game via the event bus.
    /// </summary>
    public static void StartGame() {
        EventBus.Publish(new GameStartEvent());
    }

    static Subscription<GameStartEvent> startSub;

    private void _Start(GameStartEvent e) {
        instance.gameActive = true;

        //TODO: Make more robust
        PedestalAttacker.pedestalPositions = new Dictionary<int, Vector3>
            { { 1, new Vector3(10, 0, 0) }, { 2, new Vector3(-10, 0, 0) }, { 3, new Vector3(0, 0, -9) } };
    }


    /// <summary>
    /// Ends the game with a loss via the event bus.
    /// </summary>
    public static void LoseGame() {
        EventBus.Publish(new GameLossEvent());
    }

    static Subscription<GameLossEvent> lossSub;

    private void _Lose(GameLossEvent e) {
        instance.gameActive = false;
    }


    /// <summary>
    /// Ends the game with a win via the event bus.
    /// </summary>
    public static void WinGame() {
        EventBus.Publish(new GameWinEvent());
    }

    static Subscription<GameWinEvent> winSub;

    private void _Win(GameWinEvent e) {
        instance.gameActive = false;
    }


    /// <summary>
    /// Pauses the game via the event bus.
    /// </summary>
    public static void PauseGame() {
        EventBus.Publish(new GamePauseEvent());
    }

    static Subscription<GamePauseEvent> pauseSub;

    private void _Pause(GamePauseEvent e) {
        instance.gamePaused = true;
    }


    /// <summary>
    /// Unpauses the game via the event bus.
    /// </summary>
    public static void PlayGame() {
        EventBus.Publish(new GamePlayEvent());
    }

    static Subscription<GamePlayEvent> playSub;

    private void _Play(GamePlayEvent e) {
        instance.gamePaused = false;
    }


    /// <summary>
    /// Starts the night via the event bus.
    /// </summary>
    public static void StartNight() {
        EventBus.Publish(new NightBeginEvent());
    }

    static Subscription<NightBeginEvent> nightStartSub;

    private void _NightStart(NightBeginEvent e) {
        if (!e.valid) return;

        ++day;
        nightStartTime = Time.time;

        isNight = true;
        instance.StartCoroutine(NightUpdate());
    }


    /// <summary>
    /// Ends the night via the event bus, causing the NightEnding state to be true.
    /// </summary>
    public static void EndNight() {
        EventBus.Publish(new NightEndEvent());
    }

    static Subscription<NightEndEvent> nightEndEvent;

    private void _NightEnd(NightEndEvent e) {
        if (!e.valid) return;

        instance.nightEnding = true;
        instance.StartCoroutine(NightEndingUpdate());
    }

    public void ResetGame()
    {
        day = 0;

        gameActive = false;
        gamePaused = false;
        isNight = false;
        nightEnding = false;
    }
}
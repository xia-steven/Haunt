using Events;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * This file contains (mostly) static code pertaining to public attributes of the GameControl class.
 * For all private methods, which control much of the game, see GameControl.cs
 */
namespace Game_Control {
    internal partial class GameControl : MonoBehaviour {
        /* ------ Public Properties ------ */

        /// <summary>
        /// Tracks whether the game is active. The game should be active after the <br/>
        /// game has been started and before the game has been won or lost.
        /// </summary>
        public static bool GameActive {
            get => instance.gameActive;
            private set { }
        }

        /// <summary>
        /// Tracks whether the game is paused or not.
        /// </summary>
        public static bool GamePaused {
            get => instance.gamePaused;
            private set { }
        }

        /// <summary>
        /// Tracks whether it is night or day.
        /// </summary>
        public static bool IsNight {
            get => isNight;
            private set { }
        }

        /// <summary>
        /// Tracks when the night is ending and the exit teleporter is active
        /// </summary>
        public static bool NightEnding {
            get => instance.nightEnding;
            private set { }
        }

        /// <summary>
        /// Returns the current day. Note that the day increments after each night starts.
        /// </summary>
        public static int Day { get; set; }


        /// <summary>
        /// Tracks remaining time in the night. If it is not night, NightTimeRemaining is set to -1
        /// </summary>
        public static float NightTimeRemaining {
            get {
                return Day switch {
                    0 => 15 - (Time.time - instance.nightStartTime),
                    _ => isNight switch {
                        true when !instance.nightEnding => instance.data.nightLength -
                                                           (Time.time - instance.nightStartTime),
                        _ => -1
                    }
                };
            }
        }


        /* ------ Event Methods ------ */

        /// <summary>
        /// Starts the game via the event bus.
        /// </summary>
        public static void StartGame() {
            EventBus.Publish(new GameStartEvent());
        }

        private static Subscription<GameStartEvent> startSub;

        private static void _Start(GameStartEvent e) {
            instance.gameActive = true;
        }


        /// <summary>
        /// Ends the game with a loss via the event bus.
        /// </summary>
        public static void LoseGame() {
            EventBus.Publish(new GameLossEvent(IsPlayer.instance.LastDamaged()));
        }

        private static Subscription<GameLossEvent> lossSub;

        private static void _Lose(GameLossEvent e) {
            instance.gameActive = false;
        }


        /// <summary>
        /// Ends the game with a win via the event bus.
        /// </summary>
        public static void WinGame() {
            EventBus.Publish(new GameWinEvent());
        }

        private static Subscription<GameWinEvent> winSub;

        private static void _Win(GameWinEvent e) {
            instance.gameActive = false;
        }


        /// <summary>
        /// Pauses the game via the event bus.
        /// </summary>
        public static void PauseGame() {
            EventBus.Publish(new GamePauseEvent());
        }

        private static Subscription<GamePauseEvent> pauseSub;

        private static void _Pause(GamePauseEvent e) {
            instance.gamePaused = true;
        }


        /// <summary>
        /// Unpauses the game via the event bus.
        /// </summary>
        public static void PlayGame() {
            EventBus.Publish(new GamePlayEvent());
        }

        private static Subscription<GamePlayEvent> playSub;

        private void _Play(GamePlayEvent e) {
            instance.gamePaused = false;
        }


        /// <summary>
        /// Starts the night via the event bus.
        /// </summary>
        public static void StartNight() {
            EventBus.Publish(new NightBeginEvent());
        }

        private static Subscription<NightBeginEvent> nightStartSub;

        private void _NightStart(NightBeginEvent e) {
            switch (e.valid) {
                case false:
                    return;
            }

            if (SceneManager.GetActiveScene().name == "GameScene") ++Day;
            nightStartTime = Time.time;

            Debug.Log("Starting Night");

            isNight = true;
            instance.StartCoroutine(NightUpdate());
        }


        /// <summary>
        /// Ends the night via the event bus, causing the NightEnding state to be true.
        /// </summary>
        public static void EndNight() {
            EventBus.Publish(new NightEndEvent());
        }

        private static Subscription<NightEndEvent> nightEndEvent;

        private void _NightEnd(NightEndEvent e) {
            switch (e.valid) {
                case false:
                    return;
                default:
                    instance.nightEnding = true;
                    instance.StartCoroutine(NightEndingUpdate());
                    break;
            }
        }

        public static void ResetGame(string sceneName) {
            // Day 0 is the tutorial night, start 1 below that
            Day = -1;

            instance.gameActive = false;
            instance.gamePaused = false;
            isNight = false;
            instance.nightEnding = false;

            Time.timeScale = 1;
            AudioListener.pause = false;

            PlayerModifiers.resetModifiers();

            Destroy(IsPlayer.instance.gameObject);

            SceneManager.LoadScene(sceneName);
        }
    }
}
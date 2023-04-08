using System.Collections.Generic;
using UnityEngine;

namespace Events {
    /// <summary>
    /// The GameStart event is to be broadcast when the game is started.
    /// </summary>
    internal class GameStartEvent {
        public override string ToString() {
            return "Game Start Event Sent";
        }
    }

    /// <summary>
    /// The GameLoss event is broadcast when the game is lost.
    /// </summary>
    internal class GameLossEvent {
        public readonly DeathCauses cause;

        public bool finishedDeathAnimation = false;

        // Only set if death cause is an enemy
        public Vector3 enemyPos = Vector3.zero;

        public override string ToString() {
            return "Game Loss Event Sent";
        }

        public GameLossEvent(DeathCauses _cause) {
            cause = _cause;
        }
    }

    public enum DeathCauses {
        Pit,
        Enemy,
        Pedestal
    }

    /// <summary>
    /// The GameWin event is broadcast when the game is won.
    /// </summary>
    internal class GameWinEvent {
        public override string ToString() {
            return "Game Win Event Sent";
        }
    }

    /// <summary>
    /// The GamePause event is broadcast when the game is paused.
    /// </summary>
    internal class GamePauseEvent {
        public override string ToString() {
            return "Game Pause Event Sent";
        }
    }

    /// <summary>
    /// The GamePlay event is broadcast when the game is unpaused.
    /// </summary>
    internal class GamePlayEvent {
        public override string ToString() {
            return "Game Play Event Sent";
        }
    }

    /// <summary>
    /// The NightBegin event can be broadcast to begin the night. Valid will be set true iff <br/>
    /// the night is not already in progress.<bf/>
    /// NOTE: Check valid after receiving a NightBegin event or risk processing an invalid event.
    /// </summary>
    internal class NightBeginEvent {
        public readonly bool valid;

        public NightBeginEvent() {
            valid = Game_Control.GameControl.IsNight switch {
                false => true,
                _ => valid
            };
        }

        public override string ToString() {
            return (valid ? "V" : "Inv") + "alid Night Begin Event Sent";
        }
    }

    /// <summary>
    /// The NightEnd event can be broadcast to end the night and cause the NightEnding state. <br/>
    /// Valid will be set true iff the night is already in progress.<br/>
    /// NOTE: Check valid after receiving a NightEnd event or risk processing an invalid event.
    /// </summary>
    internal class NightEndEvent {
        public readonly bool valid;

        public NightEndEvent() {
            valid = Game_Control.GameControl.IsNight switch {
                true => true,
                _ => valid
            };
        }

        public override string ToString() {
            return (valid ? "V" : "Inv") + "alid Night End Event Sent";
        }
    }

    /// <summary>
    /// The WaveBeganEvent is published after a new wave is released, and should only be published
    /// by the GameControl class. There is no currently supported way to force a wave to start.
    /// </summary>
    internal class WaveBeganEvent {
        public override string ToString() {
            return "Wave Began Event Sent";
        }
    }

    /// <summary>
    /// The ToastRequest event is sent whenever a script wants the player to know something.
    /// The sender can specify if the message must remain for a keycode to be sent,
    /// what color the message should be, as well as the message to send.
    /// </summary>
    public class ToastRequestEvent {
        private readonly string message;
        public Color color;
        public bool waitForKey;
        public KeyCode keyToWaitFor;
        private readonly Color defaultColor = new Color32(255, 255, 255, 255);

        public ToastRequestEvent(string s, bool wfk = false, KeyCode key = KeyCode.None) {
            message = s;
            color = defaultColor;
            waitForKey = wfk;
            keyToWaitFor = key;
        }

        public ToastRequestEvent(Color c, string s, bool wfk = false, KeyCode key = KeyCode.None) {
            message = s;
            color = c;
            waitForKey = wfk;
            keyToWaitFor = key;
        }

        public override string ToString() {
            return "Toast message sent with contents: " + message;
        }
    }

    /// <summary>
    /// The MessageEvent is sent whenever there is a message to display to the player from a NPC.
    /// </summary>
    public class MessageEvent {
        public readonly List<string> messages;
        public readonly KeyCode keyToWaitFor;
        public readonly int senderInstanceID;
        public bool unpauseBeforeFade;
        public readonly bool pauseTime;

        public MessageEvent(List<string> messages_in, int senderInstanceID_in, bool pauseTime_in,
            KeyCode keyToWaitFor_in = KeyCode.Mouse0,
            bool unpauseBeforeFade_in = false) {
            messages = messages_in;
            keyToWaitFor = keyToWaitFor_in;
            senderInstanceID = senderInstanceID_in;
            unpauseBeforeFade = unpauseBeforeFade_in;
            pauseTime = pauseTime_in;
        }
    }

    /// <summary>
    /// The MessageFinishedEvent is sent whenever there a message finished displaying to the player.
    /// </summary>
    public class MessageFinishedEvent {
        public readonly int senderInstanceID;

        public MessageFinishedEvent(int senderInstanceID_in) {
            senderInstanceID = senderInstanceID_in;
        }
    }
}
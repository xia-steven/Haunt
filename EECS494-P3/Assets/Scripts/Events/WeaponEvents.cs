namespace Events {
    /// <summary>
    /// The ScreenShakeEvent is to be broadcast whenever the player fires a weapon. <br/>
    /// The default amplitude of the shake is 0.1 unless specified otherwise by the constructor.
    /// </summary>
    internal class ScreenShakeEvent {
        public float amplitude;

        public ScreenShakeEvent(float amplitude_in = 0.1f) {
            amplitude = amplitude_in;
        }

        public override string ToString() {
            return "Screen Shake Event Requested with an amplitude of " + amplitude + ".";
        }
    }


    /// <summary>
    /// The ScreenShakeToggleEvent is to be broadcast whenever the player fires a weapon to constantly shake the screen. <br/>
    /// The player must request this event again to turn off the constant shake. <br/>
    /// The default amplitude of the shake is 0.1 unless specified otherwise by the constructor. <br/>
    /// The default shake frequency is 0.2 seconds unless specified otherwise by the constructor.
    /// </summary>
    internal class ScreenShakeToggleEvent {
        public float amplitude;
        public float shakeFrequency;

        public ScreenShakeToggleEvent(float amplitude_in = 0.1f, float freq_in = 0.2f) {
            amplitude = amplitude_in;
            shakeFrequency = freq_in;
        }

        public override string ToString() {
            return "Screen Shake Toggle Event Requested with an amplitude of " + amplitude + " and a frequency of " +
                   shakeFrequency + ".";
        }
    }
}
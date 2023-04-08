namespace Events {
    /// <summary>
    /// The pedestal destroyed event is sent when the player destroys a pedestal. <br/>
    /// The pedestal's UUID must be specified in the constructor.
    /// </summary>
    internal class PedestalDestroyedEvent {
        public readonly int pedestalUUID;

        public PedestalDestroyedEvent(int UUID) {
            pedestalUUID = UUID;
        }

        public override string ToString() {
            return "Pedestal Destroyed Event Sent: Pedestal " + pedestalUUID + " died.";
        }
    }

    /// <summary>
    /// The pedestal repaired event is sent when the enemies repair a pedestal. <br/>
    /// The pedestal's UUID must be specified in the constructor.
    /// </summary>
    internal class PedestalRepairedEvent {
        public readonly int pedestalUUID;

        public PedestalRepairedEvent(int UUID) {
            pedestalUUID = UUID;
        }

        public override string ToString() {
            return "Pedestal Repaired Event Sent: Pedestal " + pedestalUUID + " repaired.";
        }
    }


    /// <summary>
    /// The pedestal partial event is sent when the enemies start repairing a pedestal. <br/>
    /// The pedestal's UUID must be specified in the constructor.
    /// </summary>
    internal class PedestalPartialEvent {
        public readonly int pedestalUUID;
        public readonly bool turnOn;

        public PedestalPartialEvent(int UUID, bool turnOn_in) {
            pedestalUUID = UUID;
            turnOn = turnOn_in;
        }

        public override string ToString() {
            return "Pedestal Partial Event Sent: Pedestal " + pedestalUUID + " repaired.";
        }
    }
}
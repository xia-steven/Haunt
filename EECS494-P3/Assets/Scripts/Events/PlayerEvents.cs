using UnityEngine;

namespace Events {
    /// <summary>
    /// The PlayerDamaged event is to be broadcast whenever the player takes damage. <br/>
    /// The default damage amount is 1 unless otherwise specified in the constructor.
    /// </summary>
    internal class PlayerDamagedEvent {
        private int damageTaken;

        public PlayerDamagedEvent(int amount = 1) {
            damageTaken = amount;
        }

        public override string ToString() {
            return "Player Damage Event Sent: Took " + damageTaken + " damage.";
        }
    }

    /// <summary>
    /// The HealthUIUpdate is used by anything that will change the state of the health bar. <br/>
    /// Mostly published by functions in PlayerHasHealth, as that's where the relevant health data resides.
    /// </summary>
    internal class HealthUIUpdate {
        public int updated_health;
        public int updated_locked_health;
        public int updated_shield_health;

        public HealthUIUpdate(int health, int locked_health, int shield_health) {
            updated_health = health;
            updated_locked_health = locked_health;
            updated_shield_health = shield_health;
        }

        public override string ToString() {
            return "Health UI State: Health: " + updated_health + ", Locked: " + updated_locked_health + ", Shields: " +
                   updated_shield_health + ".";
        }
    }

    /// <summary>
    /// The ReduceMaxHealth event is to be broadcast whenever the player's max health is <br/>
    /// reduced. The default reduce amount is 2 unless otherwise specified in the constructor.
    /// </summary>
    internal class ReduceMaxHealthEvent {
        public int reduceAmount;

        public ReduceMaxHealthEvent(int amount = 2) {
            reduceAmount = amount;
        }

        public override string ToString() {
            return "Player Max Health Reduce Event Sent: Lost " + reduceAmount + " Max.";
            ;
        }
    }

    /// <summary>
    /// The IncreaseMaxHealth event is to be broadcast whenever the player's max health is <br/>
    /// increased. The default increase amount is 2 unless otherwise specified in the constructor.
    /// </summary>
    internal class IncreaseMaxHealthEvent {
        public int increaseAmount;

        public IncreaseMaxHealthEvent(int amount = 2) {
            increaseAmount = amount;
        }

        public override string ToString() {
            return "Player Max Health Increase Event Sent: Gained " + increaseAmount + " Max.";
            ;
        }
    }

    /// <summary>
    /// The PlayerShootEvent is to be broadcast whenever the player shoots.
    /// </summary>
    internal class PlayerShootEvent {
        public override string ToString() {
            return "Player Shoot Event Sent";
        }
    }

    /// <summary>
    /// The PlayerMeleeEvent is to be broadcast whenever the player melee attacks.
    /// </summary>
    internal class PlayerMeleeEvent {
        public override string ToString() {
            return "Player Melee Event Sent";
        }
    }

    /// <summary>
    /// The PlayerDodgeEvent is to be broadcast whenever the player dodges.
    /// </summary>
    internal class PlayerDodgeEvent {
        // Set to true for starting a dodge and false for ending a dodge
        public bool start;
        public Vector3 direction;

        public PlayerDodgeEvent(bool _start, Vector3 _direction) {
            start = _start;
            direction = _direction;
        }

        public override string ToString() {
            return "Player Dodge Event Sent";
        }
    }

    /// <summary>
    /// The PlayerInteractEvent is to be broadcast whenever the player attempts to interact.
    /// </summary>
    internal class PlayerInteractEvent {
        public override string ToString() {
            return "Player Interact Event Sent";
        }
    }

    /// <summary>
    /// The CoinCollectedEvent is to be broadcast whenever the player collects a coin.
    /// </summary>
    internal class CoinEvent {
        public int coinValue;

        public CoinEvent(int value = 1) {
            coinValue = value;
        }
    }

    /// <summary>
    /// This event is sent whenever a script wants to disable player controls
    /// </summary>
    public class DisablePlayerEvent {
        public override string ToString() {
            return "Disable Player Event sent";
        }
    }

    /// <summary>
    /// This event is sent whenever a script wants to enable player controls
    /// </summary>
    public class EnablePlayerEvent {
        public override string ToString() {
            return "Enable Player Event sent";
        }
    }

    /// <summary>
    /// This event is sent whenever the player attempts to interact by using the interact key
    /// </summary>
    public class TryInteractEvent {
        public override string ToString() {
            return "Interact Attempt Event sent";
        }
    }

    /// <summary>
    /// Sent from purchasable object to the shopController and/or the other weapon in the shop
    /// </summary>
    public class WeaponPurchasedEvent {
        public GameObject weapon;

        public WeaponPurchasedEvent(GameObject _purchase) {
            weapon = _purchase;
        }
    }

    /// <summary>
    /// This event is sent to reset the player's inventory back to just a base pistol
    /// </summary>
    internal class ResetInventoryEvent {
        public override string ToString() {
            return "Reset Inventory Event sent";
        }
    }

    /// <summary>
    /// This event is sent to reload all weapons in player's inventory
    /// </summary>
    public class ReloadAllEvent {
        public override string ToString() {
            return "Reload All Event sent";
        }
    }
}
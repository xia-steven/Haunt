using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The PlayerPositionEvent event is to be broadcast where the player is.
/// </summary>
class PlayerPositionEvent {
    public Vector3 position;

    public PlayerPositionEvent(Vector3 pos) {
        position = pos;
    }
}

/// <summary>
/// The PlayerDamaged event is to be broadcast whenever the player takes damage. <br/>
/// The default damage amount is 1 unless otherwise specified in the constructor.
/// </summary>
class PlayerDamagedEvent {
    public int damageTaken;

    public PlayerDamagedEvent(int amount = 1) {
        damageTaken = amount;
    }

    public override string ToString() {
        return "Player Damage Event Sent: Took " + damageTaken + " damage.";
    }
}

/// <summary>
/// The PlayerHealed event is to be broadcast whenever the player heals damage. <br/>
/// The default damage amount is 1 unless otherwise specified in the constructor.
/// </summary>
class PlayerHealedEvent {
    public int damageHealed;

    public PlayerHealedEvent(int amount = 1) {
        damageHealed = amount;
    }

    public override string ToString() {
        return "Player Damage Event Sent: Healed " + damageHealed + " damage.";
    }
}

/// <summary>
/// The ReduceMaxHealth event is to be broadcast whenever the player's max health is <br/>
/// reduced. The default reduce amount is 2 unless otherwise specified in the constructor.
/// </summary>
class ReduceMaxHealthEvent {
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
class IncreaseMaxHealthEvent {
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
class PlayerShootEvent {
    public override string ToString() {
        return "Player Shoot Event Sent";
    }
}

/// <summary>
/// The PlayerMeleeEvent is to be broadcast whenever the player melee attacks.
/// </summary>
class PlayerMeleeEvent {
    public override string ToString() {
        return "Player Melee Event Sent";
    }
}

/// <summary>
/// The PlayerDodgeEvent is to be broadcast whenever the player dodges.
/// </summary>
class PlayerDodgeEvent {
    public override string ToString() {
        return "Player Dodge Event Sent";
    }
}

/// <summary>
/// The PlayerInteractEvent is to be broadcast whenever the player attempts to interact.
/// </summary>
class PlayerInteractEvent {
    public override string ToString() {
        return "Player Interact Event Sent";
    }
}

/// <summary>
/// The CoinCollectedEvent is to be broadcast whenever the player collects a coin.
/// </summary>
class CoinCollectedEvent
{
    public int coinValue;
    
    public CoinCollectedEvent(int value = 1) {
        coinValue = value;
    }
    
}

/// <summary>
/// This event is sent whenever a script wants to disable player controls
/// </summary>
class DisablePlayerEvent
{
    public override string ToString()
    {
        return "Disable Player Event sent";
    }
}

/// <summary>
/// This event is sent whenever a script wants to enable player controls
/// </summary>
class EnablePlayerEvent
{
    public override string ToString()
    {
        return "Enable Player Event sent";
    }
}
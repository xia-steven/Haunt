using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IsPedestal))]
public class HasPedestalHealth : HasHealth {
    [SerializeField] int PedestalMaxHealth = 5;

    IsPedestal pedestal;

    private void Start() {
        pedestal = GetComponent<IsPedestal>();
        health = 0;
    }

    
    public override bool AlterHealth(int healthDelta) {
        // NOTE: health delta is treated backwards of standard health components
        // due to the player destroying pedestals and enemies 
        healthDelta = -healthDelta;
        if (health == 0 && healthDelta < 0) return false;

        if (health + healthDelta >= PedestalMaxHealth) {
            if (pedestal.IsDestroyedByPlayer())
            {
                // Increase pedestal max health for the player to destroy
                PedestalMaxHealth++;
                health = PedestalMaxHealth;
                // Let other systems know the enemies repaired a pedestal
                pedestal.PedestalRepaired();
            }
            else
            {
                health = PedestalMaxHealth;

            }
            pedestal.updateVisuals(health, PedestalMaxHealth);
            return false;
        }

        health += healthDelta;
        pedestal.updateVisuals(health, PedestalMaxHealth);

        if (health <= 0) {
            if (!pedestal.IsDestroyedByPlayer()) {
                pedestal.PedestalDied();
            }

            health = 0;
        }


        return true;
    }


}
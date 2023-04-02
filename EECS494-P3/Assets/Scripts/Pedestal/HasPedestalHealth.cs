using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IsPedestal))]
public class HasPedestalHealth : HasHealth {
    public static int PedestalMaxHealth = 5;
    [SerializeField] AudioClip damageSound;
    [SerializeField] AudioClip breakSound;
    [SerializeField] AudioClip healSound;
    [SerializeField] AudioClip restoreSound;

    IsPedestal pedestal;

    private void Start() {
        pedestal = GetComponent<IsPedestal>();
        health = 0;
    }


    public override void AlterHealth(int healthDelta) {
        // NOTE: health delta is treated backwards of standard health components
        // due to the player destroying pedestals and enemies 
        healthDelta = -healthDelta;
        if (health == 0 && healthDelta < 0 || health == PedestalMaxHealth && healthDelta > 0 ||
            healthDelta == 0) return;

        health = Mathf.Clamp(health + healthDelta, 0, PedestalMaxHealth);

        if (health <= 0 && !pedestal.IsDestroyedByPlayer()) {
            pedestal.PedestalDied();
            AudioSource.PlayClipAtPoint(breakSound, transform.position);
        }
        else if (health == PedestalMaxHealth && pedestal.IsDestroyedByPlayer()) {
            PedestalMaxHealth++;
            health = PedestalMaxHealth;
            // Let other systems know the enemies repaired a pedestal
            pedestal.PedestalRepaired();
            AudioSource.PlayClipAtPoint(restoreSound, transform.position);
        }
        else if (healthDelta > 0) {
            AudioSource.PlayClipAtPoint(healSound, transform.position);
        }
        else if (healthDelta < 0) {
            AudioSource.PlayClipAtPoint(damageSound, transform.position);
        }

        pedestal.updateVisuals(health, PedestalMaxHealth);
    }
}
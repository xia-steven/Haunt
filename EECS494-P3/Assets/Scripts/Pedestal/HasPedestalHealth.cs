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

    
    private Animator anim;
    private SpriteRenderer sr;

    IsPedestal pedestal;

    private void Start() {
        pedestal = GetComponent<IsPedestal>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        health = 0;
    }


    public override void AlterHealth(float healthDelta) {
        // NOTE: health delta is treated backwards of standard health components
        // due to the player destroying pedestals and enemies 
        healthDelta = -healthDelta;
        Debug.Log("start"+ health/PedestalMaxHealth);
        if (health == 0 && healthDelta < 0 || health == PedestalMaxHealth && healthDelta > 0 ||
            healthDelta == 0)
        {
            if (healthDelta < 0)
            {
                anim.SetTrigger("TookDamage");
            }
            
            anim.SetFloat("Health",(float)health / (float)PedestalMaxHealth);
            return;
        }

        health = Mathf.Clamp(health + healthDelta, 0, PedestalMaxHealth);

        if (health <= 0 && !pedestal.IsDestroyedByPlayer()) {
            pedestal.PedestalDied();
            AudioSource.PlayClipAtPoint(breakSound, transform.position);
        }
        else if (health == PedestalMaxHealth && pedestal.IsDestroyedByPlayer()) {
            health = PedestalMaxHealth;
            // Let other systems know the enemies repaired a pedestal
            pedestal.PedestalRepaired();
            AudioSource.PlayClipAtPoint(restoreSound, transform.position);
            EventBus.Publish<PedestalPartialEvent>(new PedestalPartialEvent(pedestal.getUUID(), false));
        }
        else if (healthDelta > 0)
        {
            AudioSource.PlayClipAtPoint(healSound, transform.position);
            EventBus.Publish<PedestalPartialEvent>(new PedestalPartialEvent(pedestal.getUUID(), true));
        }
        else if (healthDelta < 0)
        {
            anim.SetTrigger("TookDamage");
            AudioSource.PlayClipAtPoint(damageSound, transform.position);
        }

        if(health == 0)
        {
            EventBus.Publish<PedestalPartialEvent>(new PedestalPartialEvent(pedestal.getUUID(), false));
        }

        // Health should be a value between 0 and 1, ie pct of full health
        anim.SetFloat("Health",(float)health / (float)PedestalMaxHealth);
    }
}
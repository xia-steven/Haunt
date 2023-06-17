using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasDashDamageUpgrade : MonoBehaviour {
    public float cooldown;
    public float dmgMod;

    private float lastDodge;

    private bool coolingDown = false;
    private bool increased = false;
    private Animator anim;

    Subscription<PlayerDodgeEvent> dodgeSub;
    Subscription<FireEvent> fireSub;

    // Start is called before the first frame update
    void Start()
    {
        // Always allow for dodge instantly
        lastDodge = 0;
        anim = GetComponentInChildren<Animator>();
        
        EventBus.Subscribe<PlayerDodgeEvent>(_OnDash);
        EventBus.Subscribe<FireEvent>(_OnFire);
    }

    void _OnDash(PlayerDodgeEvent e) {
        // Check if dash cooldown has passed since last activation && not currently buffed
        if ((Time.time - lastDodge) >= cooldown && !increased) {
            lastDodge = Time.time;

            // Increase damage
            PlayerModifiers.damage *= dmgMod;
            increased = true;
                
            // Change visual to show increase
            anim.SetFloat("damage", PlayerModifiers.damage);
        }
    }

    void _OnFire(FireEvent e) {
        if (increased)
        {
            StartCoroutine(DecreaseAfterTick());
        }
    }

    IEnumerator DecreaseAfterTick() {
        yield return null;

        // Reset damage
        PlayerModifiers.damage /= dmgMod;
        increased = false;

        // Change visual back to normal
        anim.SetFloat("damage", PlayerModifiers.damage);
    }

    private void OnDestroy()
    {
        // Reset everything on destroy if increased (don't want active between scenes)
        if (increased)
        {
            // Reset damage
            PlayerModifiers.damage /= dmgMod;
            increased = false;

            // Change visual back to normal
            anim.SetFloat("damage", PlayerModifiers.damage);
        }
    }
}
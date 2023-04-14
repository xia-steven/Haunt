using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasDashDamageUpgrade : MonoBehaviour {
    public float cooldown;
    public float dmgMod;

    float dodgeTime;

    bool coolingDown = false;
    bool increased = false;
    private Animator anim;

    Subscription<PlayerDodgeEvent> dodgeSub;
    Subscription<FireEvent> fireSub;

    // Start is called before the first frame update
    void Start()
    {

        anim = GetComponentInChildren<Animator>();
        
        EventBus.Subscribe<PlayerDodgeEvent>(_OnDash);
        EventBus.Subscribe<FireEvent>(_OnFire);
    }

    void _OnDash(PlayerDodgeEvent e) {
        if (!coolingDown) {
            StartCoroutine(Cooldown());
            if (!increased) {
                PlayerModifiers.damage *= dmgMod;
                increased = true;
                
                // TODO: add visual of increase here
                anim.SetFloat("damage", PlayerModifiers.damage);
            }
        }
    }

    void _OnFire(FireEvent e) {
        StartCoroutine(DecreaseAfterTick());
    }

    IEnumerator DecreaseAfterTick() {
        yield return null;
        if (increased) {
            PlayerModifiers.damage /= dmgMod;
            increased = false;
            // TODO: remove visual of increase here
            anim.SetFloat("damage", PlayerModifiers.damage);
        }
    }


    IEnumerator Cooldown() {
        coolingDown = true;
        yield return new WaitForSeconds(cooldown);
        coolingDown = false;

        if (increased) {
            PlayerModifiers.damage /= dmgMod;
            increased = false;
            // TODO: remove visual of increase here
            anim.SetFloat("damage", PlayerModifiers.damage);
        }
    }
}
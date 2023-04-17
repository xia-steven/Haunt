using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasRageUpgrade : MonoBehaviour {
    public float duration;
    public float moveMod;
    public float dmgMod;

    float hitTime;

    bool active;
    bool statsChanged;
    private Animator anim;

    Subscription<PlayerDamagedEvent> dmgSub;

    // Start is called before the first frame update
    void Start()
    {
        hitTime = 0;
        active = false;
        statsChanged = false;
        anim = GetComponentInChildren<Animator>();
        
        EventBus.Subscribe<PlayerDamagedEvent>(_OnDamage);
    }

    // Update is called once per frame
    void Update() {
        if (active) {
            if (!statsChanged) {
                PlayerModifiers.damage *= dmgMod;
                PlayerModifiers.moveSpeed *= moveMod;
                statsChanged = true;
                //TODO: change player visuals here
                anim.SetFloat("damage", PlayerModifiers.damage);
            }

            if (Time.time - hitTime > duration) {
                active = false;
            }
        }

        if (!active && statsChanged) {
            PlayerModifiers.damage /= dmgMod;
            PlayerModifiers.moveSpeed /= moveMod;
            statsChanged = false;
            //TODO: change player visuals here
            anim.SetFloat("damage", PlayerModifiers.damage);
        }
    }

    void _OnDamage(PlayerDamagedEvent e) {
        active = true;
        hitTime = Time.time;
    }

    private void OnDestroy()
    {
        if (active)
        {
            active = false;

            // Reset damage mod on scene switch
            PlayerModifiers.damage /= dmgMod;
            PlayerModifiers.moveSpeed /= moveMod;
            statsChanged = false;
            //TODO: change player visuals here
            anim.SetFloat("damage", PlayerModifiers.damage);
        }
        hitTime = 0;
    }
}
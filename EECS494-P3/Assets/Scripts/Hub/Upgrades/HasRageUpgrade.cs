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

    Subscription<PlayerDamagedEvent> dmgSub;

    // Start is called before the first frame update
    void Start() {
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
        }
    }

    void _OnDamage(PlayerDamagedEvent e) {
        active = true;
        hitTime = Time.time;
    }
}
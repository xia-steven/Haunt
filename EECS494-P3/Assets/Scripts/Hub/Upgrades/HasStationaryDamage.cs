using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasStationaryDamage : MonoBehaviour {
    public float holdTime;
    public float dmgMod;

    Vector3 knownPos;
    float lastUpdate;

    bool holding = false;
    bool dmgIncreased = false;
    private Animator anim;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        StartCoroutine(PollForHold());
    }

    // Update is called once per frame
    void Update() {
        if (holding && !dmgIncreased) {
            PlayerModifiers.damage *= dmgMod;
            dmgIncreased = true;
            anim.SetFloat("damage", PlayerModifiers.damage);
        }
        else if (!holding && dmgIncreased) {
            PlayerModifiers.damage /= dmgMod;
            dmgIncreased = false;
            anim.SetFloat("damage", PlayerModifiers.damage);
        }
    }

    IEnumerator PollForHold() {
        lastUpdate = Time.time;
        knownPos = IsPlayer.instance.transform.position;

        while (true) {
            // if player has moved, reset count
            if (knownPos != IsPlayer.instance.transform.position) {
                lastUpdate = Time.time;
                knownPos = IsPlayer.instance.transform.position;
                holding = false;
            }

            // if held for long enough, set holding true
            if (Time.time - lastUpdate > holdTime) holding = true;

            yield return new WaitForSeconds(.1f);
        }
    }
}
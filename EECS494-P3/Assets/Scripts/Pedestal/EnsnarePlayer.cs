using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnsnarePlayer : MonoBehaviour {
    private LineRenderer rend;
    Transform playerShadow;
    bool flameLit = false;

    private Subscription<PedestalDestroyedEvent> ped_dest_event;
    private Subscription<PedestalRepairedEvent> ped_rep_event;

    // Start is called before the first frame update
    void Start() {
        EventBus.Subscribe<PedestalDestroyedEvent>(_OnPedestalDestroyed);
        EventBus.Subscribe<PedestalRepairedEvent>(_OnPedestalRepaired);


        rend = GetComponent<LineRenderer>();
        for (int i = 0; i < IsPlayer.instance.transform.childCount; i++) {
            Transform child = IsPlayer.instance.transform.GetChild(i);
            if (child.name == "Shadow") {
                playerShadow = child;
            }
        }

        // Initially have the line renderer disabled
        rend.enabled = false;
    }

    // Update is called once per frame
    void Update() {
        if (flameLit) {
            rend.SetPositions(new Vector3[] { transform.position, playerShadow.position });
        }
    }

    void _OnPedestalDestroyed(PedestalDestroyedEvent e) {
        if (e.pedestalUUID == GetComponent<IsPedestal>().getUUID()) {
            flameLit = false;
            rend.enabled = false;
        }
    }

    void _OnPedestalRepaired(PedestalRepairedEvent e) {
        Debug.Log(e.pedestalUUID + " == " + GetComponent<IsPedestal>().getUUID());
        if (e.pedestalUUID == GetComponent<IsPedestal>().getUUID()) {
            rend.enabled = true;
            StartCoroutine(GrowTowardPlayer());
        }
    }

    IEnumerator GrowTowardPlayer() {
        float t = 0f;
        while (t < 1f) {
            t += Time.deltaTime;
            rend.SetPositions(new Vector3[]
                { transform.position, Vector3.Lerp(transform.position, playerShadow.position, t) });
            yield return null;
        }

        flameLit = true;
    }

    private void OnDestroy() {
        EventBus.Unsubscribe<PedestalDestroyedEvent>(ped_dest_event);
        EventBus.Unsubscribe<PedestalRepairedEvent>(ped_rep_event);
    }
}
using System;
using System.Collections;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LowHealthVignette : MonoBehaviour {
    private Subscription<HealthUIUpdate> damageSub;
    private Image img;

    private void Start() {
        damageSub = EventBus.Subscribe<HealthUIUpdate>(HandleUpdate);
        img = GetComponent<Image>();
        img.enabled = IsPlayer.instance.GetHealth() < 2;
    }

    private void HandleUpdate(HealthUIUpdate e) {
        img.enabled = e.updated_health < 2 && e.updated_health > 0;
    }
}
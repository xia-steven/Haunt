using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HasPedestalHealth))]
[RequireComponent(typeof(ParticleSystem))]
public class IsPedestal : MonoBehaviour {
    [SerializeField] int UUID = -1;
    [SerializeField] Color repairedColor;
    [SerializeField] Color destroyedColor;
    [SerializeField] GameObject repairedVisual;
    [SerializeField] GameObject destroyedVisual;
    [SerializeField] GameObject floatingOrb;
    [SerializeField] GameObject healthBar;
    ParticleSystem particles;
    HasPedestalHealth pedestalHealth;

    Gradient colors;
    Renderer[] repairedRenders;
    Renderer[] destroyedRenders;
    Image healthBarImage;
    bool playerDestroyed = true;

    // Start is called before the first frame update
    void Start() {
        particles = GetComponent<ParticleSystem>();
        pedestalHealth = GetComponent<HasPedestalHealth>();
        repairedRenders = repairedVisual.GetComponentsInChildren<Renderer>();
        destroyedRenders = destroyedVisual.GetComponentsInChildren<Renderer>();
        healthBarImage = healthBar.GetComponent<Image>();

        repairedVisual.SetActive(false);
        floatingOrb.SetActive(false);
        destroyedVisual.SetActive(true);

        // Initialize gradient
        colors = new Gradient();
        GradientColorKey[] gck = new GradientColorKey[2];
        GradientAlphaKey[] gak = new GradientAlphaKey[2];
        gck[0].color = destroyedColor;
        gck[0].time = 0F;
        gck[1].color = repairedColor;
        gck[1].time = 1.0F;
        gak[0].alpha = 1.0F;
        gak[0].time = 0F;
        gak[1].alpha = 1.0F;
        gak[1].time = 1.0F;
        colors.SetKeys(gck, gak);
        // Set initial color and state to destroyed
        updateVisuals(0, 1);
        particles.Play();
    }

    // Update is called once per frame
    void Update() {
        DebugKeys();
    }

    public int getUUID() {
        return UUID;
    }

    public void PedestalDied()
    {
        Debug.Log("Pedestal destroyed by player :)");
        playerDestroyed = true;
        particles.Play();
        EventBus.Publish(new PedestalDestroyedEvent(UUID));
    }

    public void PedestalRepaired()
    {
        Debug.Log("Pedestal restored by enemies :(");
        playerDestroyed = false;
        particles.Stop();
        EventBus.Publish(new PedestalRepairedEvent(UUID));
        EventBus.Publish(new ToastRequestEvent(new Color32(255, 0, 0, 255), "Pedestal repaired by enemies"));
    }

    public bool IsDestroyedByPlayer() {
        return playerDestroyed; 
    }

    public void updateVisuals(int curr, int max) {
        float healthfraction = (float)curr / max;

        healthBarImage.fillAmount = healthfraction;

        // Update pedestal visual objects
        if (curr == 0)
        {
            // Destroyed
            floatingOrb.SetActive(false);
            repairedVisual.SetActive(false);
            destroyedVisual.SetActive(true);
        }
        else if (healthfraction == 0.5f || (float)curr / (float)(max + 1) == 0.5f)
        {
            // half Repaired
            floatingOrb.SetActive(false);
            repairedVisual.SetActive(true);
            destroyedVisual.SetActive(false);
        }
        else if (curr == max)
        {
            // fully repaired
            floatingOrb.SetActive(true);
            repairedVisual.SetActive(true);
            destroyedVisual.SetActive(false);
        }


        // If repaired visual is active
        if (repairedVisual.activeSelf)
        {
            for (int a = 0; a < repairedRenders.Length; ++a)
            {
                repairedRenders[a].material.color = colors.Evaluate(healthfraction);
            }
        }
        // Else if destroyed is active
        else if (destroyedVisual.activeSelf)
        {
            for (int a = 0; a < destroyedRenders.Length; ++a)
            {
                destroyedRenders[a].material.color = colors.Evaluate(healthfraction);
            }
        }
    }

    void DebugKeys() {
        if (Input.GetKeyDown(KeyCode.K) && UUID == 1) {
            pedestalHealth.AlterHealth(-1);
        }
        else if (Input.GetKeyDown(KeyCode.J) && UUID == 1) {
            pedestalHealth.AlterHealth(1);
        }
    }
}
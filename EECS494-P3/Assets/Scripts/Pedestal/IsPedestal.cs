using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HasPedestalHealth))]
[RequireComponent(typeof(ParticleSystem))]
public class IsPedestal : MonoBehaviour {
    [SerializeField] int UUID = -1;
    [SerializeField] Color repairedColor;
    [SerializeField] Color destroyedColor;
    ParticleSystem particles;
    HasPedestalHealth pedestalHealth;

    Gradient colors;
    Renderer[] childrenders;
    bool playerDestroyed = true;

    // Start is called before the first frame update
    void Start() {
        particles = GetComponent<ParticleSystem>();
        pedestalHealth = GetComponent<HasPedestalHealth>();
        childrenders = GetComponentsInChildren<Renderer>();

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
        updateColor(0, 1);
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

    public bool IsDestroyedByPlayer {
        get { return playerDestroyed; }
        set { }
    }

    public void updateColor(int curr, int max) {
        // Ignore the parent material
        for (int a = 1; a < childrenders.Length; ++a) {
            childrenders[a].material.color = colors.Evaluate((float)curr / (float)max);
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
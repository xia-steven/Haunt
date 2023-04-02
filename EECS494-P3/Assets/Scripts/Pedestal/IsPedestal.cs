using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HasPedestalHealth))]
public class IsPedestal : MonoBehaviour {
    [SerializeField] int UUID = -1;
    // [SerializeField] Color repairedColor;
    // [SerializeField] Color destroyedColor;
    // [SerializeField] GameObject repairedVisual;
    // [SerializeField] GameObject destroyedVisual;
    // [SerializeField] GameObject floatingOrb;
    // [SerializeField] GameObject healthBar;
    [SerializeField] bool startRepaired = false;
    HasPedestalHealth pedestalHealth;

    // Gradient colors;
    // Renderer[] repairedRenders;
    // Renderer[] destroyedRenders;
    // Image healthBarImage;
    bool playerDestroyed = true;

    // Start is called before the first frame update
    void Start() {
        pedestalHealth = GetComponent<HasPedestalHealth>();
        // repairedRenders = repairedVisual.GetComponentsInChildren<Renderer>();
        // destroyedRenders = destroyedVisual.GetComponentsInChildren<Renderer>();
        // healthBarImage = healthBar.GetComponent<Image>();

        // Initialize gradient
        // colors = new Gradient();
        // GradientColorKey[] gck = new GradientColorKey[2];
        // GradientAlphaKey[] gak = new GradientAlphaKey[2];
        // gck[0].color = destroyedColor;
        // gck[0].time = 0F;
        // gck[1].color = repairedColor;
        // gck[1].time = 1.0F;
        // gak[0].alpha = 1.0F;
        // gak[0].time = 0F;
        // gak[1].alpha = 1.0F;
        // gak[1].time = 1.0F;
        // colors.SetKeys(gck, gak);

        if (!startRepaired)
        {
            // repairedVisual.SetActive(false);
            // floatingOrb.SetActive(false);
            // destroyedVisual.SetActive(true);
            // Set initial color and state to destroyed
            //updateVisuals(0, 1);
        }
        else
        {
            // repairedVisual.SetActive(true);
            // floatingOrb.SetActive(true);
            // destroyedVisual.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update() {
        if(startRepaired)
        {
            // Manually repair the pedestal
            pedestalHealth.AlterHealth(-500);
            startRepaired = false;
        }

        DebugKeys();
    }

    public int getUUID() {
        return UUID;
    }

    public void PedestalDied()
    {
        Debug.Log("Pedestal destroyed by player :)");
        playerDestroyed = true;
        EventBus.Publish(new PedestalDestroyedEvent(UUID));
    }

    public void PedestalRepaired()
    {
        Debug.Log("Pedestal restored by enemies :(");
        playerDestroyed = false;
        EventBus.Publish(new PedestalRepairedEvent(UUID));
    }

    public bool IsDestroyedByPlayer() {
        return playerDestroyed; 
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
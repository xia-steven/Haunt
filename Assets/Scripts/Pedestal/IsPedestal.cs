using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HasPedestalHealth))]
public class IsPedestal : MonoBehaviour {
    [SerializeField] int UUID = -1;
    [SerializeField] bool startRepaired = false;
    HasPedestalHealth pedestalHealth;

    bool playerDestroyed = true;

    // Start is called before the first frame update
    void Start() {
        pedestalHealth = GetComponent<HasPedestalHealth>();
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
        EventBus.Publish(new PedestalDestroyedEvent(UUID, transform.position));
    }

    public void PedestalRepaired()
    {
        Debug.Log("Pedestal restored by enemies :(");
        playerDestroyed = false;
        EventBus.Publish(new PedestalRepairedEvent(UUID, transform.position));
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
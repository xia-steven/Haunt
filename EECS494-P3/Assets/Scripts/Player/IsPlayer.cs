using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HasHealth))]
public class IsPlayer : MonoBehaviour {
    public static IsPlayer instance;

    private HasHealth health;

    // Start is called before the first frame update
    void Awake() {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        health = GetComponent<HasHealth>();
        StartCoroutine(NaturalHealthRegen());
    }

    public int GetHealth() {
        return health.GetHealth();
    }

    public int GetMaxHealth() {
        return health.GetMaxHealth();
    }

    IEnumerator NaturalHealthRegen()
    {
        Debug.Log("got here");
        while(health.GetHealth() > 0)
        {
            Debug.Log("still here");
            EventBus.Publish(new PlayerDamagedEvent(-1));
            yield return new WaitForSeconds(5.0f);
        }
        Debug.Log("Died");
    }
}
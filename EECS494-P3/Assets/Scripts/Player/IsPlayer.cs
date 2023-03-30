using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerHasHealth))]
public class IsPlayer : MonoBehaviour {
    public static IsPlayer instance;

    private PlayerHasHealth health;

    // Start is called before the first frame update
    void Awake() {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        health = GetComponent<PlayerHasHealth>();
        //StartCoroutine(NaturalHealthRegen());
        // starting to phase this out of the game
    }

    public int GetHealth() {
        return health.GetHealth();
    }

    public int GetMaxHealth() {
        return health.GetMaxHealth();
    }

    IEnumerator NaturalHealthRegen()
    {
        // Let everything load up
        yield return new WaitForSeconds(1.0f);
        while(health.GetHealth() > 0)
        {
            health.AlterHealth(1);
            yield return new WaitForSeconds(5.0f);
        }
    }

    public static void SetPosition(Vector3 newPos)
    {
        instance.transform.position = newPos;
    }

    public void ResetHealth()
    {
        health.ResetHealth();
    }
}
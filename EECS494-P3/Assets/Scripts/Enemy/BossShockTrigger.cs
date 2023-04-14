using UnityEngine;

public class BossShockTrigger : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        // On trigger enter for laser and shockwave
        PlayerHasHealth playerHealth = other.GetComponent<PlayerHasHealth>();
        if (playerHealth != null)
        {
            Debug.Log("Player damaged from boss");
            playerHealth.AlterHealth(-1, DeathCauses.Enemy);
        }

    }
}

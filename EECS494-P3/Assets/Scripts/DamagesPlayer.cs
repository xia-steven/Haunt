using Events;
using Player;
using UnityEngine;

public class DamagesPlayer : MonoBehaviour {
    [SerializeField] private int damageAmount = -1;

    private void OnCollisionEnter(Collision collision) {
        if (!collision.gameObject.CompareTag("Player")) return;
        Debug.Log("OUCH!!");
        //alter before publishing in case a subscriber references the new health
        collision.gameObject.GetComponent<PlayerHasHealth>().AlterHealth(damageAmount, DeathCauses.Enemy);
    }
}

public class DamageEvent {
    public int damage_amt;

    public DamageEvent(int _damage_amt) {
        damage_amt = _damage_amt;
    }
}
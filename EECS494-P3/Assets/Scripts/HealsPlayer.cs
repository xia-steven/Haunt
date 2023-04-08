using System.Collections;
using Player;
using UnityEngine;

public class HealsPlayer : MonoBehaviour {
    [SerializeField] private int healAmount = 1;

    private void Start() {
        StartCoroutine(WaitAndDestroy());
    }

    private IEnumerator WaitAndDestroy() {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("PlayerPhysical")) return;
        IsPlayer.instance.gameObject.GetComponent<PlayerHasHealth>().AlterHealth(healAmount);
        Destroy(gameObject);
    }
}
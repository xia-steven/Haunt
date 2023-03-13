using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsWaveMediator : MonoBehaviour {
    public static IsWaveMediator instance;

    // Start is called before the first frame update
    void Awake() {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
    }

    public void Spawn(Dictionary<int, IsWaveMember>.ValueCollection members, float timeBetweenSpawns) {
        StartCoroutine(TimedSpawn(members, timeBetweenSpawns));
    }

    private IEnumerator TimedSpawn(Dictionary<int, IsWaveMember>.ValueCollection members, float timeBetweenSpawns) {
        foreach (var m in new List<IsWaveMember>(members)) {
            if (m == null) continue;
            m.gameObject.SetActive(true);
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    private void OnDestroy() {
        instance = null;
    }
}
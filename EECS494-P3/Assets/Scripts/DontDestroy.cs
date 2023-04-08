using UnityEngine;

public class DontDestroy : MonoBehaviour {
    private void Awake() {
        var objs = GameObject.FindGameObjectsWithTag("Player");

        switch (objs.Length) {
            case > 1:
                Destroy(gameObject);
                break;
        }

        DontDestroyOnLoad(gameObject);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperTrail : MonoBehaviour
{
    private LineRenderer lr;
    private float animationDelay = 0.2f;

    // Start is called before the first frame update
    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    public void SetTrail(Vector3 target, Vector3 weapon)
    {
        lr.SetPositions(new Vector3[] { weapon, target });

        StartCoroutine(TrailAnimation());
    }

    private IEnumerator TrailAnimation()
    {
        for (int i = 1; i <= 5; i++)
        {
            lr.material = Resources.Load<Material>("SniperTrail/SniperTrail" + i);
            yield return new WaitForSeconds(animationDelay);
        }
        Destroy(gameObject);
    }

    // Make sure trail gets destroyed if swapping weapons
    private void OnDisable()
    {
        Destroy(gameObject);
    }
}

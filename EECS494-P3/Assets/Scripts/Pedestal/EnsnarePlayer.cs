using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnsnarePlayer : MonoBehaviour
{
    private LineRenderer rend;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<LineRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        rend.SetPositions(new Vector3[] { transform.position, IsPlayer.instance.transform.position });
    }

}

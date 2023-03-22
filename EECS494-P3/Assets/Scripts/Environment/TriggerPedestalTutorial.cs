using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPedestalTutorial : MonoBehaviour
{
    [SerializeField] GameObject wall;

    private void OnTriggerEnter(Collider other)
    {
        wall.SetActive(true);
    }
}

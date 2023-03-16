using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectOnTrigger : MonoBehaviour
{
    [SerializeField] private int value = 1;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        EventBus.Publish(new CoinCollectedEvent(value));
    }
}

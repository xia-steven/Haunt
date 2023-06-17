using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CollectOnTrigger : MonoBehaviour
{
    [SerializeField] private int value = 1;
    [SerializeField] private AudioClip coin_collected_sound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioSource.PlayClipAtPoint(coin_collected_sound, transform.position);
            EventBus.Publish(new CoinEvent(value));
            Destroy(gameObject);
        }
            
    }
}

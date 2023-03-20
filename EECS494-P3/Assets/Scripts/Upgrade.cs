using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public abstract class Upgrade : MonoBehaviour
{
    [SerializeField] private float bobDistance = 1f;
    [SerializeField] private float bobSpeed = 3f;
    [SerializeField] GameObject itemDescription;
    [SerializeField] private int cost = 10;
    private Vector3 origin;
    private Vector3 destination;
    private bool selected = false;
    private Inventory playerInventory;
    
    
    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        destination = new Vector3(origin.x, origin.x + bobDistance, origin.z);
        playerInventory = GameObject.Find("Player").GetComponent<Inventory>();

    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            selected = true;
            StartCoroutine(BobUp());
        }
                
    
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            selected = false;
            StartCoroutine(BobDown());
        }
    }

    private IEnumerator BobUp()
    {
        itemDescription.SetActive(true);
        while (Vector3.Distance(transform.position, destination) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime*bobSpeed);
            yield return null;
        }

        transform.position = destination;
    }
    
    private IEnumerator BobDown()
    {
        itemDescription.SetActive(false);
        while (Vector3.Distance(transform.position, origin) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, origin, Time.deltaTime*bobSpeed);
            yield return null;
        }
        transform.position = origin;
    }

    private void OnPurchase()
    {
        if (selected && playerInventory.GetCoins() >= cost)
        {
            EventBus.Publish(new CoinEvent(-cost));
            ApplyUpgrade();
            Destroy(gameObject);
            
        }
    }

    protected abstract void ApplyUpgrade();
}

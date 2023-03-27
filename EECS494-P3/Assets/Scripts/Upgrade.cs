using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Upgrade : MonoBehaviour
{
    [SerializeField] private float bobDistance = 1f;
    [SerializeField] private float bobSpeed = 3f;
    [SerializeField] GameObject itemDescription;
    [SerializeField] private int cost = 10;
    private Vector3 origin;
    private Vector3 destination;
    private bool selected = false;
    private Inventory playerInventory;
    private Subscription<TryInteractEvent> interact_subscription;
    
    
    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        destination = new Vector3(origin.x, origin.y + bobDistance, origin.z);
        playerInventory = GameObject.Find("Player").GetComponent<Inventory>();
        interact_subscription = EventBus.Subscribe<TryInteractEvent>(OnPurchase);

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
            Debug.DrawLine(transform.position, destination);
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime*bobSpeed);
            yield return null;
        }

        transform.position = destination;
        Debug.Log("BobUp:" + selected);
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
        Debug.Log("BobDown: " + selected);
    }

    private void OnPurchase(TryInteractEvent e)
    {
        Debug.Log("Attempted purchase," + Vector3.Distance(transform.position, playerInventory.gameObject.transform.position));
        if (Vector3.Distance(transform.position, playerInventory.gameObject.transform.position) < 1f && playerInventory.GetCoins() >= cost)
        {
            EventBus.Publish(new CoinEvent(-cost));
            ApplyUpgrade();
            Destroy(gameObject);
            
        }
    }

    protected virtual void ApplyUpgrade()
    {
        
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<TryInteractEvent>(interact_subscription);
    }
}

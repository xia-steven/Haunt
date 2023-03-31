using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IsBuyable : MonoBehaviour
{
    
    [SerializeField] protected float bobDistance = 1f;
    [SerializeField] protected float bobSpeed = 3f;

    protected int cost;

    // access to a sub-object
    [SerializeField] protected TextMeshPro itemDescription;

    protected Inventory playerInventory;
    protected Subscription<TryInteractEvent> interact_subscription;

    

    private Vector3 origin;
    private Vector3 destination;
    private bool selected = false;

    protected virtual void Awake()
    {
        origin = transform.position;
        destination = new Vector3(origin.x, origin.y + bobDistance, origin.z);

        playerInventory = GameObject.Find("Player").GetComponent<Inventory>();
        interact_subscription = EventBus.Subscribe<TryInteractEvent>(OnPurchase);
    }

    private void OnPurchase(TryInteractEvent e)
    {
        Debug.Log("Attempted purchase," + Vector3.Distance(transform.position, playerInventory.gameObject.transform.position));
        if (Vector3.Distance(transform.position, playerInventory.gameObject.transform.position) < 1f && playerInventory.GetCoins() >= cost)
        {
            EventBus.Publish(new CoinEvent(-cost));
            Apply();
            Destroy(gameObject);
            
        }
    }

    protected virtual void Apply()
    {
        
    }

    private IEnumerator BobUp()
    {
        itemDescription.gameObject.SetActive(true);
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
        itemDescription.gameObject.SetActive(false);
        while (Vector3.Distance(transform.position, origin) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, origin, Time.deltaTime*bobSpeed);
            yield return null;
        }
        transform.position = origin;
        Debug.Log("BobDown: " + selected);
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

    
    protected virtual void OnDestroy()
    {
        EventBus.Unsubscribe<TryInteractEvent>(interact_subscription);
    }
}

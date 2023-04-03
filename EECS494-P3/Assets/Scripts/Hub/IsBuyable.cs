using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IsBuyable : MonoBehaviour
{
    
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] protected int cost;
    // access to a sub-object
    [SerializeField] protected GameObject itemDescription;

    protected Inventory playerInventory;
    protected Subscription<TryInteractEvent> interact_subscription;

    private bool selected = false;
    private SpriteRenderer sr;
    private Sprite defaultSprite;

    protected virtual void Awake()
    {
        playerInventory = GameObject.Find("Player").GetComponent<Inventory>();
        interact_subscription = EventBus.Subscribe<TryInteractEvent>(OnPurchase);
        sr = GetComponent<SpriteRenderer>();
        defaultSprite = sr.sprite;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "ItemDescription")
            {
                itemDescription = transform.GetChild(i).gameObject;
            }

        }
         
    }

    private void OnPurchase(TryInteractEvent e)
    {
        Debug.Log("Purchase attempted for " + gameObject);
        if (selected && playerInventory.GetCoins() >= cost)
        {
            EventBus.Publish(new CoinEvent(-cost));
            Apply();
            Destroy(gameObject);
            
        }
    }

    protected virtual void Apply()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerPhysical"))
        {
            Debug.Log(gameObject.name + " triggered");
            selected = true;
            sr.sprite = selectedSprite;
            if (itemDescription)
                itemDescription.SetActive(true);
        }
                
    
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerPhysical"))
        {
            selected = false;
            sr.sprite = defaultSprite;
            if (itemDescription)
                itemDescription.SetActive(false);

        }
    }

    
    protected virtual void OnDestroy()
    {
        EventBus.Unsubscribe<TryInteractEvent>(interact_subscription);
    }
}

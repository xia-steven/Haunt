using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IsBuyable : MonoBehaviour
{
    
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] protected int cost;
    // access to a sub-object
    [SerializeField] protected TextMeshPro itemDescription;

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
    }

    private void OnPurchase(TryInteractEvent e)
    {
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
            selected = true;
            sr.sprite = selectedSprite;
        }
                
    
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerPhysical"))
        {
            selected = false;
            sr.sprite = defaultSprite;
        }
    }

    
    protected virtual void OnDestroy()
    {
        EventBus.Unsubscribe<TryInteractEvent>(interact_subscription);
    }
}

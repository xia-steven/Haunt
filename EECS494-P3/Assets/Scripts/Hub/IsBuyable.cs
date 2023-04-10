using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IsBuyable : MonoBehaviour
{
    
    [SerializeField] private Sprite selectedSprite;
    // Cost of shield and health
    protected int cost = 4;
    // access to a sub-object
    protected GameObject itemDescription;
    protected TextMeshPro descriptionText;
    protected TextMeshPro costText;

    protected Inventory playerInventory;
    protected Subscription<TryInteractEvent> interact_subscription;

    private bool selected = false;
    private SpriteRenderer sr;
    private Sprite defaultSprite;


    protected static PurchaseableTypesData typesData;
    // Data for buyable items
    protected UpgradeData thisData;


    protected virtual void Awake()
    {
        playerInventory = GameObject.Find("Player").GetComponent<Inventory>();
        interact_subscription = EventBus.Subscribe<TryInteractEvent>(OnPurchase);
        sr = GetComponent<SpriteRenderer>();
        defaultSprite = sr.sprite;

        if (typesData == null)
            typesData = ConfigManager.GetData<PurchaseableTypesData>("PurchaseableTypes");

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "ItemDescription")
            {
                itemDescription = transform.GetChild(i).gameObject;
                for (int j = 0; j < itemDescription.transform.childCount; j++)
                {
                    if (itemDescription.transform.GetChild(j).name == "Text")
                    {
                        descriptionText = itemDescription.transform.GetChild(j).gameObject.GetComponent<TextMeshPro>();
                    }
                    if (itemDescription.transform.GetChild(j).name == "Cost")
                    {
                        costText = itemDescription.transform.GetChild(j).gameObject.GetComponent<TextMeshPro>();
                    }
                }
            }

        }
         
    }

    protected void Start()
    {
        itemDescription.SetActive(false);

    }

    private void OnPurchase(TryInteractEvent e)
    {
        Debug.Log("Purchase attempted for " + gameObject);
        if (selected && playerInventory.GetCoins() >= cost)
        {
            EventBus.Publish(new CoinEvent(-cost));
            AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Audio/coin"), transform.position);
            Apply();
            Destroy(gameObject);
            
        }
    }

    protected virtual void Apply()
    {
        
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (!selected && other.tag == "Player")
        {
            Debug.Log(gameObject.name + " triggered");
            selected = true;
            sr.sprite = selectedSprite;
            if (itemDescription)
            {
                Debug.Log(costText.text);
                itemDescription.SetActive(true);
                costText.text = cost.ToString();
            }
        }
                
    
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
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


public enum PurchaseableType
{
    dashReflect = 0,        //dashing through incoming shots reflects them
    dashExplode = 1,        //dashes leave behind explosives
    damageRage = 2,         //extra damage and move speed after taking damage
    doubleDashDamage = 3,   //first shot after dash deals extra damage
    piercingShot = 4,       //shots pierce one more enemy
    speed = 5,              //extra speed
    stationaryDamage = 6,   //extra damage while standing still
    fastReload = 7,         //faster reload time
    shotgun = 8,            //shotgun
    sniper = 9,             //sniper
    minigun = 10,           //minigun
    sword = 11,             //sword/bazooka (once implemented)
    radius = 12,            //expands explosion radius
    launcher = 13           //launcher
}
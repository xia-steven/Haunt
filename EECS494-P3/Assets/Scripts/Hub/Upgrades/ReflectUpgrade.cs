using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectUpgrade : Upgrade
{
    private GameObject dashShield;
    private GameObject appliedShield;
    private GameObject player;
    private Subscription<PlayerDodgeEvent> dodgeEvent;


    protected override void Awake()
    {
        dashShield = Resources.Load<GameObject>("Prefabs/DashShield");
        dodgeEvent = EventBus.Subscribe<PlayerDodgeEvent>(_OnDodge);
        base.Awake();
    }

    protected override void Start()
    {
        thisData = typesData.types[(int)UpgradeType.dashReflect];
        base.Start();
    }

    protected override void Apply()
    {
        player = GameObject.Find("Player");
        base.Apply();
    }

    // Attach shield on dodge start and destroy it on dodge finish
    private void _OnDodge(PlayerDodgeEvent e)
    {
        if (e.start)
        {
            appliedShield = Instantiate(dashShield, player.transform);
        } 
        else
        {
            Destroy(appliedShield);
        }
    }

    protected override void OnDestroy()
    {
        EventBus.Unsubscribe(dodgeEvent);
        base.OnDestroy();
    }
}

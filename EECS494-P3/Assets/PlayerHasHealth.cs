using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHasHealth : HasHealth
{
    Subscription<PlayerDamagedEvent> damageSub;

    // Start is called before the first frame update
    void Start()
    {
        damageSub = EventBus.Subscribe<PlayerDamagedEvent>(_OnPlayerDamaged);
    }


    void _OnPlayerDamaged(PlayerDamagedEvent pde)
    {
        AlterHealth(-pde.damageTaken);

        if (health == 0)
        {
            EventBus.Publish(new GameLossEvent());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

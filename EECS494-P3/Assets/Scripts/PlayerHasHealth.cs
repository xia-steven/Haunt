using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHasHealth : HasHealth {
    Subscription<PlayerDamagedEvent> damageSub;
    Subscription<PedestalDestroyedEvent> pedDestSub;
    Subscription<PedestalRepairedEvent> pedRepSub;

    // Start is called before the first frame update
    void Start() {
        damageSub = EventBus.Subscribe<PlayerDamagedEvent>(_OnPlayerDamaged);
        pedDestSub = EventBus.Subscribe<PedestalDestroyedEvent>(_OnPedestalDied);
        pedRepSub = EventBus.Subscribe<PedestalRepairedEvent>(_OnPedestalRepaired);
    }


    void _OnPlayerDamaged(PlayerDamagedEvent pde) {
        AlterHealth(-pde.damageTaken);

        if (health == 0) {
            EventBus.Publish(new GameLossEvent());
        }
    }

    void _OnPedestalDied(PedestalDestroyedEvent pde) {
        maxHealth -= 2;
        if (health > maxHealth) {
            AlterHealth(-(maxHealth - health));
        }
    }

    void _OnPedestalRepaired(PedestalRepairedEvent pre) {
        maxHealth += 2;
    }

    private void OnDestroy() {
        EventBus.Unsubscribe(pedDestSub);
        EventBus.Unsubscribe(pedRepSub);
    }
}
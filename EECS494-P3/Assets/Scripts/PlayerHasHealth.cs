using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class PlayerHasHealth : HasHealth {
    Subscription<PlayerDamagedEvent> damageSub;
    Subscription<PedestalDestroyedEvent> pedDestSub;
    Subscription<PedestalRepairedEvent> pedRepSub;

    [SerializeField] private float invincibilityTimer = 1f;
    private bool isInvincible = false;

    // Start is called before the first frame update
    void Start() {
        damageSub = EventBus.Subscribe<PlayerDamagedEvent>(_OnPlayerDamaged);
        pedDestSub = EventBus.Subscribe<PedestalDestroyedEvent>(_OnPedestalDied);
        pedRepSub = EventBus.Subscribe<PedestalRepairedEvent>(_OnPedestalRepaired);
    }


    void _OnPlayerDamaged(PlayerDamagedEvent pde) {
        if (!isInvincible)
        {
            
            AlterHealth(-pde.damageTaken);
            StartCoroutine(TriggerInvincibility());
        }
        

        if (health == 0) {
            EventBus.Publish(new GameLossEvent());
        }
    }

    void _OnPedestalDied(PedestalDestroyedEvent pde) {
        maxHealth += 2;
        
    }

    void _OnPedestalRepaired(PedestalRepairedEvent pre) {
        maxHealth -= 2;
        if (health > maxHealth) {
            AlterHealth(maxHealth - health);
        }
    }

    public void UpgradeHealth()
    {
        maxHealth += 2;
        health = maxHealth;
    }
    
    private IEnumerator TriggerInvincibility()
    {
        isInvincible = true;
        float duration = 0;
        SpriteRenderer sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        Color normalColor = sr.color;
        while (duration < invincibilityTimer)
        {
            Debug.Log("Inv_timer:" + duration);
            duration += 0.1f;
            normalColor.a = 1 - normalColor.a;
            sr.color = normalColor;
            yield return new WaitForSeconds(0.1f);
        }

        normalColor.a = 1;
        sr.color = normalColor;
        isInvincible = false;

    }
    private void OnDestroy() {
        EventBus.Unsubscribe(pedDestSub);
        EventBus.Unsubscribe(pedRepSub);
    }
}
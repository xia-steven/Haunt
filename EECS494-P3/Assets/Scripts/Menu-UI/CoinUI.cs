using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinUI : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText;

    private int coinCount = 0;
    Subscription<CoinCollectedEvent> coinSub;
    void Start()
    {
        coinSub = EventBus.Subscribe<CoinCollectedEvent>(_OnCollected);
    }

   void _OnCollected(CoinCollectedEvent e)
    {
        coinCount += e.coinValue;
        coinText.text = coinCount.ToString();

    }
    
    private void OnDestroy() {

        EventBus.Unsubscribe(coinSub);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinUI : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText;

    private int coinCount = 0;
    Subscription<CoinEvent> coinSub;
    void Start()
    {
        coinSub = EventBus.Subscribe<CoinEvent>(_OnCoinCountChange);
    }

   void _OnCoinCountChange(CoinEvent e)
    {
        coinCount += e.coinValue;
        coinText.text = coinCount.ToString();

    }
    
    private void OnDestroy() {

        EventBus.Unsubscribe(coinSub);
    }
}

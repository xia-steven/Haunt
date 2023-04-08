using System.Collections;
using Events;
using Player;
using TMPro;
using UnityEngine;

namespace Menu_UI {
    public class CoinUI : MonoBehaviour {
        [SerializeField] private TMP_Text coinText;

        private int coinCount;
        private Subscription<CoinEvent> coinSub;

        private void Start() {
            coinSub = EventBus.Subscribe<CoinEvent>(_OnCoinCountChange);
            StartCoroutine(LoadCoins());
        }

        private IEnumerator LoadCoins() {
            yield return null;
            coinCount = GameObject.Find("Player").GetComponent<Inventory>().GetCoins();
            coinText.text = coinCount.ToString();
        }

        private void _OnCoinCountChange(CoinEvent e) {
            coinCount += e.coinValue;
            coinText.text = coinCount.ToString();
        }

        private void OnDestroy() {
            EventBus.Unsubscribe(coinSub);
        }
    }
}
using Events;
using UnityEngine;

namespace Pedestal {
    [RequireComponent(typeof(HasPedestalHealth))]
    public class IsPedestal : MonoBehaviour {
        [SerializeField] private int UUID = -1;
        [SerializeField] private bool startRepaired;
        private HasPedestalHealth pedestalHealth;

        private bool playerDestroyed = true;

        // Start is called before the first frame update
        private void Start() {
            pedestalHealth = GetComponent<HasPedestalHealth>();
        }

        // Update is called once per frame
        private void Update() {
            switch (startRepaired) {
                case true:
                    // Manually repair the pedestal
                    pedestalHealth.AlterHealth(-500);
                    startRepaired = false;
                    break;
            }

            DebugKeys();
        }

        public int getUUID() {
            return UUID;
        }

        public void PedestalDied() {
            Debug.Log("Pedestal destroyed by player :)");
            playerDestroyed = true;
            EventBus.Publish(new PedestalDestroyedEvent(UUID));
        }

        public void PedestalRepaired() {
            Debug.Log("Pedestal restored by enemies :(");
            playerDestroyed = false;
            EventBus.Publish(new PedestalRepairedEvent(UUID));
        }

        public bool IsDestroyedByPlayer() {
            return playerDestroyed;
        }

        private void DebugKeys() {
            if (Input.GetKeyDown(KeyCode.K) && UUID == 1)
                pedestalHealth.AlterHealth(-1);
            else if (Input.GetKeyDown(KeyCode.J) && UUID == 1) pedestalHealth.AlterHealth(1);
        }
    }
}
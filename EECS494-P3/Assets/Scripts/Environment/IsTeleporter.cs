using Events;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Environment {
    public class IsTeleporter : MonoBehaviour {
        [SerializeField] private string otherScene;
        [SerializeField] private GameObject selectedRing;

        private MeshRenderer visualRenderer;
        private Animator anim;


        private Sprite eSprite;
        private SpritePromptEvent ePrompt;

        private bool sentPrompt;

        private bool isActive = true;

        public bool Active {
            get => isActive;
            set {
                var tmp = isActive;
                isActive = value;
                switch (isActive) {
                    case true when !tmp:
                        Activate();
                        break;
                    default: {
                        switch (tmp) {
                            case true:
                                Deactivate();
                                break;
                        }

                        break;
                    }
                }
            }
        }

        private bool isUsable;

        private Subscription<TryInteractEvent> interactSub;

        // Start is called before the first frame update
        private void Awake() {
            anim = GetComponent<Animator>();

            Activate();

            interactSub = EventBus.Subscribe<TryInteractEvent>(_Interact);

            var sprites = Resources.LoadAll("tilemap");
            eSprite = (Sprite)sprites[360];

            ePrompt = new SpritePromptEvent(eSprite, KeyCode.E);
        }

        private void OnTriggerStay(Collider other) {
            switch (isUsable) {
                case false when isActive && other.CompareTag("Player") && !sentPrompt:
                    selectedRing.SetActive(true);
                    isUsable = true;
                    ePrompt.cancelPrompt = false;
                    EventBus.Publish(ePrompt);
                    sentPrompt = true;
                    break;
            }
        }

        private void OnTriggerExit(Collider other) {
            if (!other.CompareTag("Player")) return;
            ePrompt.cancelPrompt = true;
            sentPrompt = false;
            selectedRing.SetActive(false);
            isUsable = false;
        }

        private void Activate() {
            Debug.Log(isActive);
            //activate other visuals here
            anim.SetBool("isActive", isActive);
        }

        private void Deactivate() {
            //deactivate other visuals here
            Debug.Log(isActive);
            anim.SetBool("isActive", isActive);
        }

        private void _Interact(TryInteractEvent e) {
            switch (isUsable) {
                case true: {
                    IsPlayer.SetPosition(new Vector3(0, .25f, 0));
                    if (Game_Control.GameControl.Day != 3 || SceneManager.GetActiveScene().name != "HubWorld")
                        SceneManager.LoadScene(otherScene);
                    else if (SceneManager.GetActiveScene().name == "HubWorld") SceneManager.LoadScene("LAB_BossTesting");
                    break;
                }
            }
        }
    }
}
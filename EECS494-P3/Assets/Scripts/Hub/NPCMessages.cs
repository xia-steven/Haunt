using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCMessages : MonoBehaviour
{
    [SerializeField] string configName = "REPLACE ME";

    MessageList NPCMessageData;
    static CoinMessageList coinMessages;
    [SerializeField] GameObject interactSprite;
    [SerializeField] GameObject initialBubble;
    [SerializeField] bool isGhost = false;
    [SerializeField] bool givesRandomCoins = false;
    bool selected = false;

    bool spoken = false;

    bool sentMessage = false;

    bool triedGivingCoins = false;

    public static bool givenInitialCoins = false;

    Sprite eSprite;
    Sprite ePressedSprite;
    SpritePromptEvent ePrompt;

    Subscription<TryInteractEvent> interactSubscription;
    Subscription<MessageFinishedEvent> finishedSub;
    Subscription<MessageStartedEvent> startedSub;

    GameObject coinPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if(coinMessages == null)
        {
            coinMessages = ConfigManager.GetData<CoinMessageList>("CoinMessageData");
        }

        coinPrefab = Resources.Load<GameObject>("Prefabs/Coin");


        NPCMessageData = ConfigManager.GetData<MessageList>(configName);
        interactSubscription = EventBus.Subscribe<TryInteractEvent>(OnInteract);
        finishedSub = EventBus.Subscribe<MessageFinishedEvent>(OnMessageFinished);
        startedSub = EventBus.Subscribe<MessageStartedEvent>(OnMessageStarted);

        interactSprite.SetActive(false);
        Object [] sprites = Resources.LoadAll("tilemap");
        eSprite = (Sprite)sprites[360];
        ePressedSprite = (Sprite)sprites[88];

        ePrompt = new SpritePromptEvent(eSprite, ePressedSprite, KeyCode.E);
        ePrompt.cancelPrompt = true;
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(interactSubscription);
        EventBus.Unsubscribe(finishedSub);
        EventBus.Unsubscribe(startedSub);

        ePrompt.cancelPrompt = true;
    }

    private void OnTriggerStay(Collider other)
    {
        // Only trigger on the player
        if (other.tag != "Player") return;

        if(spoken)
        {
            interactSprite.SetActive(true);
        }
        selected = true;
        if(ePrompt.cancelPrompt)
        {
            ePrompt.cancelPrompt = false;
            EventBus.Publish(ePrompt);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Only trigger on the player
        if (other.tag != "Player") return;

        if (spoken)
        {
            interactSprite.SetActive(false);
        }
        selected = false;
        ePrompt.cancelPrompt = true;
    }

    private void OnInteract(TryInteractEvent e)
    {
        if(selected && !sentMessage)
        {
            
            initialBubble.SetActive(false);
            if(SceneManager.GetActiveScene().name == "TutorialHubWorld")
            {
                // Ghost sends initial message
                EventBus.Publish(new MessageEvent(NPCMessageData.initialTutorial, GetInstanceID(), false, NPCMessageData.name));
            }
            else if (givesRandomCoins && !triedGivingCoins )
            {
                List<string> messagesToSend = new List<string>();
                triedGivingCoins = true;
                int chance = Random.Range(0, 2);
                // 50% change to give a coin
                if(chance == 0)
                {
                    messagesToSend.Add(coinMessages.possibleMessages[Random.Range(0, coinMessages.possibleMessages.Count)].messages[0]);

                    //GameObject coin = Instantiate(coinPrefab, transform.position + new Vector3(0, 0, -1), Quaternion.identity);
                    EventBus.Publish(new CoinEvent(1));
                }

                for(int c = 0; c < NPCMessageData.allMessages[GameControl.Day].messages.Count; ++c)
                {
                    messagesToSend.Add(NPCMessageData.allMessages[GameControl.Day].messages[c]);
                }

                // Standard dialogue for the night (failed giving coins)
                EventBus.Publish(new MessageEvent(messagesToSend, GetInstanceID(), false, NPCMessageData.name));
            }
            else
            {
                if (!givenInitialCoins && isGhost)
                {
                    EventBus.Publish(new MessageEvent(NPCMessageData.initialCoinGift, GetInstanceID(), false, NPCMessageData.name));
                    EventBus.Publish(new CoinEvent(4));
                    givenInitialCoins = true;
                }
                else
                {
                    // Standard dialogue for the night
                    EventBus.Publish(new MessageEvent(NPCMessageData.allMessages[GameControl.Day].messages, GetInstanceID(), false, NPCMessageData.name));
                }

            }

            sentMessage = true;
            spoken = true;
        }
    }

    private void OnMessageStarted(MessageStartedEvent mse)
    {
        if (mse.senderInstanceID != GetInstanceID())
            return;

        // Send talking sounds
        if (!isGhost)
        {
            int clipNum = (int)Random.Range(1, 15);
            AudioClip clip = Resources.Load<AudioClip>("Audio/mnstr" + clipNum);
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }
        else
        {
            int clipNum = (int)Random.Range(1, 8);
            AudioClip clip = Resources.Load<AudioClip>("Audio/ghost" + clipNum);
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }
    }

    private void OnMessageFinished(MessageFinishedEvent mfe)
    {
        sentMessage = false;
        // Activate teleporter after first message
        if(mfe.senderInstanceID == GetInstanceID() && isGhost && SceneManager.GetActiveScene().name == "TutorialHubWorld")
        {
            EventBus.Publish(new ActivateTeleporterEvent());
        }
        // Activate sword after first shopkeeper message
        else if (mfe.senderInstanceID == GetInstanceID() && !isGhost && GameControl.Day == 0)
        {

        }

    }
}

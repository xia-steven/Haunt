using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour
{
    private AudioSource audioSource;

    Subscription<GamePauseEvent> gamePauseSubscription;
    Subscription<GamePlayEvent> gamePlaySubscription;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        audioSource = GetComponent<AudioSource>();

        gamePauseSubscription = EventBus.Subscribe<GamePauseEvent>(_OnPause);
        gamePlaySubscription = EventBus.Subscribe<GamePlayEvent>(_OnPlay);
    }

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        // Get audio source
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (s.name == "GameScene")
        {
            // Start gameplay music
            Debug.Log("Set clip to GameplayMusic");
            audioSource.clip = Resources.Load<AudioClip>("Audio/Music/GameplayMusic");
        } else if (s.name == "HubWorld")
        {
            // Start shop music
            Debug.Log("Set clip to ShopMusic");
            audioSource.clip = Resources.Load<AudioClip>("Audio/Music/ShopMusic");
        } else if (s.name == "MainMenu")
        {
            // Start menu music
            Debug.Log("Set clip to ShopMusic");
            audioSource.clip = Resources.Load<AudioClip>("Audio/Music/ShopMusic");
        } else if (s.name == "TutorialGameScene")
        {
            // Start tutorial music (gameplay music for now)
            Debug.Log("Set clip to GameplayMusic");
            audioSource.clip = Resources.Load<AudioClip>("Audio/Music/GameplayMusic");
        } else if (s.name == "BossScene")
        {
            // Start boss music
            Debug.Log("Set clip to BossMusic");
            audioSource.clip = Resources.Load<AudioClip>("Audio/Music/BossMusic");
        } else if (s.name == "TutorialHubWorld")
        {
            // Start shop music
            Debug.Log("Set clip to ShopMusic");
            audioSource.clip = Resources.Load<AudioClip>("Audio/Music/ShopMusic");
        } else
        {
            // Play shop music if nothing else
            Debug.Log("Set clip to ShopMusic");
            audioSource.clip = Resources.Load<AudioClip>("Audio/Music/ShopMusic");
        }

        PlayMusic();
    }

    private void PlayMusic()
    {
        Debug.Log("Playing music");
        audioSource.Play();
    }

    private void StopMusic()
    {
        Debug.Log("Stopping music");
        audioSource.Stop();
    }

    private void _OnPause(GamePauseEvent e)
    {
        StopMusic();
    }

    private void _OnPlay(GamePlayEvent e)
    {
        PlayMusic();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

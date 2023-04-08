using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour
{
    private AudioSource audioSource;
    private bool musicPlaying = false;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        audioSource = GetComponent<AudioSource>();
        Debug.Log("Audio source: " + audioSource);
    }

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
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
        } else if (s.name == "LAB_BossTesting")
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
        musicPlaying = true;
        audioSource.Play();
    }

    private void StopMusic()
    {
        musicPlaying = false;
        audioSource.Stop();
    }

    private void Update()
    {
        if (GameControl.GamePaused && musicPlaying)
        {
            StopMusic();
        } else if (!GameControl.GamePaused && !musicPlaying)
        {
            PlayMusic();
        }
    }
}

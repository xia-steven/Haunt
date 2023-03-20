using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * This file contains code which controls much of the game state.
 * For all public attributes, see GameControl_Public.cs.
 */
partial class GameControl : MonoBehaviour {
    /*Turn this off if you don't want the night cycle to run*/
    const bool DEBUG_DO_DAYNIGHT = true;


    /*CONTROL PARAMETERS*/

    const int maxDays = 3;
    const float nightLength = 30f;//180f; //3 minutes
    const float waveTimeout = 20f;
    const float updateFrequency = .05f; // .05 -> 20 times/second
    const float waveDailyScale = 2.5f; //multiplies initial size daily
    const float initialWaveSize = 2; //2 non-pedestal enemies

    /*END CONTROL PARAMETERS*/


    /*STATE DEPENDENT VARIABLES*/
    private float nightStartTime;

    private static int day = 0; //set to 1 less than the first day

    private bool gameActive = false;
    private bool gamePaused = false;
    private static bool isNight = false;
    private bool nightEnding = false;

    private int waveSize;

    /*END STATE DEPENDENT VARIABLES*/

    //singleton
    static GameControl instance;

    /*Editor objects held by singleton instance*/
    [SerializeField] List<Transform> spawners;
    /*End editor objects held by singleton*/

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Start is called before the first frame update
    void Start() {

        startSub = EventBus.Subscribe<GameStartEvent>(_Start);
        lossSub = EventBus.Subscribe<GameLossEvent>(_Lose);
        winSub = EventBus.Subscribe<GameWinEvent>(_Win);
        pauseSub = EventBus.Subscribe<GamePauseEvent>(_Pause);
        playSub = EventBus.Subscribe<GamePlayEvent>(_Play);
        nightStartSub = EventBus.Subscribe<NightBeginEvent>(_NightStart);
        nightEndEvent = EventBus.Subscribe<NightEndEvent>(_NightEnd);

        waveSize = (int)(initialWaveSize * Mathf.Pow(waveDailyScale, day));

        StartGame();
    }

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        if (s.name == "GameScene")
        {
            StartCoroutine(StartOnDelay(StartNight, .01f));
        }
        else
        {
            isNight = false;
            StartCoroutine(StartOnDelay(DayUpdate, .01f));
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    delegate IEnumerator delayedEnum();
    delegate void delayedVoid();

    private IEnumerator StartOnDelay(delayedEnum f, float delay)
    {
        yield return new WaitForSeconds(delay);

        StartCoroutine(f());
    }

    private IEnumerator StartOnDelay(delayedVoid f, float delay)
    {
        yield return new WaitForSeconds(delay);

        f();
    }


    //NightUpdate runs during the night
    private IEnumerator NightUpdate() {
        Wave w;
        w = new Wave(waveSize++, waveTimeout, spawners);
        w.Spawn();
        nightStartTime = Time.time;
        while (isNight && Time.time - nightStartTime < nightLength) {
            if (!gameActive || gamePaused) {
                yield return new WaitForSeconds(updateFrequency);
                continue;
            }

            if (w.IsOver()) {
                w = new Wave(waveSize++, waveTimeout, spawners);
                w.Spawn();
            }

            yield return new WaitForSeconds(updateFrequency);
        }

        if (isNight) EndNight();
    }
    
    private IEnumerator NightEndingUpdate()
    {
        Wave w;
        w = new Wave(15, 5, spawners);
        w.Spawn();
        while(nightEnding)
        {
            if (!gameActive || gamePaused) {
                yield return new WaitForSeconds(updateFrequency);
                continue;
            }

            if (w.IsOver())
            {
                w = new Wave(4, 5, spawners, false);
                w.Spawn();
            }

            yield return new WaitForSeconds(updateFrequency);
        }
    }

    //DayUpdate runs while it is day
    private IEnumerator DayUpdate() {
        yield return null;
        if (day == maxDays) WinGame();
        
        while (!isNight)
        {
            if(!gameActive || gamePaused)
            {
                yield return new WaitForSeconds(updateFrequency);
                continue;
            }

            //code to start night may go here

            yield return new WaitForSeconds(updateFrequency);
        }
    }
}
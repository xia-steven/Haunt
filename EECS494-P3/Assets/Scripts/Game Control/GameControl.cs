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
    private const bool DEBUG_DO_DAYNIGHT = true;

    private GameData data;

    /*STATE DEPENDENT VARIABLES*/
    private float nightStartTime;

    //set to 1 less than the first day - does not increment during tutorial (tutorial is day 0)
    private static int day;

    private bool gameActive;
    private bool gamePaused;
    private static bool isNight;
    private bool nightEnding;
    private bool started;

    private int waveSize;

    /*END STATE DEPENDENT VARIABLES*/

    //singleton
    private static GameControl instance;

    /*Editor objects held by singleton instance*/
    [SerializeField] private List<Transform> spawners;
    /*End editor objects held by singleton*/

    private void Awake() {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Start is called before the first frame update
    private void Start() {
        startSub = EventBus.Subscribe<GameStartEvent>(_Start);
        lossSub = EventBus.Subscribe<GameLossEvent>(_Lose);
        winSub = EventBus.Subscribe<GameWinEvent>(_Win);
        pauseSub = EventBus.Subscribe<GamePauseEvent>(_Pause);
        playSub = EventBus.Subscribe<GamePlayEvent>(_Play);
        nightStartSub = EventBus.Subscribe<NightBeginEvent>(_NightStart);
        nightEndEvent = EventBus.Subscribe<NightEndEvent>(_NightEnd);

        data = ConfigManager.GetData<GameData>("GameData");

        waveSize = (int)(data.initialWaveSize * Mathf.Pow(data.waveDailyScale, day));

        StartGame();
        started = true;
    }

    private void OnSceneLoaded(Scene s, LoadSceneMode m) {
        if (s.name == "GameScene") {
            Debug.Log("GameScene Loaded");
            StartCoroutine(StartOnDelay(StartNight));
        }
        else {
            StartCoroutine(StartOnDelay(DayUpdate));
        }

        TimeManager.ResetTimeScale();
        EventBus.Publish(new ReloadAllEvent());
    }

    private void OnDisable() {
        isNight = false;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private delegate IEnumerator delayedEnum();

    private delegate void delayedVoid();

    private IEnumerator StartOnDelay(delayedEnum f) {
        while (!started) {
            yield return null;
        }

        StartCoroutine(f());
    }

    private IEnumerator StartOnDelay(delayedVoid f) {
        while (!started) {
            yield return null;
        }

        f();
    }


    //NightUpdate runs during the night
    private IEnumerator NightUpdate() {
        var w = new Wave(waveSize++, data.waveTimeout, spawners);
        w.Spawn();
        nightStartTime = Time.time;
        var nLength = data.nightLength;
        if (day == 0) {
            // Set tutorial night length to 15 seconds
            nLength = 15f;
        }

        while (isNight && Time.time - nightStartTime < nLength) {
            if (!gameActive || gamePaused) {
                yield return new WaitForSeconds(data.updateFrequency);
                continue;
            }

            if (w.IsOver()) {
                w = new Wave(waveSize++, data.waveTimeout, spawners);
                w.Spawn();
            }

            yield return new WaitForSeconds(data.updateFrequency);
        }

        if (isNight) EndNight();
    }

    private IEnumerator NightEndingUpdate() {
        var w = new Wave(15, 5, spawners);
        w.Spawn();
        while (nightEnding) {
            if (!gameActive || gamePaused) {
                yield return new WaitForSeconds(data.updateFrequency);
                continue;
            }

            if (w.IsOver()) {
                w = new Wave(4, 5, spawners, false);
                w.Spawn();
            }

            yield return new WaitForSeconds(data.updateFrequency);
        }
    }

    //DayUpdate runs while it is day
    private IEnumerator DayUpdate() {
        yield return null;
        //if (day == data.maxDays) WinGame();

        while (!isNight) {
            if (!gameActive || gamePaused) {
                yield return new WaitForSeconds(data.updateFrequency);
                continue;
            }

            //code to start night may go here

            yield return new WaitForSeconds(data.updateFrequency);
        }
    }
}

public class GameData : Savable {
    public int maxDays;
    public float nightLength; // seconds
    public float waveTimeout; // seconds
    public float updateFrequency; // seconds per update
    public float waveDailyScale; // base wave size multiplier per day (multiplicative)
    public float initialWaveSize; // number of non-pedestal enemies
}
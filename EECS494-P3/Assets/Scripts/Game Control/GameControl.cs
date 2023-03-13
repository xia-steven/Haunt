using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This file contains code which controls much of the game state.
 * For all public attributes, see GameControl_Public.cs.
 */
partial class GameControl : MonoBehaviour {
    /*Turn this off if you don't want the night cycle to run*/
    const bool DEBUG_DO_DAYNIGHT = true;


    /*CONTROL PARAMETERS*/

    const int maxDays = 1;
    const float nightLength = 300f; //5 minutes
    const float updateFrequency = .05f; // .05 -> 20 times/second

    /*END CONTROL PARAMETERS*/


    /*STATE DEPENDENT VARIABLES*/
    private float nightStartTime;

    private int day = 0; //set to 1 less than the first day

    private bool gameActive = false;
    private bool gamePaused = false;
    private bool isNight = false;

    private int waveSize = 2;

    /*END STATE DEPENDENT VARIABLES*/

    //singleton
    static GameControl instance;

    /*Editor objects held by singleton instance*/
    [SerializeField] List<Transform> spawners;
    /*End editor objects held by singleton*/

    // Start is called before the first frame update
    void Start() {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        startSub = EventBus.Subscribe<GameStartEvent>(_Start);
        lossSub = EventBus.Subscribe<GameLossEvent>(_Lose);
        winSub = EventBus.Subscribe<GameWinEvent>(_Win);
        pauseSub = EventBus.Subscribe<GamePauseEvent>(_Pause);
        playSub = EventBus.Subscribe<GamePlayEvent>(_Play);
        nightStartSub = EventBus.Subscribe<NightBeginEvent>(_NightStart);
        nightEndEvent = EventBus.Subscribe<NightEndEvent>(_NightEnd);

        //temp
        StartGame();
        StartNight();
    }

    //NightUpdate runs during the night
    private IEnumerator NightUpdate() {
        Wave w;
        w = new Wave(waveSize++, 300, spawners);
        w.Spawn();
        nightStartTime = Time.time;
        while (isNight && Time.time - nightStartTime < nightLength) {
            if (!gameActive || gamePaused) {
                yield return new WaitForSeconds(updateFrequency);
                continue;
            }

            if (w.IsOver()) {
                w = new Wave(waveSize++, 300, spawners);
                w.Spawn();
            }

            yield return new WaitForSeconds(updateFrequency);
        }

        if (isNight) EndNight();
    }

    //DayUpdate runs while it is day
    private IEnumerator DayUpdate() {
        yield return null;
        WinGame();
        /*
        while (!isNight)
        {
            if(!gameActive || gamePaused)
            {
                yield return new WaitForSeconds(updateFrequency);
                continue;
            }

            //code to start night may go here

            yield return new WaitForSeconds(updateFrequency);
        }*/
    }
}
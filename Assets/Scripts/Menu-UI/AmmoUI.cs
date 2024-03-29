using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] Image gunImg;
    [SerializeField] Transform tickHolder;
    [SerializeField] GameObject bulletTick;
    //[SerializeField] GameObject reloadSprite;

    Subscription<WeaponSwapEvent> swapSub;
    Subscription<ReloadStartedEvent> reloadSub;

    List<GameObject> shellInstances = new List<GameObject>();

    Weapon current;
    int clipMax;
    int curClip;
    float reloadTime;

    int knownClip;

    bool reloading = false;
    bool swapped = false;

    void Awake()
    {
        swapSub = EventBus.Subscribe<WeaponSwapEvent>(_OnSwap);
        reloadSub = EventBus.Subscribe<ReloadStartedEvent>(_OnReload);
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    void OnSceneLoad(Scene s, LoadSceneMode m)
    {
        curClip = clipMax;
        UpdateBulletCounts();
        Debug.Log("Here?");
    }


    // Update is called once per frame
    void Update()
    {
        if (reloading || current == null) return;

        clipMax = current.FullClipAmount;
        curClip = current.CurrentClipAmount;

        if (curClip == 0)
        {
            //reloadSprite.SetActive(true);
        }

        if (curClip < knownClip)
        {
            RemoveBullets();
        }

        knownClip = curClip;
        swapped = false;
    }

    void _OnSwap(WeaponSwapEvent e)
    {
        if (!gameObject.activeInHierarchy) return;

        swapped = true;

        current = e.newWeapon;
        clipMax = current.FullClipAmount;
        curClip = current.CurrentClipAmount;
        reloadTime = current.ReloadTime;

        string weaponType = current.Type;

        if (weaponType == "pistol")
        {
            gunImg.sprite = Resources.LoadAll<Sprite>("Textures-Sprites/six_shooter")[1];
        }
        else if (weaponType == "rifle")
        {
            gunImg.sprite = Resources.LoadAll<Sprite>("Textures-Sprites/minigun")[0];
        }
        else if (weaponType == "shotgun")
        {
            gunImg.sprite = Resources.LoadAll<Sprite>("Textures-Sprites/shotguns")[1];
        }
        else if (weaponType == "sniper")
        {
            gunImg.sprite = Resources.LoadAll<Sprite>("Textures-Sprites/sniper")[0];
        }
        else if (weaponType == "launcher")
        {
            gunImg.sprite = Resources.LoadAll<Sprite>("Textures-Sprites/grenade_launchers")[0];
        }

        gunImg.SetNativeSize();

        UpdateBulletCounts();
        knownClip = curClip;
    }

    void UpdateBulletCounts()
    {
        int i = 0;

        // repurpose any already existing bullet ticks
        for(; i < clipMax && i < shellInstances.Count; ++i)
        {
            if (i < curClip)
                shellInstances[i].SetActive(true);
            else
                shellInstances[i].SetActive(false);
        }

        //if need more ticks
        if (i < clipMax)
        {
            //make new ticks
            for(; i < clipMax; ++i)
            {
                shellInstances.Add(Instantiate(bulletTick, tickHolder).transform.GetChild(1).gameObject);
                if (i >= curClip)
                    shellInstances[i].SetActive(false);
            }
        }
        //else if need less ticks
        else if (i < shellInstances.Count)
        {
            //remove excess ticks
            for(int j = shellInstances.Count-1; j >= i; --j)
            {
                Destroy(shellInstances[j].transform.parent.gameObject);
                shellInstances.RemoveAt(j);
            }
        }
    }


    void _OnReload(ReloadStartedEvent e)
    {
        //reloadSprite.SetActive(false);
        if (gameObject.activeInHierarchy) StartCoroutine(ReloadOverTime());
    }

    IEnumerator ReloadOverTime()
    {
        reloading = true;
        yield return null;

        int toAdd = clipMax - curClip;

        for(int i = 0; i < toAdd; ++i)
        {
            if (swapped) break;


            AddBullet();

            yield return new WaitForSecondsRealtime(reloadTime / toAdd);
        }

        reloading = false;
    }

    // uses knowledge that knownClip is lower than desired amount of ammo, and that it should be the 
    // index of the next bullet image to be enabled
    private void AddBullet()
    {
        if (knownClip == clipMax || knownClip >= shellInstances.Count) return;

        shellInstances[knownClip++].SetActive(true);
    }

    // uses knowledge that knownClip is higher than desired amount of ammo to enable us
    // to remove all between the two indices
    private void RemoveBullets()
    {
        if (curClip == clipMax || knownClip <= curClip) return;

        for(int i = curClip; i < knownClip; ++i)
        {
            if (i > shellInstances.Count) return;

            shellInstances[i].SetActive(false);
        }
    }

    private void OnDestroy()
    {
        
        SceneManager.sceneLoaded -= OnSceneLoad;
    }
}

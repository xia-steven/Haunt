using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu_UI {
    public class AmmoUI : MonoBehaviour {
        [SerializeField] private Image gunImg;
        [SerializeField] private Transform tickHolder;
        [SerializeField] private GameObject bulletTick;
        [SerializeField] private GameObject reloadSprite;

        private Subscription<WeaponSwapEvent> swapSub;
        private Subscription<ReloadStartedEvent> reloadSub;

        private List<GameObject> shellInstances = new();

        private Weapon current;
        private int clipMax;
        private int curClip;
        private float reloadTime;

        private int knownClip;

        private bool reloading;
        private bool swapped;

        private void Awake() {
            swapSub = EventBus.Subscribe<WeaponSwapEvent>(_OnSwap);
            reloadSub = EventBus.Subscribe<ReloadStartedEvent>(_OnReload);
            SceneManager.sceneLoaded += OnSceneLoad;
        }

        private void OnSceneLoad(Scene s, LoadSceneMode m) {
            curClip = clipMax;
            UpdateBulletCounts();
        }


        // Update is called once per frame
        private void Update() {
            if (reloading || current == null) return;

            clipMax = current.FullClipAmount;
            curClip = current.CurrentClipAmount;

            switch (curClip) {
                case 0:
                    reloadSprite.SetActive(true);
                    break;
            }

            if (curClip < knownClip) RemoveBullets();

            knownClip = curClip;
            swapped = false;
        }

        private void _OnSwap(WeaponSwapEvent e) {
            switch (gameObject.activeInHierarchy) {
                case false:
                    return;
            }

            swapped = true;

            current = e.newWeapon;
            clipMax = current.FullClipAmount;
            curClip = current.CurrentClipAmount;
            reloadTime = current.ReloadTime;

            var weaponType = current.Type;

            gunImg.sprite = weaponType switch {
                "pistol" => Resources.LoadAll<Sprite>("Textures-Sprites/six_shooter")[1],
                "rifle" => Resources.LoadAll<Sprite>("Textures-Sprites/minigun")[0],
                "shotgun" => Resources.LoadAll<Sprite>("Textures-Sprites/shotguns")[1],
                "sniper" => Resources.LoadAll<Sprite>("Textures-Sprites/sniper")[0],
                "sword" => Resources.LoadAll<Sprite>("Textures-Sprites/swords")[1],
                _ => gunImg.sprite
            };

            gunImg.SetNativeSize();

            UpdateBulletCounts();
            knownClip = curClip;
        }

        private void UpdateBulletCounts() {
            var i = 0;

            // repurpose any already existing bullet ticks
            for (; i < clipMax && i < shellInstances.Count; ++i) shellInstances[i].SetActive(i < curClip);

            //if need more ticks
            if (i < clipMax)
                //make new ticks
                for (; i < clipMax; ++i) {
                    shellInstances.Add(Instantiate(bulletTick, tickHolder).transform.GetChild(1).gameObject);
                    if (i >= curClip)
                        shellInstances[i].SetActive(false);
                }
            //else if need less ticks
            else if (i < shellInstances.Count)
                //remove excess ticks
                for (var j = shellInstances.Count - 1; j >= i; --j) {
                    Destroy(shellInstances[j].transform.parent.gameObject);
                    shellInstances.RemoveAt(j);
                }
        }


        private void _OnReload(ReloadStartedEvent e) {
            reloadSprite.SetActive(false);
            switch (gameObject.activeInHierarchy) {
                case true:
                    StartCoroutine(ReloadOverTime());
                    break;
            }
        }

        private IEnumerator ReloadOverTime() {
            reloading = true;
            yield return null;

            var toAdd = clipMax - curClip;

            for (var i = 0; i < toAdd; ++i) {
                if (swapped) break;


                AddBullet();

                yield return new WaitForSecondsRealtime(reloadTime / toAdd);
            }

            reloading = false;
        }

        // uses knowledge that knownClip is lower than desired amount of ammo, and that it should be the
        // index of the next bullet image to be enabled
        private void AddBullet() {
            if (knownClip == clipMax || knownClip >= shellInstances.Count) return;

            shellInstances[knownClip++].SetActive(true);
        }

        // uses knowledge that knownClip is higher than desired amount of ammo to enable us
        // to remove all between the two indices
        private void RemoveBullets() {
            if (curClip == clipMax || knownClip <= curClip) return;

            for (var i = curClip; i < knownClip; ++i) {
                if (i > shellInstances.Count) return;

                shellInstances[i].SetActive(false);
            }
        }

        private void OnDestroy() {
            SceneManager.sceneLoaded -= OnSceneLoad;
        }
    }
}
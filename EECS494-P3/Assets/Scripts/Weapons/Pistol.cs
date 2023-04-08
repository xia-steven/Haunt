using System.Collections;
using Events;
using Player;
using UnityEngine;

namespace Weapons {
    public class Pistol : Weapon {
        private GameObject wielder;
        protected GameObject basicBullet;
        [SerializeField] protected float bulletSpread = 10f;

        // Sprite of pistol - necessary so it can be flipped
        [SerializeField] protected GameObject pistolSprite;

        protected override void Awake() {
            base.Awake();
            thisData = typesData.types[(int)WeaponType.pistol];

            SetData();
            currentClipAmount = fullClipAmount;

            Subscribe();

            spriteRenderer = pistolSprite.GetComponent<SpriteRenderer>();
            wielder = transform.parent.gameObject;
            basicBullet = Resources.Load<GameObject>("Prefabs/Weapons/BasicBullet");
        }

        protected override void _OnFire(FireEvent e) {
            switch (gameObject.activeInHierarchy) {
                case false:
                    return;
            }

            // Check if fire event comes from pistol holder
            if (e.shooter != wielder) return;

            base._OnFire(e);
        }

        protected override void _OnReload(ReloadEvent e) {
            switch (gameObject.activeInHierarchy) {
                case false:
                    return;
            }

            // Check if reload event comes from pistol holder
            if (e.reloader != wielder) return;

            if (CanReload()) StartCoroutine(ReloadDelay());
        }


        private IEnumerator ReloadDelay() {
            isReloading = true;
            Debug.Log("Reloading");

            EventBus.Publish(new ReloadStartedEvent(reloadTime * PlayerModifiers.reloadSpeed));
            yield return new WaitForSeconds(reloadTime * PlayerModifiers.reloadSpeed);

            // TODO: change to line up with inventory ammo
            ReloadInfinite();
            Debug.Log("Pistol ammo: " + currentClipAmount);
            isReloading = false;
        }

        protected override void WeaponFire(Vector3 direction) {
            direction.y = 0;
            direction = direction.normalized;

            var spread = Quaternion.AngleAxis(Random.Range(-bulletSpread, bulletSpread) / 2f, Vector3.up);

            var randomDirection = spread * direction;
            randomDirection = randomDirection.normalized;

            FireProjectile(basicBullet, randomDirection, transform, BasicBullet.bulletSpeed, Shooter.Player);
            // Give the player unlimited ammo for now
            currentClipAmount--;

            lastBullet = Time.time;
            lastTap = Time.time;

            // Shake screen
            EventBus.Publish(new ScreenShakeEvent(screenShakeStrength));
        }

        protected override void GunReload() {
            if (CanReload()) StartCoroutine(ReloadDelay());
        }

        private void OnDestroy() {
            UnSubscribe();
        }
    }
}
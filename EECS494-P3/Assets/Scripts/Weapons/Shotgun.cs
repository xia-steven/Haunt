using System.Collections;
using Events;
using Player;
using UnityEngine;

namespace Weapons {
    public class Shotgun : Weapon {
        private GameObject wielder;
        private GameObject shotgunBullet;

        [SerializeField] protected GameObject shotgunSprite;

        // Bullet spread in degrees
        [SerializeField] protected float spread = 30f;


        protected override void Awake() {
            base.Awake();
            thisData = typesData.types[(int)WeaponType.shotgun];

            SetData();
            currentClipAmount = fullClipAmount;

            Subscribe();

            spriteRenderer = shotgunSprite.GetComponent<SpriteRenderer>();
            wielder = transform.parent.gameObject;
            shotgunBullet = Resources.Load<GameObject>("Prefabs/Weapons/ShotgunBullet");
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
            Debug.Log("Shotgun ammo: " + currentClipAmount);
            isReloading = false;
        }

        protected override void WeaponFire(Vector3 direction) {
            // Fires shotgun bullets in direction shotgun is facing

            direction.y = 0;
            var straight = direction.normalized;

            var rotation1 = Quaternion.AngleAxis(-spread / 2f, Vector3.up);
            var rotation2 = Quaternion.AngleAxis(-spread / 4f, Vector3.up);
            var rotation3 = Quaternion.AngleAxis(spread / 4f, Vector3.up);
            var rotation4 = Quaternion.AngleAxis(spread / 2f, Vector3.up);

            var bullet1 = rotation1 * direction;
            var bullet2 = rotation2 * direction;
            var bullet3 = rotation3 * direction;
            var bullet4 = rotation4 * direction;

            bullet1.Normalize();
            bullet2.Normalize();
            bullet3.Normalize();
            bullet4.Normalize();

            FireProjectile(shotgunBullet, straight, transform, ShotgunBullet.bulletSpeed, Shooter.Player);
            FireProjectile(shotgunBullet, bullet1, transform, ShotgunBullet.bulletSpeed, Shooter.Player);
            FireProjectile(shotgunBullet, bullet2, transform, ShotgunBullet.bulletSpeed, Shooter.Player);
            FireProjectile(shotgunBullet, bullet3, transform, ShotgunBullet.bulletSpeed, Shooter.Player);
            FireProjectile(shotgunBullet, bullet4, transform, ShotgunBullet.bulletSpeed, Shooter.Player);

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
using System;
using System.Collections;
using Events;
using Pedestal;
using Player;
using UnityEngine;

namespace Weapons {
    public class Sniper : Weapon {
        private GameObject wielder;
        private GameObject lastHit;

        // The length of the raycast
        private const float raycastLength = 100f;
        private LineRenderer lineRenderer;
        private const int damage = -5;
        private bool isFiring;

        // Sprite of pistol - necessary so it can be flipped
        [SerializeField] protected GameObject sniperSprite;
        [SerializeField] protected Color laserColor;
        [SerializeField] protected Color flashColor;
        [SerializeField] protected Color reloadColor;

        private float pierce = 1;
        private RaycastHit[] hits;

        protected override void Awake() {
            base.Awake();
            thisData = typesData.types[(int)WeaponType.sniper];

            SetData();
            currentClipAmount = fullClipAmount;
            pierce += PlayerModifiers.maxPierce;

            Subscribe();

            spriteRenderer = sniperSprite.GetComponent<SpriteRenderer>();
            wielder = transform.parent.gameObject;

            lineRenderer = gameObject.GetComponent<LineRenderer>();

            // Set the width and color of the line
            lineRenderer.startWidth = 0.04f;
            lineRenderer.endWidth = 0.04f;
            lineRenderer.material.color = laserColor;
        }

        // Need to add extra elements for the sniper fixed update such as the scope line
        protected override void FixedUpdate() {
            base.FixedUpdate();

            gunDirection.y = 0;
            gunDirection = gunDirection.normalized;

            // Set spawn position based on barrel length
            var barrelOffset = gunDirection * barrelLength;
            var barrelSpawn = transform.position + barrelOffset;

            var raycastSpawn = barrelSpawn;
            raycastSpawn.y = 0.5f;

            var layerMask = ~(1 << LayerMask.NameToLayer("Special"));

            // Get all hits of raycast
            hits = Physics.RaycastAll(raycastSpawn, transform.forward, layerMask, 100);

            // Set the positions of the LineRenderer to draw a line from the current position to the point of intersection
            if (Physics.Raycast(raycastSpawn, transform.forward, out var hit, raycastLength, layerMask)) {
                // Set laser
                lineRenderer.SetPositions(new[] { barrelSpawn, hit.point });

                // Sort hit array (now that we know objects were hit)
                Array.Sort(hits,
                    (x, y) => Vector3.Distance(raycastSpawn, x.point).CompareTo(Vector3.Distance(raycastSpawn, y.point)));
                lastHit = hit.collider.gameObject;
            }
            else {
                // If the raycast doesn't hit anything, draw a line for the entire length of the ray
                lineRenderer.SetPositions(new[]
                    { barrelSpawn, transform.position + transform.forward * raycastLength });
            }
        }

        protected override void _OnFire(FireEvent e) {
            switch (gameObject.activeInHierarchy) {
                case false:
                    return;
            }

            // Check if fire event comes from sniper holder
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
            lineRenderer.material.color = reloadColor;
            Debug.Log("Reloading");

            EventBus.Publish(new ReloadStartedEvent(reloadTime));
            yield return new WaitForSeconds(reloadTime);

            // TODO: change to line up with inventory ammo
            ReloadInfinite();
            Debug.Log("Sniper ammo: " + currentClipAmount);
            lineRenderer.material.color = laserColor;
            isReloading = false;
        }

        protected override void WeaponFire(Vector3 direction) {
            currentClipAmount--;

            // Double check pierce amount (sniper can always pierce one)
            pierce = PlayerModifiers.maxPierce + 1;

            StartCoroutine(SniperFire());

            lastBullet = Time.time;
            lastTap = Time.time;
        }

        protected override void GunReload() {
            if (CanReload()) StartCoroutine(ReloadDelay());
        }

        private IEnumerator SniperFire() {
            isFiring = true;
            lineRenderer.material.color = flashColor;
            lineRenderer.endWidth = 0.06f;

            Debug.Log("Beginning of loop");
            for (var i = 0; i < pierce; i++) {
                if (i >= hits.Length) break;

                // Alter pedestal health if collided is pedestal and shot by player
                var pedHealth = hits[i].collider.gameObject.GetComponent<HasPedestalHealth>();
                if (pedHealth != null) {
                    pedHealth.AlterHealth(-damage);
                    // Pedestals count as "buildings" so don't pierce
                    break;
                }

                // Alter health if collided has health
                var health = hits[i].collider.gameObject.GetComponent<HasHealth>();
                if (health != null && pedHealth == null) {
                    Debug.Log("Health altered");
                    health.AlterHealth(damage);
                }

                // Hit non-pedestal and non-enemy (most likely wall) so don't allow pierce
                if (health == null && pedHealth == null) break;
            }

            Debug.Log("Got to end of loop");

            yield return new WaitForSeconds(0.1f);

            Debug.Log("Beginning reload and line fix");
            lineRenderer.endWidth = 0.04f;
            isFiring = false;
            GunReload();

            // Shake screen
            EventBus.Publish(new ScreenShakeEvent(screenShakeStrength));
        }

        private void OnDestroy() {
            UnSubscribe();
        }

        protected override void OnDisable() {
            if (isReloading || isFiring) {
                lineRenderer.material.color = laserColor;
                lineRenderer.endWidth = 0.04f;
            }

            base.OnDisable();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using Events;
using Pedestal;
using UnityEngine;

namespace Enemy {
    public class TutorialClericEnemy : EnemyBase {
        public static Dictionary<int, Vector3> pedestalPositions = new() { { 1, new Vector3(8, 0, -9) } };

        private Subscription<PedestalDestroyedEvent> switchPedestalSub;
        private Subscription<PedestalRepairedEvent> addPedestalSub;

        private float prevTime;
        public int pedestalTimeout;

        private new void Start() {
            base.Start();
            baseSpeed = 1.5f;
            switchPedestalSub = EventBus.Subscribe<PedestalDestroyedEvent>(pedestalDied);
            addPedestalSub = EventBus.Subscribe<PedestalRepairedEvent>(pedestalRepaired);
            StartCoroutine(WaitAndFindPath());
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.layer == LayerMask.NameToLayer("Pedestal")) {
                var h = other.gameObject.GetComponent<HasPedestalHealth>();
                if (h != null) h.AlterHealth(-5000);
            }
            else if (!other.CompareTag("Player")) {
                //base.OnTriggerEnter(other);
            }
        }

        private int findClosestPedestal() {
            var closestDist = float.MaxValue;
            var closest = 1;
            foreach (var ped in pedestalPositions) {
                var distance = Vector3.Distance(transform.position, ped.Value);
                if (distance < closestDist) {
                    closestDist = distance;
                    closest = ped.Key;
                }
            }

            return closest;
        }

        private IEnumerator WaitAndFindPath() {
            yield return null;
            SetTargetPosition(pedestalPositions[findClosestPedestal()]);
        }

        private void SetTargetPosition(Vector3 pos) {
            currentPathIndex = 0;
            pathVectorList = Pathfinding.AStar.Instance.FindPath(transform.position, pos);

            if (pathVectorList != null && pathVectorList.Count > 1) pathVectorList.RemoveAt(0);
        }

        private IEnumerator pedetalCoroutine(int uuid) {
            yield return new WaitForSeconds(pedestalTimeout);
            SetTargetPosition(pedestalPositions[findClosestPedestal()]);
        }

        private void pedestalDied(PedestalDestroyedEvent event_) {
            pedestalPositions[1] = event_.pedestalUUID switch {
                1 => new Vector3(23, 0, -9),
                _ => pedestalPositions[1]
            };

            StartCoroutine(pedetalCoroutine(event_.pedestalUUID));
        }

        private void pedestalRepaired(PedestalRepairedEvent event_) {
            StartCoroutine(pedetalCoroutine(event_.pedestalUUID));
        }
    }
}
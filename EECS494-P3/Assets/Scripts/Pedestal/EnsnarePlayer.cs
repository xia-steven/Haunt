using System.Collections;
using Events;
using Player;
using UnityEngine;

namespace Pedestal {
    public class EnsnarePlayer : MonoBehaviour {
        private LineRenderer rend;
        private Transform playerShadow;
        private bool flameLit;

        private Subscription<PedestalDestroyedEvent> ped_dest_event;
        private Subscription<PedestalRepairedEvent> ped_rep_event;

        // Start is called before the first frame update
        private void Start() {
            EventBus.Subscribe<PedestalDestroyedEvent>(_OnPedestalDestroyed);
            EventBus.Subscribe<PedestalRepairedEvent>(_OnPedestalRepaired);


            rend = GetComponent<LineRenderer>();
            for (var i = 0; i < IsPlayer.instance.transform.childCount; i++) {
                var child = IsPlayer.instance.transform.GetChild(i);
                playerShadow = child.name switch {
                    "Shadow" => child,
                    _ => playerShadow
                };
            }

            // Initially have the line renderer disabled
            rend.enabled = false;
        }

        // Update is called once per frame
        private void Update() {
            switch (flameLit) {
                case true:
                    rend.SetPositions(new[] { transform.position, playerShadow.position });
                    break;
            }
        }

        private void _OnPedestalDestroyed(PedestalDestroyedEvent e) {
            if (e.pedestalUUID == GetComponent<IsPedestal>().getUUID()) {
                flameLit = false;
                rend.enabled = false;
            }
        }

        private void _OnPedestalRepaired(PedestalRepairedEvent e) {
            Debug.Log(e.pedestalUUID + " == " + GetComponent<IsPedestal>().getUUID());
            if (e.pedestalUUID == GetComponent<IsPedestal>().getUUID()) {
                rend.enabled = true;
                StartCoroutine(GrowTowardPlayer());
            }
        }

        private IEnumerator GrowTowardPlayer() {
            var t = 0f;
            while (t < 1f) {
                t += Time.deltaTime;
                rend.SetPositions(new[]
                    { transform.position, Vector3.Lerp(transform.position, playerShadow.position, t) });
                yield return null;
            }

            flameLit = true;
        }

        private void OnDestroy() {
            EventBus.Unsubscribe(ped_dest_event);
            EventBus.Unsubscribe(ped_rep_event);
        }
    }
}
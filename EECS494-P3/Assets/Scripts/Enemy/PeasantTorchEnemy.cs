using System.Collections;
using Enemy.Weapons;
using Player;
using UnityEngine;

namespace Enemy {
    public class PeasantTorchEnemy : EnemyBase {
        private GameObject torchObject;

        // Override enemy ID to load from config
        protected override int GetEnemyID() {
            return 0;
        }

        protected override void Start() {
            base.Start();
            torchObject = GetComponentInChildren<Torch>().gameObject;
            torchObject.SetActive(false);
        }

        // Override attack function
        protected override IEnumerator EnemyAttack() {
            var direction = (IsPlayer.instance.transform.position - transform.position).normalized;

            // Calculate the rotation that points in the direction of the intersection point
            var rotation = Quaternion.LookRotation(direction, Vector3.up);


            // Set the rotation of the knife
            torchObject.transform.rotation = rotation;
            torchObject.transform.Rotate(0, 90, 0);


            // While attacking
            while (state == EnemyState.Attacking) {
                torchObject.SetActive(true);
                yield return new WaitForSeconds(attributes.swingTime);
                torchObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, -10, 0);
                yield return new WaitForSeconds(attributes.swingTime);
                torchObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                yield return new WaitForSeconds(attributes.swingTime);
                torchObject.SetActive(false);
                yield return new WaitForSeconds(attributes.attackSpeed);
            }


            // Let update know that we're done
            runningCoroutine = false;
        }
    }
}
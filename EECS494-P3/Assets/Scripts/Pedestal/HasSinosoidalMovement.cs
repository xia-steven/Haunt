using UnityEngine;

namespace Pedestal {
    public class HasSinosoidalMovement : MonoBehaviour {
        [Header("0 = x, 1 = y, 2 = z")] [Range(0, 2)] [SerializeField]
        private int direction;

        [SerializeField] private float magnitude = 5f;
        [SerializeField] private bool reverseDirection;

        private float initialTime;

        private void Start() {
            initialTime = Time.time;
        }


        // Update is called once per frame
        private void Update() {
            var movementAmount = reverseDirection
                ? -Mathf.Cos(Time.time - initialTime) * magnitude * Time.deltaTime
                : Mathf.Cos(Time.time - initialTime) * magnitude * Time.deltaTime;
            var amountToMove = transform.position;
            switch (direction) {
                // x
                case 0:
                    amountToMove.x += movementAmount;
                    break;
                // y
                case 1:
                    amountToMove.y += movementAmount;
                    break;
                // z
                case 2:
                    amountToMove.z += movementAmount;
                    break;
            }

            transform.position = amountToMove;
        }
    }
}
using UnityEngine;

namespace Game_Control {
    public class IsWaveMember : MonoBehaviour {
        private Wave owner;
        private int id;

        public void Init(Wave newOwner, int _id) {
            owner = newOwner;
            id = _id;
        }

        private void OnDestroy() {
            owner?.LoseMember(id);
        }
    }
}
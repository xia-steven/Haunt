using Player;
using UnityEngine;

namespace Hub.Upgrades {
    public class ReflectUpgrade : Upgrade {
        protected override void Start() {
            thisData = typesData.types[(int)PurchaseableType.dashReflect];
            base.Start();
        }

        protected override void Apply() {
            Debug.Log("Applying ReflectUpgrade");
            IsPlayer.instance.gameObject.AddComponent<HasReflectUpgrade>();

            base.Apply();
        }
    }
}
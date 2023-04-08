using Player;

namespace Hub.Upgrades {
    public class ExplodeUpgrade : Upgrade {
        protected override void Start() {
            thisData = typesData.types[(int)PurchaseableType.dashExplode];
            base.Start();
        }

        protected override void Apply() {
            var newInstance = IsPlayer.instance.gameObject.AddComponent<HasExplodeUpgrade>();
            newInstance.explosiveRadius = thisData.rate1;

            base.Apply();
        }
    }
}
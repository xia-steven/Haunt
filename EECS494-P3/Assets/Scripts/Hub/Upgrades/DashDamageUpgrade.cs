using Player;

namespace Hub.Upgrades {
    public class DashDamageUpgrade : Upgrade {
        protected override void Start() {
            thisData = typesData.types[(int)PurchaseableType.doubleDashDamage];
            base.Start();
        }

        protected override void Apply() {
            var newInstance = IsPlayer.instance.gameObject.AddComponent<HasDashDamageUpgrade>();
            newInstance.cooldown = thisData.duration;
            newInstance.dmgMod = thisData.rate1;
            base.Apply();
        }
    }
}
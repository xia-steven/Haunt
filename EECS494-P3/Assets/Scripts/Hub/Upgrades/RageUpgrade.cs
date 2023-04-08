using Player;

namespace Hub.Upgrades {
    public class RageUpgrade : Upgrade {
        protected override void Start() {
            thisData = typesData.types[(int)PurchaseableType.damageRage];
            base.Start();
        }

        protected override void Apply() {
            var newInstance = IsPlayer.instance.gameObject.AddComponent<HasRageUpgrade>();
            newInstance.duration = thisData.duration;
            newInstance.dmgMod = thisData.rate1;
            newInstance.moveMod = thisData.rate2;

            base.Apply();
        }
    }
}
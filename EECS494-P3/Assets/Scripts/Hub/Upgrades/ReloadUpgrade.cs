using Player;

namespace Hub.Upgrades {
    public class ReloadUpgrade : Upgrade {
        protected override void Start() {
            thisData = typesData.types[(int)PurchaseableType.fastReload];
            base.Start();
        }

        protected override void Apply() {
            PlayerModifiers.reloadSpeed /= thisData.rate1;

            base.Apply();
        }
    }
}
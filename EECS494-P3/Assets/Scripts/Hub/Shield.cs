using Player;

namespace Hub {
    public class Shield : IsBuyable {
        protected override void Apply() {
            IsPlayer.instance.GetComponent<PlayerHasHealth>().AddShield();
        }
    }
}
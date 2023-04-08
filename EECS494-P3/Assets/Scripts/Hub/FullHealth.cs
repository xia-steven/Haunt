using Player;

namespace Hub {
    public class FullHealth : IsBuyable {
        protected override void Apply() {
            // overflowing the player health will ensure they get to max
            IsPlayer.instance.GetComponent<PlayerHasHealth>().AlterHealth(6);
        }
    }
}
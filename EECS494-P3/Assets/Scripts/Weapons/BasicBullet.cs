// Most simple bullet

namespace Weapons {
    public class BasicBullet : Bullet {
        public const float bulletSpeed = 10;

        protected override void Awake() {
            base.Awake();
            damage = -1;
            bulletLife = 2.5f;
        }
    }
}
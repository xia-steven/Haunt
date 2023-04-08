

// Most simple enemy bullet

namespace Weapons {
    public class EnemyBasicBullet : Bullet {
        public const float bulletSpeed = 4;

        protected override void Awake() {
            base.Awake();
            damage = -1;
            bulletLife = 3.0f;
        }

        public void setLifetime(float lifetime) {
            bulletLife = lifetime;
        }
    }
}
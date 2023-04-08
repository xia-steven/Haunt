namespace Weapons {
    public class ShotgunBullet : Bullet {
        public const float bulletSpeed = 6;

        protected override void Awake() {
            base.Awake();
            damage = -1;
            bulletLife = 1.5f;
        }
    }
}
using Bremsengine;
using UnityEngine;

namespace BremseTouhou
{
    public class CheeseOutcome : WheelOutcome
    {
        [SerializeField] ProjectileSO cheeseProjectile;
        public override void ApplyEffect(BaseUnit unit)
        {
            Projectile.SpawnProjectile(cheeseProjectile, null, unit.Center + new Vector2(0f, 20f), new ProjectileDirection(cheeseProjectile, Vector2.down), OnProjectileSpawn, unit.transform);
        }
        private void OnProjectileSpawn(Projectile projectile, Transform target, Transform owner)
        {
            projectile.SetFaction(BremseFaction.Enemy);
            if (projectile.transform.GetComponent<Rigidbody2D>() is Rigidbody2D rb and not null)
            {
                rb.angularVelocity = Random.Range(100f, -100f);
            }
        }
        public override void RemoveEffect(BaseUnit unit)
        {

        }

        public override float GetDuration()
        {
            return 0f;
        }

        public override void GameReset(BaseUnit unit)
        {

        }
    }
}
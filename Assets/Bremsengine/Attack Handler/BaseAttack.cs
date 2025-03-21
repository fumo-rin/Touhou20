using UnityEngine;

namespace Bremsengine
{
    public abstract class BaseAttack : MonoBehaviour
    {
        public AttackHandler Handler => attackHandler;
        [SerializeField] protected AttackHandler attackHandler;
        [SerializeField] UnitDamageScaler scaler;
        [SerializeField] private float attackBaseDamage = 10f;
        public abstract float GetAttackCooldown();
        public abstract void PerformAttack(AttackDirectionPacket packet);
        protected void OnProjectileSpawn(Projectile p, Transform owner, Transform target)
        {
            p.SetFaction(attackHandler.Faction);
            if (scaler != null)
            {
                p.SetDamage(scaler.DamageScale * attackBaseDamage);
            }
            else
            {
                p.SetDamage(attackBaseDamage);
            }
        }
    }
}

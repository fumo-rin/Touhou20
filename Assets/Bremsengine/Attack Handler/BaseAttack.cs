using UnityEngine;

namespace Bremsengine
{
    public abstract class BaseAttack : MonoBehaviour
    {
        [SerializeField] protected AttackHandler attackHandler;
        [SerializeField] private float attackBaseDamage = 10f;
        public abstract float GetAttackCooldown();
        public abstract void PerformAttack(AttackDirectionPacket packet);
        protected void OnProjectileSpawn(Projectile p, Transform owner, Transform target)
        {
            p.SetFaction(attackHandler.Faction);
            if (owner.GetComponent<UnitDamageScaler>() is not null and UnitDamageScaler scaler)
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

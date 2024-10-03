using UnityEngine;

namespace Bremsengine
{
    public class EnemyAttackHandler : AttackHandler
    {
        [SerializeField] Vector2 fallbackDirection = new(0f,-1f);
        [SerializeField] BaseAttack TESTINGAttack;
        protected override BaseAttack CollapseBaseAttack()
        {
            return TESTINGAttack;
        }
        public override bool CanAttack()
        {
            if (Projectile.PlayerBombedRecently)
            {
                return false;
            }
            return Time.time >= nextAttackTime;
        }
        public override void TriggerAttack(BaseAttack attack)
        {
            if (!CanAttack())
            {
                return;
            }
            SetNextAttackDelay(attack.attackDuration);
            AttackDirectionPacket p = new(owner, assignedTarget);
            p.SetAimDirectionOverride(fallbackDirection);
            attack.PerformAttack(p);
        }

    }
}

using UnityEngine;

namespace Bremsengine
{
    public class EnemyAttackHandler : AttackHandler
    {
        [SerializeField] Vector2 fallbackDirection = new(0f,-1f);
        [SerializeField] BaseAttack TESTINGAttack;
        private Bounds worldBounds;
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
            if (!worldBounds.Contains(owner.transform.position))
            {
                return false;
            }
            if (!UnitAlive)
                return false;
            return Time.time >= nextAttackTime;
        }

        protected override void WhenStart()
        {
            worldBounds = DirectionSolver.GetPaddedBounds(0);
        }
        public override void TriggerAttack(BaseAttack attack)
        {
            if (!CanAttack())
            {
                return;
            }
            ForceAttack(attack);
        }
        public override void ForceAttack(BaseAttack attack)
        {
            SetNextAttackDelay(attack.GetAttackCooldown());
            AttackDirectionPacket p = new(owner, assignedTarget);
            p.SetAimDirectionOverride(fallbackDirection);
            attack.PerformAttack(p);
        }
    }
}

using UnityEngine;

namespace Bremsengine
{
    public class EnemyAttackHandler : AttackHandler
    {
        float nextAttackTime;
        public Transform testingTarget;
        [SerializeField] Vector2 fallbackDirection = new(0f,-1f);
        public override bool CanAttack()
        {
            return Time.time >= nextAttackTime;
        }

        public override void TriggerAttack(BaseAttack attack)
        {
            nextAttackTime = Time.time + attack.attackDuration;
            AttackDirectionPacket p = new(owner, testingTarget);
            p.SetAimDirectionOverride(fallbackDirection);
            attack.PerformAttack(p);
        }
    }
}

using UnityEngine;

namespace Bremsengine
{
    public class EnemyAttackHandler : AttackHandler
    {
        float nextAttackTime;
        public Transform testingTarget;
        public override bool CanAttack()
        {
            return Time.time >= nextAttackTime;
        }

        public override void TriggerAttack(BaseAttack attack)
        {
            nextAttackTime = Time.time + attack.attackDuration;
            AttackDirectionPacket p = new(owner, testingTarget, new(0f, 0.5f));
            p.SetTargetPositionOverride(testingTarget.position);
            p.SetAimOffset(new(0f,0.5f));
            attack.PerformAttack(p);
        }
    }
}

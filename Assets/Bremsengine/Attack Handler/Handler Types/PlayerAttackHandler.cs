using Core.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bremsengine
{
    public class PlayerAttackHandler : AttackHandler
    {
        [SerializeField] BaseAttack TESTINGcurrentAttack;
        [SerializeField] Vector2 overrideDirection;
        bool attackHeld;
        protected override BaseAttack CollapseBaseAttack()
        {
            return TESTINGcurrentAttack;
        }
        public override bool CanAttack()
        {
            return Time.time >= nextAttackTime;
        }
        private void Update()
        {
            if (attackHeld)
            {
                TriggerAttack(ContainedAttack);
            }
        }
        public void SetAttackPressed(bool state)
        {
            attackHeld = state;
        }
        public override void TriggerAttack(BaseAttack attack)
        {
            if (!CanAttack())
            {
                return;
            }
            SetNextAttackDelay(attack.attackDuration);
            AttackDirectionPacket packet = new(owner, null);
            if (overrideDirection != Vector2.zero)
            {
                packet.SetAimDirectionOverride(overrideDirection);
            }
            attack.PerformAttack(packet);
        }
    }
}

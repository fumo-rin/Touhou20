using Core.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bremsengine
{
    public class PlayerAttackHandler : AttackHandler
    {
        [SerializeField] BaseAttack TESTINGcurrentAttack;
        bool attackHeld;
        protected override BaseAttack CollapseBaseAttack()
        {
            return TESTINGcurrentAttack;
        }
        public override bool CanAttack()
        {
            if (!UnitAlive)
                return false;
            return Time.time >= nextAttackTime;
        }
        private void Update()
        {
            if (!attackHeld && Gamepad.current != null)
            {
                attackHeld = Gamepad.current.buttonSouth.ReadValue() > 0.5f;
            }
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
            ForceAttack(attack);
        }
        public override void ForceAttack(BaseAttack attack)
        {
            SetNextAttackDelay(attack.GetAttackCooldown());
            AttackDirectionPacket packet = new(owner, null);
            attack.PerformAttack(packet);
        }

        protected override void WhenStart()
        {

        }
    }
}

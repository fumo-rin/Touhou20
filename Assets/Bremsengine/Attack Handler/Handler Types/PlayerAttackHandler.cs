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
            if (Dialogue.IsDialogueRunning)
                return false;
            return Time.time >= nextAttackTime;
        }
        private void Update()
        {
            if (attackHeld || (Gamepad.current != null && Gamepad.current.buttonSouth.ReadValue() > 0.5f))
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
        public void SetPressed(InputAction.CallbackContext c)
        {
            attackHeld = true;
        }
        public void UnsetPressed(InputAction.CallbackContext c)
        {
            attackHeld = false;
        }
        public override void ForceAttack(BaseAttack attack)
        {
            SetNextAttackDelay(attack.GetAttackCooldown());
            AttackDirectionPacket packet = new(owner, null);
            attack.PerformAttack(packet);
        }

        protected override void WhenStart()
        {
            PlayerInputController.actions.Shmup.Fire.started += SetPressed;
            PlayerInputController.actions.Shmup.Fire.canceled += UnsetPressed;
            
        }
        private void OnDestroy()
        {
            PlayerInputController.actions.Shmup.Fire.started -= SetPressed;
            PlayerInputController.actions.Shmup.Fire.canceled -= UnsetPressed;
        }
    }
}

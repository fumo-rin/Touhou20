using Core.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bremsengine
{
    public class PlayerAttackHandler : AttackHandler
    {
        [SerializeField] Rigidbody2D movementRigidbody;
        float nextAttackTime;
        [SerializeField] float TESTINGattackDelay = 0.6f;
        [SerializeField] BaseAttack TESTINGcurrentAttack;
        [SerializeField] Vector2 overrideDirection;
        bool attackHeld;
        Vector2 lastVelocity;
        Vector2 GetAttackDirection => overrideDirection == Vector2.zero ? (lastVelocity == Vector2.zero ? Vector2.right : lastVelocity) : overrideDirection;
        public override bool CanAttack()
        {
            return Time.time >= nextAttackTime;
        }
        private void Update()
        {
            if (movementRigidbody.linearVelocity != Vector2.zero)
            {
                lastVelocity = movementRigidbody.linearVelocity;
            }
            if (attackHeld)
            {
                if (CanAttack())
                {
                    nextAttackTime = Time.time + TESTINGattackDelay;
                    TriggerAttack(TESTINGcurrentAttack);
                }
            }
        }
        private void Start()
        {
            PlayerInputController.actions.Player.Fire.performed += OnAttackInput;
            PlayerInputController.actions.Player.Fire.canceled += OnAttackInput;
        }
        private void OnDestroy()
        {
            PlayerInputController.actions.Player.Fire.performed -= OnAttackInput;
            PlayerInputController.actions.Player.Fire.canceled -= OnAttackInput;
        }
        private void OnAttackInput(InputAction.CallbackContext c)
        {
            switch (c.phase)
            {
                case InputActionPhase.Disabled:
                    break;
                case InputActionPhase.Waiting:
                    break;
                case InputActionPhase.Started:
                    break;
                case InputActionPhase.Performed:
                    attackHeld = true;
                    break;
                case InputActionPhase.Canceled:
                    attackHeld = false;
                    break;
                default:
                    break;
            }
        }
        public override void TriggerAttack(BaseAttack attack)
        {
            AttackDirectionPacket packet = new(owner, null);
            if (overrideDirection != Vector2.zero)
            {
                packet.SetAimDirectionOverride(overrideDirection);
            }
            attack.PerformAttack(packet);
        }
    }
}

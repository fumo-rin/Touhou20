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
        Vector2 lastVelocity;
        Vector2 GetAttackDirection => lastVelocity == Vector2.zero ? Vector2.right : lastVelocity;
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
        }
        private void Start()
        {
            PlayerInputController.actions.Player.Fire.performed += OnAttackInput;
        }
        private void OnDestroy()
        {
            PlayerInputController.actions.Player.Fire.performed -= OnAttackInput;
        }
        private void OnAttackInput(InputAction.CallbackContext c)
        {
            if (CanAttack())
            {
                nextAttackTime = Time.time + TESTINGattackDelay;
                TriggerAttack(TESTINGcurrentAttack);
            }
        }
        public override void TriggerAttack(BaseAttack attack)
        {
            AttackDirectionPacket packet = new(owner, null, ownerAttackOffset);
            packet.SetTargetPositionOverride((Vector2)owner.position +  GetAttackDirection);
            attack.PerformAttack(packet);
        }
    }
}

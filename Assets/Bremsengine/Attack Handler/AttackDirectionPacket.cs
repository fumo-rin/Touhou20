using UnityEngine;

namespace Bremsengine
{
    public class AttackDirectionPacket
    {
        public AttackDirectionPacket(Transform owner, Transform target, Vector2 attackOffset)
        {
            Owner = owner;
            Target = target;
            OwnerAttackOffset = attackOffset;
            aimOffset = Vector2.zero;
            targetPositionOverride = Vector2.zero;
        }
        public Transform Owner;
        public Transform Target;
        public Vector2 OwnerAttackOffset;
        private Vector2 aimOffset;
        private Vector2 targetPositionOverride;
        private Vector2 OwnerPosition => (Vector2)Owner.transform.position + OwnerAttackOffset;
        public Vector2 AimTarget => targetPositionOverride != Vector2.zero ? targetPositionOverride : (Vector2)Target.position + aimOffset;
        public void SetTargetPositionOverride(Vector2 position) => targetPositionOverride = position;
        public void SetAimOffset(Vector2 offset) => aimOffset = offset;
    }
}
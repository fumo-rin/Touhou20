using UnityEngine;

namespace Bremsengine
{
    public class AttackDirectionPacket
    {
        public AttackDirectionPacket(Transform owner, Transform target)
        {
            Owner = owner;
            Target = target;
            targetPositionOverride = Vector2.zero;
        }
        public Transform Owner;
        public Transform Target;
        private Vector2 targetPositionOverride;
        public Vector2 aimDirectionOverride { get; private set; }
        private Vector2 prioritizeAimDirection => aimDirectionOverride == Vector2.zero ? (targetPositionOverride == Vector2.zero ? (Target == null ? Vector2.down : (Vector2)Target.position) : targetPositionOverride) : aimDirectionOverride;
        public Vector2 AimTarget => prioritizeTarget;
        private Vector2 prioritizeTarget => Target == null ? (aimDirectionOverride == Vector2.zero ? (Vector2.down) : aimDirectionOverride) : (Vector2)Target.position;
        public void SetAimDirectionOverride(Vector2 direction)
        {
            aimDirectionOverride = direction;
        }
        public void SetTargetPositionOverride(Vector2 position) => targetPositionOverride = position;
        public ProjectileGraphInput ToGraphInput()
        {
            ProjectileGraphInput input = new ProjectileGraphInput(Owner, Target);
            input.SetOverrideTarget(AimTarget);
            return input;
        }
    }
}
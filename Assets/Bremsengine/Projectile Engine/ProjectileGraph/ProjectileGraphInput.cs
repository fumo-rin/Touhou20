using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bremsengine
{
    public struct ProjectileGraphInput
    {
        public ProjectileGraphInput(Transform owner, Transform target)
        {
            this.Owner = owner;
            this.Target = target;
            addedAngle = 0f;
            overrideTarget = Vector2.zero;
            ownerSpawnOffset = Vector2.zero;
        }
        public Vector2 OverrideTargetPosition => overrideTarget != Vector2.zero ? overrideTarget : Target == null ? (Vector2)Owner.transform.position + Vector2.down : Target.position;
        public Transform Target;
        public Transform Owner;
        public float addedAngle;
        private Vector2 overrideTarget;
        private Vector2 ownerSpawnOffset;
        public void SetOwnerSpawnOffset(Vector2 offset) => ownerSpawnOffset = offset;
        public void SetOverrideTarget(Vector2 position) { overrideTarget = position; }
        public Vector2 OwnerCurrentPosition => (Vector2)Owner.position + ownerSpawnOffset;
    }
}

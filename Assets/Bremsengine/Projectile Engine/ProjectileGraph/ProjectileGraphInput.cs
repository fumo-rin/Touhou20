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
            overrideDirection = Vector2.zero;
        }
        public Vector2 NewAimPosition => overrideDirection == Vector2.zero ? (overrideTarget == Vector2.zero ? (Target == null ? (Vector2)Owner.transform.position + Vector2.down * 2f : Target.position) : overrideTarget) : (Vector2)Owner.transform.position + overrideDirection;
        public Vector2 AimPosition => NewAimPosition;
        public Transform Target;
        public Transform Owner;
        public float addedAngle;
        public Vector2 overrideTarget { get; private set; }
        public Vector2 overrideDirection { get; private set; }
        public void SetOverrideTarget(Vector2 position) { overrideTarget = position; }
        public void SetOverrideDirection(Vector2 direction) { overrideDirection = direction; }
        public Vector2 OwnerCurrentPosition => (Vector2)Owner.position;
    } 
}

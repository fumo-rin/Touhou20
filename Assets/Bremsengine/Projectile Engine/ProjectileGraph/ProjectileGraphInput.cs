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
            this.TargetStartPosition = null;
            if (target != null)
            {
                this.TargetStartPosition = target.position;
            }
        }
        public Vector2 NewAimPosition => TargetStartPosition != null ? (Vector2) TargetStartPosition : overrideDirection == Vector2.zero ? (overrideTarget == Vector2.zero ? (Target == null ? (Vector2)Owner.transform.position + Vector2.down * 2f : Target.position) : overrideTarget) : (Vector2)Owner.transform.position + overrideDirection;
        public Vector2 AimPosition => NewAimPosition;
        public Vector2? TargetStartPosition;
        public Transform Target;
        public Transform Owner;
        public float addedAngle;
        public Vector2 overrideTarget { get; private set; }
        public Vector2 overrideDirection { get; private set; }
        public void SetTargetStartPosition(Vector2 position) { TargetStartPosition = position; }
        public void SetOverrideTarget(Vector2 position) { overrideTarget = position; }
        public void SetOverrideDirection(Vector2 direction) { overrideDirection = direction; }
        public Vector2 OwnerCurrentPosition => (Vector2)Owner.position;
    } 
}

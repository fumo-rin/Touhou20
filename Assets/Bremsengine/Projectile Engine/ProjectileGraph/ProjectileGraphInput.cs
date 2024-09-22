using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bremsengine
{
    public struct ProjectileGraphInput
    {
        public Vector2 Origin;
        public Vector2 OverrideTargetPosition => overrideTarget != Vector2.zero ? overrideTarget : Target == null ? (Vector2)Owner.transform.position + Vector2.down : Target.position;
        public Transform Target;
        public Transform Owner;
        public float addedAngle;
        private Vector2 overrideTarget;
        public void SetOverrideTarget(Vector2 position) { overrideTarget = position; }
        public Vector2 OwnerCurrentPosition => Owner.position;
    }
}

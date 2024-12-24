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
            internalOverrideDirection = Vector2.zero;
            this.TargetStartPosition = null;
            if (target != null)
            {
                this.TargetStartPosition = target.position;
            }
        }
        private Vector2 internalOverrideDirection;
        private bool GetInternalOverrideDirection(out Vector2 direction)
        {
            if (internalOverrideDirection != Vector2.zero)
            {
                direction = internalOverrideDirection;
                return true;
            }
            direction = Vector2.zero;
            return false;
        }
        private Vector2 AimSequence()
        {
            if (GetInternalOverrideDirection(out Vector2 d))
            {
                return (Vector2)Owner.position + d;
            }
            Vector2 direction = Vector2.down;



            return (Vector2)Owner.position + direction;
        }
        public Vector2 AimTargetPosition => AimSequence();
        public Vector2? TargetStartPosition;
        public Transform Target;
        public Transform Owner;
        public float addedAngle;
        public Vector2 OwnerCurrentPosition => (Vector2)Owner.position;
        public Vector2 Position => OwnerCurrentPosition;
        public void SetOverrideDirection(Vector2 direction)
        {
            internalOverrideDirection = direction;
        }
        public void SetTargetStartPosition(Vector2 position)
        {
            TargetStartPosition = position;
        }
    } 
}

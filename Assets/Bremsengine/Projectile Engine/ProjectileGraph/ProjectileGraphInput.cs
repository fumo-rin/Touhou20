using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bremsengine
{
    public struct ProjectileGraphInput
    {
        public Vector2 Origin;
        public Vector2 OverrideTargetPosition;
        public Transform Target;
        public Transform Owner;
        public Vector2 OwnerCurrentPosition => Owner.position;
    }
}

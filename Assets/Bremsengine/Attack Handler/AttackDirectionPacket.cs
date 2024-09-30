using UnityEngine;

namespace Bremsengine
{
    public class AttackDirectionPacket
    {
        public AttackDirectionPacket(Transform owner, Transform target)
        {
            Owner = owner;
            Target = target;
        }
        public Transform Owner;
        public Transform Target;
        public Vector2 aimDirectionOverride { get; private set; }
        public void SetAimDirectionOverride(Vector2 direction)
        {
            aimDirectionOverride = direction;
        }
    }
}
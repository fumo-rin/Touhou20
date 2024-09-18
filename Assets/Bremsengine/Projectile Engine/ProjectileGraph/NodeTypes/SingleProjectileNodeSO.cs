using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Bremsengine
{
#if UNITY_EDITOR
    public partial class SingleProjectileNodeSO
    {
        protected override void OnDraw(GUIStyle style)
        {
            base.OnDraw(style);
        }
        protected override Rect GetRect(Vector2 mousePosition)
        {
            return new(mousePosition.x, mousePosition.y, 350f, 400f);
        }

        public override string GetGraphComponentName()
        {

            return "Single Projectile";
        }

        public override void OnGraphDelete()
        {

        }
    }
#endif
    public partial class SingleProjectileNodeSO : ProjectileNodeSO
    {
        public override void Spawn(in List<Projectile> l, Transform owner, Transform target, Vector2 lastTargetPosition, TriggeredEvent triggeredEvent)
        {
            Projectile p = CreateProjectile(ProjectileType.Prefab, owner.position, BuildDirection(owner, target));
            l.Add(p);
            SendProjectileEvents(p, triggeredEvent);
        }
    }
}

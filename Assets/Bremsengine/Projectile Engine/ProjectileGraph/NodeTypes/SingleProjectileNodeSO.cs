using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

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
        public override void Spawn(in List<Projectile> l, ProjectileGraphInput input, TriggeredEvent triggeredEvent)
        {
            ProjectileNodeDirection direction = BuildDirectionAlternate(input);
            direction.AddAngle(input.addedAngle);
            Projectile p = CreateProjectile(ProjectileType.Prefab, input.OwnerCurrentPosition, direction);
            l.Add(p);
            SendProjectileEvents(p, triggeredEvent);
        }
    }
}

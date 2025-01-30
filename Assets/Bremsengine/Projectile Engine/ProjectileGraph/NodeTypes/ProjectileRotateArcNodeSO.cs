using System.Collections.Generic;
using UnityEngine;

namespace Bremsengine
{
    public class ProjectileRotateArcNodeSO : ProjectileNodeSO
    {
        public override string GetGraphComponentName()
        {
            return "Projectile Rotate Arc";
        }

        public override void Spawn(in List<Projectile> list, ProjectileGraphInput input, TriggeredEvent triggeredEvent)
        {
            throw new System.NotImplementedException();
        }

        protected override Rect GetRect(Vector2 mousePosition)
        {
            return new(mousePosition,new(350f,500f));
        }
    }
}

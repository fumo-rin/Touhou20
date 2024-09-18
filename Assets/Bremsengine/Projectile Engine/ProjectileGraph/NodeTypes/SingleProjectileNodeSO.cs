using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Bremsengine
{
#if UNITY_EDITOR
    public partial class SingleProjectileNodeSO
    {
        public override string GetHeaderName()
        {
            return "Single Projectile";
        }
        protected override void OnInitialize(Rect rect, ProjectileGraphSO graph, ProjectileTypeSO type)
        {

        }
        protected override void OnDraw(GUIStyle style)
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

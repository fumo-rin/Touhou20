using System.Collections.Generic;
using UnityEngine;

namespace Bremsengine
{
    #region Editor
#if UNITY_EDITOR
    using UnityEditor;

    public partial class ProjectileRotateArcNodeSO
    {
        public override string GetGraphComponentName()
        {
            return "Projectile Rotate Arc";
        }
        protected override Rect GetRect(Vector2 mousePosition)
        {
            return new(mousePosition, new(350f, 500f));
        }

        protected override void OnDraw(GUIStyle style)
        {
            base.OnDraw(style);
            testMod = (ProjectileMod)EditorGUILayout.ObjectField("Test Mod",testMod, typeof(ProjectileMod), false);
        }
    }
#endif
    #endregion
    public partial class ProjectileRotateArcNodeSO : ProjectileNodeSO
    {
        public ProjectileMod testMod;
        public override void Spawn(in List<Projectile> list, ProjectileGraphInput input, TriggeredEvent triggeredEvent)
        {

        }
    }
}

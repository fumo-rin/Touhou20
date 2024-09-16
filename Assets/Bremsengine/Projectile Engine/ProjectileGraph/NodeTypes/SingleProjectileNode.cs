using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Bremsengine
{
#if UNITY_EDITOR
    public partial class SingleProjectileNode
    {
        protected override void OnInitialize(Rect rect, ProjectileGraphSO graph, ProjectileTypeSO type)
        {
            this.ProjectileType = type;
            this.rect = NodeRect(rect.x, rect.y, 350f, 700f);

        }
        const float spritePreviewSize = 250f;
        protected override void OnDraw(GUIStyle style)
        {
            int selected = ProjectileCache.FindIndex(x => x == ProjectileType);
            int selection = EditorGUILayout.Popup("", selected, GetProjectileTypesToDisplay());

            ProjectileType = ProjectileCache[selection];
            if (ProjectileType != null)
            {
                SetPreviewTexture(ProjectileType);
            }
            staticDirection = EditorGUILayout.Vector2Field("Override Direction", staticDirection);

            directionalOffset = EditorGUILayout.Slider("Directional Offset", directionalOffset, 0f, 10f);
            spread = EditorGUILayout.Slider("Spread", spread, 0f, 60f);
            speed = EditorGUILayout.Slider("Speed", speed, 0f, 35f);
            addedAngle = EditorGUILayout.Slider("Added Angle", addedAngle, -180f, 180f);
        }
    }
#endif
    public partial class SingleProjectileNode : ProjectileNodeSO
    {
        Vector2 staticDirection;
        float directionalOffset;
        float spread;
        float speed;
        float addedAngle;

        public ProjectileTypeSO ProjectileType;

        public override List<Projectile> Spawn(Transform owner, Transform target, Vector2 lastTargetPosition)
        {
            ProjectileNodeDirection direction = new(owner, target, staticDirection);
            direction.SetDirectionalOffset(directionalOffset).SetSpread(spread).SetSpeed(speed).AddAngle(addedAngle);
            List<Projectile> spawnList = new List<Projectile>();
            CreateProjectile(ProjectileType.Prefab, owner.position, direction);
            SendProjectileEvents(spawnList);
            return spawnList;
        }

    }
}

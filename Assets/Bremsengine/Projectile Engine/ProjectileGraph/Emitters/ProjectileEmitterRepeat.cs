using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Extensions;
using UnityEditor;
using UnityEngine.Serialization;

namespace Bremsengine
{
    #region Editor
#if UNITY_EDITOR
    public partial class ProjectileEmitterRepeat : ProjectileEmitterSO
    {
        public override string GetGraphComponentName()
        {
            return "Repeat Emitter";
        }

        protected override Rect GetRect(Vector2 mousePosition)
        {
            return base.GetRect(mousePosition);
        }

        protected override void OnDraw(GUIStyle style)
        {
            base.OnDraw(style);
            timeBetweenRepeats = EditorGUILayout.Slider("Time Between Repeats", timeBetweenRepeats, 0f, 2f);
            repeatCount = EditorGUILayout.IntSlider("Repeat Count", repeatCount, 1, 100);
            repeatAddedAngle = EditorGUILayout.Slider("Repeat Added Angle", repeatAddedAngle, -180f, 180f);
        }

        protected override void OnInitialize(Vector2 mousePosition, ProjectileGraphSO graph, ProjectileTypeSO type)
        {
            base.OnInitialize(mousePosition, graph, type);
            graph.emitters.AddIfDoesntExist(this);
        }
    }
#endif
    #endregion
    public partial class ProjectileEmitterRepeat : ProjectileEmitterSO
    {
        [FormerlySerializedAs("repeatInterval")]
        public float timeBetweenRepeats = 0.3f;
        public int repeatCount = 4;
        public float repeatAddedAngle = 0f;
        public override void Trigger(TriggeredEvent triggeredEvent, ProjectileGraphInput input, Projectile.SpawnCallback callback, int forcedLayer)
        {
            EmitterSettings settings = new();
            settings.EntryDelay = addedDelay;
            settings.AddedAnglePerIteration = repeatAddedAngle;
            settings.RepeatCounts = repeatCount;
            settings.TimeBetweenRepeats = timeBetweenRepeats;
            ProjectileEmitterTimelineHandler.Queue(Co_Emit(settings, triggeredEvent, input, callback, forcedLayer), input.Owner);
            /*for (int i = 0; i < repeatCount; i++)
            {
                //input.addedAngle = addedAngle;
                ProjectileEmitterTimelineHandler.Queue(Co_Emit(settings, triggeredEvent, input, callback, forcedLayer), input.Owner);
                //delay += repeatInterval;
                //addedAngle += repeatAddedAngle;
            }*/
        }

        protected override float GetCooldownDelay()
        {
            return addedDelay + (repeatCount * timeBetweenRepeats);
        }
    }
}

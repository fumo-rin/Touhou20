using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bremsengine
{
    #region Editor
#if UNITY_EDITOR

    public partial class ProjectileEmitterSingle : ProjectileEmitterSO
    {
        public override string GetGraphComponentName()
        {
            return "Single Emitter";
        }
        protected override Rect GetRect(Vector2 mousePosition)
        {
            return base.GetRect(mousePosition);
        }
        protected override void OnInitialize(Vector2 mousePosition, ProjectileGraphSO graph, ProjectileTypeSO type)
        {
            base.OnInitialize(mousePosition, graph, type);
            graph.emitters.AddIfDoesntExist(this);
        }
    }
#endif
    #endregion
    public partial class ProjectileEmitterSingle : ProjectileEmitterSO
    {
        public override void Trigger(TriggeredEvent triggeredEvent, ProjectileGraphInput input, Projectile.SpawnCallback callback, int forcedLayer)
        {
            EmitterSettings settings = new();
            settings.EntryDelay = addedDelay;
            settings.AddedAnglePerIteration = 0f;
            settings.RepeatCounts = 1;
            settings.TimeBetweenRepeats = 0f;
            ProjectileEmitterTimelineHandler.Queue(Co_Emit(settings, triggeredEvent, input, callback, forcedLayer), input.Owner);
        }

        protected override float GetCooldownDelay()
        {
            return addedDelay;
        }
    }
}
using Core.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Bremsengine
{
    #region Editor
#if UNITY_EDITOR
    public abstract partial class ProjectileEventSO : ProjectileGraphComponent
    {
        protected override Rect GetRect(Vector2 mousePosition)
        {
            return new Rect(mousePosition, new(160f, 75f));
        }
        public override string GetGraphComponentName()
        {
            return "Headhunter, Leather Belt";
        }
        public void Reinitialize()
        {
            OnInitialize(new(rect.x, rect.y), graph, null);
        }
        protected override void OnInitialize(Vector2 mousePosition, ProjectileGraphSO graph, ProjectileTypeSO type)
        {

        }
        public override void OnGraphDelete()
        {
            foreach (var item in graph.nodes)
            {
                item.linkedProjectileEvents.Remove(this);
            }
        }
    }
#endif
    #endregion
    public abstract partial class ProjectileEventSO : ProjectileGraphComponent
    {
        public float EventDelay = 0f;
        public void QueueEvents(Projectile p, ProjectileNodeSO node, TriggeredEvent e)
        {
            TriggerEvent(p, e);
        }
        protected abstract void TriggerEvent(Projectile p, TriggeredEvent e);
    }
    [System.Serializable]
    public class TriggeredEvent
    {
        public ProjectileNodeSO Caller;
        HashSet<ProjectileEventSO> LinkedEvents;
        HashSet<AudioClipWrapper> playedSounds = new();
        public bool HasPlayed(AudioClipWrapper acw) => playedSounds.Contains(acw);
        public TriggeredEvent Bind(ProjectileNodeSO node)
        {
            Caller = node;
            LinkedEvents = new HashSet<ProjectileEventSO>();
            foreach (var item in Caller.linkedProjectileEvents)
            {
                LinkedEvents.Add(item);
            }
            return this;
        }
        public TriggeredEvent PlayRepeatSound(Vector2 position, AudioClipWrapper acw)
        {
            acw.Play(position);
            return this;
        }
        public TriggeredEvent PlaySound(Vector2 position, AudioClipWrapper acw)
        {
            if (playedSounds.Contains(acw))
                return this;
            playedSounds.Add(acw);
            acw.Play(position);
            return this;
        }
    }
}

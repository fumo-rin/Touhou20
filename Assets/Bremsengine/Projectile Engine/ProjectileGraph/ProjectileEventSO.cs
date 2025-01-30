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
        protected override void OnInitialize(Vector2 mousePosition, ProjectileGraphSO graph, ProjectileTypeSO type)
        {

        }
        protected override void OnDraw(GUIStyle style)
        {
            EventDelay = EditorGUILayout.Slider("Event Delay", EventDelay, 0f, 10f);
        }
        public override void OnGraphDelete()
        {
            foreach (var item in graph.nodes)
            {
                item.linkedProjectileEvents.Remove(this);
            }
        }
        public override void TryBreakLinks()
        {
            foreach(var item in graph.nodes)
            {
                item.linkedProjectileEvents.Remove(this);
            }
        }

        public override void TryCreateLink(ProjectileGraphComponent other)
        {
            if (other is ProjectileNodeSO node)
            {
                node.linkedProjectileEvents.AddIfDoesntExist(this);
            }
        }

        public override void ReceiveBroadcastUnlink(ProjectileGraphComponent unlink)
        {

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
        public Dictionary<string, float> keyfloats = new Dictionary<string, float>();
        public ProjectileNodeSO Caller;
        HashSet<ProjectileEventSO> LinkedEvents = new();
        HashSet<AudioClipWrapper> playedSounds = new();
        Dictionary<int, HashSet<ProjectileEventSO>> PlayedEvents = new();
        public bool HasPlayedEvent(Projectile p, ProjectileEventSO e)
        {
            // requires manually adding the event with RegisterEvent
            if (!PlayedEvents.ContainsKey(p.projectileID))
            {
                return false;
            }
            if (PlayedEvents[p.projectileID].Contains(e))
                return true;
            return false;
        }
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
        public void RegisterEvent(Projectile p, ProjectileEventSO e)
        {
            if (!PlayedEvents.ContainsKey(p.projectileID))
            { 
                PlayedEvents[p.projectileID] = new();
            }
            PlayedEvents[p.projectileID].Add(e);
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
        public TriggeredEvent ClearPlayedSounds()
        {
            playedSounds.Clear();
            return this;
        }
    }
}

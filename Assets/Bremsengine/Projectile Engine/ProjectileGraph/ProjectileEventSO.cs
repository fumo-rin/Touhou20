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
    public abstract partial class ProjectileEventSO : ScriptableObject
    {
        protected abstract Rect GetRect(Vector2 mousePosition);
        public abstract string GetEventName();
        [HideInInspector] public Rect rect;
        [HideInInspector] public ProjectileGraphSO graph;
        protected abstract void OnInitialize(Vector2 mousePosition, ProjectileGraphSO graph);
        protected abstract void OnDraw(GUIStyle style);
        public void Initialize(Vector2 mousePosition, ProjectileGraphSO graph)
        {
            rect = GetRect(mousePosition);
            this.graph = graph;
            this.name = GetEventName();
            this.ID = Guid.NewGuid().ToString();
            graph.knownEvents.AddIfDoesntExist(this);
            OnInitialize(mousePosition, graph);
        }
        public bool IsMouseOver(Vector2 mousePosition)
        {
            return rect.Contains(mousePosition);
        }
        public void Reinitialize()
        {
            Initialize(new(rect.x, rect.y), graph);
        }
        public void Draw(GUIStyle style)
        {
            GUILayout.BeginArea(rect, style);
            EditorGUI.BeginChangeCheck();

            OnDraw(style);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssetIfDirty(this);
            }
            GUILayout.EndArea();
        }
        public void DeleteEvent()
        {
            foreach (var item in graph.nodes)
            {
                item.linkedProjectileEvents.Remove(this);
            }
            graph.knownEvents.Remove(this);
            DestroyImmediate(this, true);
            AssetDatabase.SaveAssets();
        }
    }
#endif
    #endregion
    public abstract partial class ProjectileEventSO : ScriptableObject
    {
        public string ID;
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

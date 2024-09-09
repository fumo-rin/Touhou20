using Bremsengine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace BremseTouhou
{
    public abstract class ProjectileEvent : ScriptableObject
    {
        public float Delay => delay;
        [SerializeField] protected float delay = 0f;
        public abstract void PerformEvent(Projectile p, Vector2 target);
        public void QueueEvent(Projectile p, BaseUnit target)
        {
            if (p == null || target == null)
            {
                Debug.LogWarning("1");
            }
            ProjectileEventHandler.QueueEvent(this, p, target);
        }
    }
}

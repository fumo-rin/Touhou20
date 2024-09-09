using Bremsengine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace BremseTouhou
{
    public abstract class ProjectileEvent : ScriptableObject
    {
        WaitForSeconds storedDelay;
        public WaitForSeconds Delay => storedDelay == null ? storedDelay = new WaitForSeconds(delay) : storedDelay;
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

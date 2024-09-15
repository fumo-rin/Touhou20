using Bremsengine;
using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace BremseTouhou
{
    public abstract class ProjectileEvent : ScriptableObject
    {
        WaitForSeconds storedDelay;
        public WaitForSeconds Delay => (storedDelay == null) ? storedDelay = new WaitForSeconds(delay) : storedDelay;
        [SerializeField] protected float delay = 0f;
        [SerializeField] AudioClipWrapper eventSound;
        float validateDelay;
        private void OnValidate()
        {
            if (validateDelay != delay)
            {
                storedDelay = null;
            }
            validateDelay = delay;
        }
        public abstract void PerformEvent(Projectile p, BaseUnit owner,Vector2 target);
        public void PlaySound(Vector2 position)
        {
            eventSound.Play(position);
        }
        public void QueueEvent(Projectile p, BaseUnit owner, BaseUnit target)
        {
            if (p == null)
            {
                Debug.LogWarning("1");
            }
            ProjectileEventHandler.QueueEvent(this, p, owner, target);
        }
    }
}

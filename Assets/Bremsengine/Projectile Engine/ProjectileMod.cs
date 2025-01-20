using System.Collections;
using UnityEngine;

namespace Bremsengine
{
    public abstract class ProjectileMod : ScriptableObject
    {
        [SerializeField] private float delayInSeconds = 0.5f;
        protected abstract IEnumerator CO_ModifierPayload(Projectile p, WaitForSeconds delay);
        public WaitForSeconds Delay => new WaitForSeconds(delayInSeconds);
        public void QueueMod(Projectile p, WaitForSeconds delay)
        {
            p.StartCoroutine(CO_ModifierPayload(p, delay));
        }
    }
}
using Bremsengine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    public class ProjectileEventHandler : MonoBehaviour
    {
        static ProjectileEventHandler instance;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (instance == null)
            {
                GameObject g = new GameObject("Projectile Event Handler");
                instance = g.AddComponent<ProjectileEventHandler>();
                DontDestroyOnLoad(g);
            }
        }
        private static void ClearAllEvents()
        {
            instance.StopAllCoroutines();
        }
        public static void QueueEvent(ProjectileEvent e, Projectile p, BaseUnit owner, BaseUnit target)
        {
            p.StartCoroutine(instance.Run(e, p, owner, target));
        }
        private IEnumerator Run(ProjectileEvent e, Projectile p, BaseUnit owner, BaseUnit target)
        {
            yield return e.Delay;
            Vector2 eventTarget = p.Position + p.Velocity;
            if (target != null)
            {
                eventTarget = target.Center;
            }
            if (!owner.Alive)
            {
                yield break;
            }
            e.PerformEvent(p, owner, eventTarget);
            e.PlaySound(p.Position);
        }
    }
}
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
        private static void ClearEvents()
        {
            instance.StopAllCoroutines();
        }
        public static void QueueEvent(ProjectileEvent e, Projectile p, BaseUnit target)
        {
            p.StartCoroutine(instance.Run(e, p, target));
        }
        private IEnumerator Run(ProjectileEvent e, Projectile p, BaseUnit target)
        {
            yield return e.Delay;
            e.PerformEvent(p, target.Center);
        }
    }
}

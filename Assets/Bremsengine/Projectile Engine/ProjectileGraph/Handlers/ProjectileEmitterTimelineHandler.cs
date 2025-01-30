using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bremsengine
{
    public class ProjectileEmitterTimelineHandler : MonoBehaviour
    {
        static ProjectileEmitterTimelineHandler instance;
        public static Dictionary<Transform, List<Coroutine>> activeRoutines;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (instance != null)
            {
                return;
            }
            activeRoutines = new Dictionary<Transform, List<Coroutine>>();
            GameObject o = new GameObject("Projectile Emitter Timeline Handler");
            instance = o.AddComponent<ProjectileEmitterTimelineHandler>();
            DontDestroyOnLoad(o);
        }
        public static void ClearEmitQueue(Transform owner)
        {
            if (activeRoutines.ContainsKey(owner) && activeRoutines[owner] is not null)
            {
                foreach (var item in activeRoutines[owner])
                {
                    if (item == null)
                        continue;
                    instance.StopCoroutine(item);
                }
                activeRoutines[owner].Clear();
            }
        }
        public static void Queue(IEnumerator coroutine, Transform owner)
        {
            Coroutine co = instance.StartCoroutine(coroutine);
            if (owner == null)
            {
                return;
            }
            activeRoutines[owner].Add(co);
        }
    }
}

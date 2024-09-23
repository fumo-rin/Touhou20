using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bremsengine
{
    public class ProjectileEmitterTimelineHandler : MonoBehaviour
    {
        static ProjectileEmitterTimelineHandler instance;
        static Dictionary<Transform, List<Coroutine>> activeRoutines;
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
                activeRoutines[owner] = null;
            }
        }
        public static void Queue(IEnumerator coroutine, Transform owner)
        {
            Debug.Log("Queue");
            Coroutine co = instance.StartCoroutine(coroutine);
            if (owner == null)
            {
                return;
            }
            if (!activeRoutines.ContainsKey(owner))
            {
                activeRoutines[owner] = new();
            }
            activeRoutines[owner].Add(co);
        }
    }
}

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Bremsengine
{
    public class BulletFlare : MonoBehaviour
    {
        static BulletFlare instance;
        [SerializeField] ParticleSystem flare;
        Queue<ParticleSystem> flares;
        [SerializeField] Transform stack;
        [SerializeField] int flareCacheSize = 100;
        private void Awake()
        {
            flares = new Queue<ParticleSystem>(flareCacheSize);
            instance = this;
            for (int i = 0; i < flareCacheSize; ++i)
            {
                ParticleSystem newFlare = Instantiate(flare, stack);
                flares.Enqueue(newFlare);
            }
        }
        public static bool TryNextFlare(int id, out ParticleSystem flare)
        {
            flare = null;
            if (id == 0)
                return false;
            if (instance.flares.Count <= 0f)
            {
                return false;
            }
            flare = instance.flares.Dequeue();
            instance.flares.Enqueue(flare);
            return flare != null;
        }
        public static void FlareAt(Vector2 position, int id)
        {
            if (TryNextFlare(id, out ParticleSystem flare))
            {
                if (flare == null)
                {
                    return;
                }
                flare.transform.position = position;
                flare.Play();
            }
        }
    }
}

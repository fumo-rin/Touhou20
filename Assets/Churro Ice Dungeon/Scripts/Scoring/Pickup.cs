using Bremsengine;
using Core.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChurroIceDungeon
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Pickup : MonoBehaviour
    {
        struct SpawnQueue
        {
            public Pickup prefab;
            public Vector2 position;
            public SpawnQueue(Pickup prefab, Vector2 position)
            {
                this.prefab = prefab;
                this.position = position;
            }
        }
        static Queue<Pickup> pool;
        static Queue<SpawnQueue> ToSpawn;
        [SerializeField] Collider2D col;
        [SerializeField] Rigidbody2D rb;
        float velocity = 4f;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Reinitialize()
        {
            pool = new();
            ToSpawn = new();
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BindToSceneLoader()
        {
            GeneralManager.SetStageLoadAction("Reset Pickups Pool", Reinitialize);
        }
        float pickupSpeed = 2.5f;
        public static void SpawnPickup(Pickup prefab, Vector2 position)
        {
            ToSpawn.Enqueue(new(prefab, position));
        }
        public static void RunPickupQueue(int amount)
        {
            SpawnQueue item;
            for (int i = 0; i < amount; i++)
            {
                if (ToSpawn.Count > 0)
                {
                    item = ToSpawn.Dequeue();
                    if (item.prefab == null)
                        continue;
                    CreatePickup(item.prefab, item.position);
                }
            }
        }
        private static void CreatePickup(Pickup prefab, Vector2 position)
        {
            Pickup spawned = null;
            if (pool != null && pool.Count > 0)
            {
                spawned = pool.Dequeue();
                spawned.gameObject.SetActive(true);
                spawned.col.enabled = true;
                spawned.transform.position = position;
            }
            else
            {
                spawned = Instantiate(prefab, position, Quaternion.identity);
            }
            spawned.rb.bodyType = RigidbodyType2D.Dynamic;
            spawned.rb.linearVelocity = new(0f, prefab.velocity);
        }
        public void StartPickup(Transform target)
        {
            IEnumerator CO_Pickup(Transform target)
            {
                col.enabled = false;
                Vector2 originalPosition = transform.position;
                float lerp = 0f;
                rb.bodyType = RigidbodyType2D.Kinematic;
                while (lerp <= 1f)
                {
                    lerp += Time.deltaTime * pickupSpeed;
                    transform.position = originalPosition.Lerp(target.position, lerp.Squared().Clamp(0f, 1f));
                    yield return null;
                }
                ClearPickup();
                WakaScoring.AddPickupScore();
            }
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(CO_Pickup(target));
            }
        }
        public void ClearPickup()
        {
            gameObject.SetActive(false);
            pool.Enqueue(this);
        }
    }
}
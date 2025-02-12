using Core.Extensions;
using System.Collections;
using UnityEngine;

namespace BremseTouhou
{
    public class UnitSpawner : MonoBehaviour
    {
        [SerializeField] BaseUnit spawnUnit;
        [SerializeField] float interval = 0.2f;
        [SerializeField] int count = 20;
        [SerializeField] Vector2 spawnBounds;
        public Vector2 GetSpawnPosition => spawnBounds.RandomFromZero() + (Vector2)transform.position - (0.5f * spawnBounds);
        private void SpawnUnits()
        {
            WaitForSeconds wait = new WaitForSeconds(interval);
            IEnumerator CO_Spawn()
            {
                yield return null;
                int i = 0;
                while (i < count)
                {
                    Instantiate(spawnUnit, GetSpawnPosition, Quaternion.identity);
                    i++;
                    yield return wait;
                }
            }
            StartCoroutine(CO_Spawn());
        }
        private void OnEnable()
        {
            SpawnUnits();
        }
        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}

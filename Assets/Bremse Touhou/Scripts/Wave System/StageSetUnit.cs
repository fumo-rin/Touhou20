using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    [CreateAssetMenu(menuName ="Bremse Touhou/Stageset/Unit")]
    public class StageSetUnit : ScriptableObject
    {
        [SerializeField] BaseUnit unit;
        [SerializeField] StageSpawnPoint overrideSpawnpoint;
        public BaseUnit SpawnUnit(StageSpawnPoint point, Vector2 worldCenter)
        {
            StageSpawnPoint s = point;
            if (overrideSpawnpoint != null)
            {
                s = overrideSpawnpoint;
            }
            Debug.Log(worldCenter);
            Debug.Log(s.spawnOffset);
            BaseUnit spawned = Instantiate(unit, worldCenter + s.spawnOffset, Quaternion.identity);
            if (spawned is EnemyUnit e)
            {
                e.SetTarget(BaseUnit.Player);
            }
            return spawned;
        }
    }
}
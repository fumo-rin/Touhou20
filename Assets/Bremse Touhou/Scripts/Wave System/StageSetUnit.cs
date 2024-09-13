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
        public BaseUnit SpawnUnit(StageSpawnPoint point)
        {
            StageSpawnPoint s = point;
            if (overrideSpawnpoint != null)
            {
                s = overrideSpawnpoint;
            }
            BaseUnit spawned = Instantiate(unit, s.SpawnPosition, Quaternion.identity);
            if (spawned is EnemyUnit e)
            {
                e.SetTarget(BaseUnit.Player);
            }
            return spawned;
        }
    }
}
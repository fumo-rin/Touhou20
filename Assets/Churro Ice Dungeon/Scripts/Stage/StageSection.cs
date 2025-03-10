using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChurroIceDungeon
{
    public abstract class StageSection : MonoBehaviour
    {
        Coroutine activeSection;
        List<DungeonUnit> spawnedUnits = new();
        public void ActivateSection(float time)
        {
            if (activeSection != null)
            {
                StopCoroutine(activeSection);
            }
            activeSection = StartCoroutine(StartSection(time));
        }
        public bool SpawnUnit(DungeonUnit unit, Vector2 position, out DungeonUnit spawnedUnit)
        {
            spawnedUnit = Instantiate(unit, position, Quaternion.identity);
            return spawnedUnit != null;
        }
        public void EndSection()
        {
            foreach (DungeonUnit unit in spawnedUnits)
            {
                unit.ExternalKill();
            }
        }
        protected abstract IEnumerator StartSection(float startingTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    [System.Serializable]
    public class StageSet
    {
        public string stageSetName = "";
        public float StageSetForcedDuration = -1f;
        [SerializeField] List<StageSetUnitEntry> entries = new List<StageSetUnitEntry>();
        public StageSetUnitEntry[] UnitEntries => entries.ToArray();
    }
    [System.Serializable]
    public class StageSetUnitEntry
    {
        public StageSetUnit unit;
        public StageSpawnPoint spawnPoint;
        [field: SerializeField] public float SpawnDelay { get; set; } = 0f;
    }
}

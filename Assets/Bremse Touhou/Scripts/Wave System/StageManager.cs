using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    #region Load Stage Set
    public partial class StageManager
    {
        static Coroutine currentDelayedQueue;
        public static bool LoadNextInQueue()
        {
            if (stageSetQueue == null || stageSetQueue.Count <= 0)
            {
                return false;
            }
            StageSet set = stageSetQueue.Dequeue();


            foreach (var unitEntry in set.UnitEntries)
            {
                instance.LoadUnitEntry(unitEntry);
            }

            if (set.StageSetForcedDuration > 0f)
            {
                LoadNextInQueueAfterSeconds(set.StageSetForcedDuration);
            }

            return true;
        }
        private IEnumerator CO_LoadNextInQueueAfterSeconds(float delay)
        {
            yield return new WaitForSeconds(delay);
            LoadNextInQueue();
        }
        public static void LoadNextInQueueAfterSeconds(float delay)
        {
            if (currentDelayedQueue != null)
            {
                instance.StopCoroutine(currentDelayedQueue);
            }
            currentDelayedQueue = instance.StartCoroutine(instance.CO_LoadNextInQueueAfterSeconds(delay));
        }
    }
    #endregion
    #region
    public partial class StageManager
    {
        private static void ClearAllKnownUnits()
        {
            foreach (var unitEntry in knownUnits)
            {
                if (unitEntry != null && unitEntry.gameObject != null)
                {
                    Destroy(unitEntry.gameObject);
                }
            }
            knownUnits.Clear();
        }
        private void LoadUnitEntry(StageSetUnitEntry entry)
        {
            StartCoroutine(CO_LoadUnitEntry(entry));
        }
        private IEnumerator CO_LoadUnitEntry(StageSetUnitEntry entry)
        {
            yield return new WaitForSeconds(entry.SpawnDelay);
            BaseUnit spawnedUnit = entry.unit.SpawnUnit(entry.spawnPoint);
            knownUnits.Add(spawnedUnit);
        }
    }
    #endregion
    public partial class StageManager : MonoBehaviour
    {
        static StageManager instance;
        static Queue<StageSet> stageSetQueue = new();
        [SerializeField] StageSO editorStage;
        static HashSet<BaseUnit> knownUnits = new();
        private void Awake()
        {
            instance = this;
            if (editorStage != null)
            {
                LoadStageSet(editorStage);
            }
        }
        private void LoadStageSet(StageSO stage)
        {
            stageSetQueue.Clear();
            ClearAllKnownUnits();
            foreach (var item in stage.sets)
            {
                stageSetQueue.Enqueue(item);
            }
            LoadNextInQueue();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Extensions;
using UnityEngine.UI;
using UnityEditor;
using Bremsengine;

namespace BremseTouhou
{
    #region Boss Entry
    [System.Serializable]
    public class BossEntry
    {
        public BaseUnit unit;
        public BossHealthbar healthBar;
        public void SetBossHealthUI()
        {
            if (unit == null && healthBar != null && healthBar.gameObject != null)
            {
                Debug.Log("Tt2");
                GameObject.Destroy(healthBar.gameObject);
            }
            healthBar.SetHealthUI(unit);
        }
        public void DestroyEntry()
        {
            if (healthBar != null)
            {
                Debug.Log("Tt 3 : " + unit.name);
                GameObject.Destroy(healthBar.gameObject);
            }
        }
    }
    #endregion
    [DefaultExecutionOrder(-3)]
    public partial class BossManager : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {

        }
        static BossManager instance;
        [SerializeField] BossHealthbar healthbarPrefab;
        [SerializeField] BossPositionBar positionBarPrefab;
        [SerializeField] Transform healthbarSocket;
        List<BossEntry> bossList = new();
        private void ProcessBossList(int tickIndex, float deltaTime)
        {
            for (int i = 0; i < bossList.Count; ++i)
            {
                if (bossList[i].unit == null)
                {
                    bossList.RemoveAt(i);
                    i--;
                    continue;
                }
                if (bossList[i] != null && bossList[i].unit.Alive)
                {
                    bossList[i].SetBossHealthUI();
                    continue;
                }
                Destroy(bossList[i].healthBar.gameObject);
                bossList.RemoveAt(i);
                i--;
            }
        }
        private void Awake()
        {
            instance = this;
            bossList = new();
        }
        private void Start()
        {
            TickManager.MainTick += instance.ProcessBossList;
        }
        private void OnDestroy()
        {
            TickManager.MainTick -= instance.ProcessBossList;
        }
        public static bool Contains(BaseUnit boss)
        {
            #region Slow Search
            foreach (var item in instance.bossList)
            {
                if (item.unit == boss)
                    return true;
            }
            #endregion
            return false;
        }
        public static void Bind(BaseUnit boss)
        {
            Debug.Log("T1");
            if (Contains(boss))
            {
                return;
            }
            Debug.Log("TT1");
            BossEntry entry = new BossEntry();
            entry.unit = boss;
            entry.healthBar = Instantiate(instance.healthbarPrefab, instance.healthbarSocket);
            BossPositionBar positionBar = Instantiate(instance.positionBarPrefab, BossPositionBarSocket.Socket);
            positionBar.SetTrackedBoss(boss);
            entry.SetBossHealthUI();

            instance.bossList.Add(entry);
        }
        public static void Release(BaseUnit boss)
        {
            if (boss == null)
                return;
            for (int i = 0; i < instance.bossList.Count; i++)
            {
                if (instance.bossList[i].unit == boss)
                {
                    instance.bossList[i].DestroyEntry();
                    instance.bossList.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}

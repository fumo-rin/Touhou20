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
                GameObject.Destroy(healthBar.gameObject);
            }
            healthBar.SetHealthUI(unit);
        }
    }
    #endregion
    [DefaultExecutionOrder(-3)]
    public partial class BossManager : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            bossList = new();
        }
        static BossManager instance;
        [SerializeField] BossHealthbar healthbarPrefab;
        [SerializeField] BossPositionBar positionBarPrefab;
        [SerializeField] Transform healthbarSocket;
        static List<BossEntry> bossList;
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
            foreach (var item in bossList)
            {
                if (item.unit == boss)
                    return true;
            }
            #endregion
            return false;
        }
        public static void Bind(BaseUnit boss)
        {
            if (Contains(boss))
            {
                return;
            }
            BossEntry entry = new BossEntry();
            entry.unit = boss;
            entry.healthBar = Instantiate(instance.healthbarPrefab, instance.healthbarSocket);
            BossPositionBar positionBar = Instantiate(instance.positionBarPrefab, BossPositionBarSocket.Socket);
            positionBar.SetTrackedBoss(boss);
            entry.SetBossHealthUI();

            bossList.Add(entry);
        }
        public static void Release(BaseUnit boss)
        {
            for (int i = 0; i < bossList.Count; i++)
            {
                if (bossList[i] != null)
                {
                    bossList.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}

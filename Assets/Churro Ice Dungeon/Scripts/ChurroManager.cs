using Core.Extensions;
using UnityEngine;

namespace ChurroIceDungeon
{
    #region Debris
    public partial class ChurroManager
    {
        [SerializeField] Debris debrisPrefab;
        public static void SpawnDebris(Vector2 position, DebrisPacket packet)
        {
            if (instance == null || packet.amount <= 0 || packet.sprite == null)
            {
                return;
            }
            for (int i = 0; i < packet.amount.Min(20); i++)
            {
                Instantiate(instance.debrisPrefab, position + Random.insideUnitCircle, Quaternion.identity).SetDebris(packet);
            }
        }
    }
    #endregion
    #region Stats
    public partial class ChurroManager
    {
        public const int RespawnCost = -3;
        public static int Strength { get; private set; }
        public static int Braincells { get; private set; }
        public static bool CanRespawn => Braincells >= 3;
        public delegate void StatRefresh(int value);
        public static StatRefresh OnStrengthChange;
        public static StatRefresh OnBraincellsChange;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Reinitialize()
        {
            Strength = 100;
            Braincells = 9;
        }
        public static void RequestStatsRefresh()
        {
            OnStrengthChange?.Invoke(Strength);
            OnBraincellsChange?.Invoke(Braincells);
        }
        [QFSW.QC.Command("-braincells")]
        public static void ChangeBraincells(int amount)
        {
            Braincells += amount;
            OnBraincellsChange?.Invoke(Braincells);
        }
        [QFSW.QC.Command("-strength")]
        public static void ChangeStrength(int amount)
        {
            Strength += amount;
            OnStrengthChange?.Invoke(Strength);
        }
        public void KillChangeStrength(float multiplier = 0.6f)
        {
            Strength = ((float)(Strength - 100) * multiplier).Max(100f).Floor().ToInt();
            OnStrengthChange?.Invoke(Strength);
        }
    }
    #endregion
    #region Home Improvement
    public partial class ChurroManager
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeDestruction()
        {
            currentDestruction = 0;
            StageMaxDestruction = 1000;
            StageDestructionToProgress = 800;
        }
        static float currentDestruction;
        static float StageMaxDestruction;
        static float StageDestructionToProgress;
        public delegate void DestructionBarValues(float current, float progressCap);
        public static DestructionBarValues OnDestructionRefresh;
        public static void SLOW_BuildBar()
        {
            float collection = 0;
            foreach (var item in GameObject.FindObjectsByType<DestructionItem>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                if (item == null)
                    continue;
                collection += item.CollectValue();
            }
            currentDestruction = 0;
            StageMaxDestruction = collection;
            SetDestructionCapPercentage(80f);
        }
        public static void SetDestructionCapPercentage(float percent)
        {
            percent = percent.Clamp(0f, 100f).Percentify();
            StageDestructionToProgress = percent * StageMaxDestruction;
            RequestDestructionBarRefresh();
        }
        public static void RequestDestructionBarRefresh()
        {
            OnDestructionRefresh?.Invoke(currentDestruction, StageDestructionToProgress);
        }
        public static void AddDestruction(float value)
        {
            Debug.Log(value);
            currentDestruction += value;
            RequestDestructionBarRefresh();
        }
    }
    #endregion
    [DefaultExecutionOrder(-5)]
    public partial class ChurroManager : MonoBehaviour
    {
        static ChurroManager instance;
        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {
            RequestStatsRefresh();
        }
    }
}
using Bremsengine;
using Core.Extensions;
using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChurroIceDungeon
{
    #region Game Progress
    public partial class ChurroManager
    {
        [SerializeField] string MainMenuSceneString = "Churro Main Menu";
        static Dictionary<int, Action> progressCache;
        public static int CurrentProgress;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ReinitializeProgress()
        {
            CurrentProgress = 0;
            progressCache = new Dictionary<int, Action>();
        }
        public static void DefineProgress(int progress, System.Action action)
        {
            if (!progressCache.ContainsKey(progress))
                progressCache.Add(progress, action);
            else
                progressCache[progress] += action;
        }
        [Command("-prog-set")]
        public static bool LoadProgress(int value)
        {
            if (progressCache.TryGetValue(value, out Action action))
            {
                action?.Invoke();
                CurrentProgress = value;
                return true;
            }
            else
            {
                ProgressEndGame();
                return false;
            }
        }
        [Command("-prog-next")]
        public static void Test_GoToNextProgress()
        {
            if (SceneManager.GetActiveScene().name == instance.MainMenuSceneString)
            {
                StartGame?.Invoke();
            }
            CurrentProgress++;
            LoadProgress(CurrentProgress);
        }
        public static void LoseGame()
        {
            ChurroGameProgress.SetEndGame();
        }
        static void ProgressEndGame()
        {
            ChurroGameProgress.SetEndGame();
        }
        public static void LoadMainMenu(float delay)
        {
            void ResetGameState()
            {
                ResetStats();
                ChurroUnit.ClearInventorySnapshot();
                OnRestartGame?.Invoke();
            }
            if (instance == null)
            {
                return;
            }
            GeneralManager.LoadSceneAfterDelay(instance.MainMenuSceneString, delay, ResetGameState);
        }
    }
    #endregion
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
                Instantiate(instance.debrisPrefab, position + UnityEngine.Random.insideUnitCircle, Quaternion.identity).SetDebris(packet);
            }
        }
    }
    #endregion
    #region Stats
    public partial class ChurroManager
    {
        public const int RespawnCost = -1;
        public static int Strength { get; private set; }
        public static int Braincells { get; private set; }
        public static bool HardMode => GeneralManager.ChurroHardmode;
        public static bool CanRespawn => Braincells >= RespawnCost.Abs();
        public delegate void GameState();
        public static GameState OnRestartGame;
        public static GameState StartGame;
        public delegate void StatRefresh(int value);
        public static StatRefresh OnStrengthChange;
        public static StatRefresh OnBraincellsChange;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Reinitialize()
        {
            Strength = 100;
            Braincells = 9;
        }
        public static void ResetStats()
        {
            Strength = 100;
            Braincells = 9;
            RequestStatsRefresh();
        }
        public static void RequestStatsRefresh()
        {
            ChangeStrength(0);
            ChangeBraincells(0);
        }
        [QFSW.QC.Command("-braincells")]
        public static void ChangeBraincells(int amount)
        {
            /*if (HardMode)
            {
                amount = amount.Min(0);
                Braincells = Braincells.Clamp(0, 9);
            }*/
            Braincells += amount;
            OnBraincellsChange?.Invoke(Braincells);
        }
        [QFSW.QC.Command("-strength")]
        public static void ChangeStrength(int amount)
        {
            if (HardMode)
            {
                Strength = 100;
                OnStrengthChange?.Invoke(Strength);
                return;
            }
            Strength += amount;
            OnStrengthChange?.Invoke(Strength);
        }
        public static void KillChangeStrength(float multiplier = 0.85f)
        {
            Strength = (Strength * multiplier).Floor().Max(100f).ToInt();
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
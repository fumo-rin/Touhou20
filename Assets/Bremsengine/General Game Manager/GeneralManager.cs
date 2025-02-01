using Core.Extensions;
using Core.Input;
using Mono.CSharp;
using System.Collections.Generic;
using UnityEngine;

namespace Bremsengine
{
    #region Pause
    #region Funny Explosion
    public partial class GeneralManager
    {
        [SerializeField] GameObject funnyExplosion;
        [SerializeField] AudioClipWrapper funnyExplosionSound;
        public static void FunnyExplosion(Vector2 position)
        {
            Destroy(Instantiate(Instance.funnyExplosion, position, Quaternion.identity),1.02f);
            Instance.funnyExplosionSound.Play(position);
        }
    }
    #endregion
    public partial class GeneralManager
    {
        public static bool IsPaused { get; private set; }
        private static float StoredPausedTimescale = 1f;
        public static void SetPause(bool state)
        {
            IsPaused = state;
            if (state)
            {
                //Pause
                StoredPausedTimescale = Time.timeScale;
                Time.timeScale = 0f;
                IsPaused = true;
                Debug.Log("Paused Game");
                PlayerInputController.actions.Player.Disable();
                PlayerInputController.actions.Shmup.Disable();
            }
            else
            {
                //Unpause
                Time.timeScale = StoredPausedTimescale;
                IsPaused = false;
                Debug.Log("Un-paused Game");
                PlayerInputController.actions.Player.Enable();
                PlayerInputController.actions.Shmup.Enable();
            }
        }
        [QFSW.QC.Command("-Pause")]
        public static void PauseGame()
        {
            SetPause(true);
        }
        [QFSW.QC.Command("-Unpause")]
        public static void UnPauseGame()
        {
            SetPause(false);
        }
        public static void TogglePause()
        {
            SetPause(!IsPaused);
        }
        [QFSW.QC.Command("-timescale")]
        public static void Command_SetTimescale(float timescale)
        {
            StoredPausedTimescale = timescale;
            UnPauseGame();
        }
    }
    #endregion
    #region Score
    public partial class GeneralManager
    {
        public static bool ShouldAddScoreKey => ScoreBreakdownAnalysis != null;
        public static void AddScoreAnalysisKey(string scoreKey, float score)
        {
            if (!ShouldAddScoreKey)
                return;
            if (!ScoreBreakdownAnalysis.ContainsKey(scoreKey))
                ScoreBreakdownAnalysis[scoreKey] = 0;
            ScoreBreakdownAnalysis[scoreKey] += score;
        }
        private static Dictionary<string, float> ScoreBreakdownAnalysis;
        [SerializeField] bool breakDownScore = false;
        public static float actualScore;
        public static float HighestScore { get; private set; }
        public static float VisibleScore;
        [SerializeField] float visibleScoreDivisor = 0.01f;
        [SerializeField] float visibleScoreMultiplier = 100f;
        static string HighScoreStringKey = "HiScore_Save";
        static bool IsHighscorePotentiallyOutOfSync = true;
        public delegate void ScoreAction(float score, float hiscore);
        public static ScoreAction OnScoreUpdate;
        public static float LoadHighScore()
        {
            ResyncHighscore();
            return HighestScore;
        }
        public static float ResetScore()
        {
            SetScoreValue(0f);
            return VisibleScore;
        }
        private static void SendUpdateScoreEvent(float scoreValue, float highScoreValue)
        {
            OnScoreUpdate?.Invoke(scoreValue, highScoreValue);
        }
        public static float AddScore(float value)
        {
            SetScoreValue(actualScore + value);
            return value;
        }
        public static void ApplyHighscoreToSave(float value)
        {
            PlayerPrefs.SetFloat(HighScoreStringKey, value);
        }
        static void ResyncHighscore()
        {
            if (!IsHighscorePotentiallyOutOfSync)
                return;

            IsHighscorePotentiallyOutOfSync = false;
            float loadedScore = PlayerPrefs.GetFloat(HighScoreStringKey);

            if (HighestScore < loadedScore) HighestScore = loadedScore;
        }
        private static void SetScoreValue(float value)
        {
            ResyncHighscore();
            actualScore = value;
            VisibleScore = (value.Multiply(Instance.visibleScoreDivisor)).Floor().Multiply(Instance.visibleScoreMultiplier);

            if (actualScore > HighestScore)
            {
                HighestScore = VisibleScore;
            }
            SendUpdateScoreEvent(VisibleScore, HighestScore);
        }
        private void OnApplicationQuit()
        {
            ApplyHighscoreToSave(LoadHighScore());
            if (ScoreBreakdownAnalysis != null)
            {
                foreach (var item in ScoreBreakdownAnalysis)
                {
                    string scoreMessage = "Score Breakdown##".ReplaceLineBreaks("##");
                    scoreMessage += $"Score Partition({item.Key}) : {item.Value.ToString("F0")}##".ReplaceLineBreaks("##");
                    Debug.Log(scoreMessage);
                }
            }
        }
    }
    #endregion
    public partial class GeneralManager : MonoBehaviour
    {
        public static GeneralManager Instance { get; private set; }
        private void Awake()
        {
            StartInstance();
        }
        private void OnDestroy()
        {
            if (Instance == this)
            {
                QCHelper.ReleaseCloseAction(UnPauseGame);
                QCHelper.ReleaseOpenAction(PauseGame);
                CloseInstance();
            }
        }
        private void Start()
        {
            if (Instance == this)
            {
                QCHelper.BindOpenAction(PauseGame);
                QCHelper.BindCloseAction(UnPauseGame);
                IsHighscorePotentiallyOutOfSync = true;
                SetScoreValue(0f);
            }
        }
        private void StartInstance()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (breakDownScore)
            {
                ScoreBreakdownAnalysis = new();
            }
        }
        private void CloseInstance()
        {
            if (Instance != this)
                return;
            Instance = null;
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ReInitialize()
        {
            Instance = null;
            ScoreBreakdownAnalysis = null;
        }
    }
}

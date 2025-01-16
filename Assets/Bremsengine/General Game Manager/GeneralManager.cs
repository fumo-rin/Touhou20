using Core.Extensions;
using Core.Input;
using UnityEngine;

namespace Bremsengine
{
    #region Pause
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
            }
            else
            {
                //Unpause
                Time.timeScale = StoredPausedTimescale;
                IsPaused = false;
                Debug.Log("Un-paused Game");
                PlayerInputController.actions.Player.Enable();
            }
        }
        public static void PauseGame()
        {
            SetPause(true);
        }
        public static void UnPauseGame()
        {
            SetPause(false);
        }
        public static void TogglePause()
        {
            SetPause(!IsPaused);
        }
    }
    #endregion
    #region Score
    public partial class GeneralManager
    {
        public static float actualScore;
        public static float HighestScore { get ; private set; }
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
        private static void SendUpdateScoreEvent(float scoreValue, float highScoreValue)
        {
            OnScoreUpdate?.Invoke(scoreValue, highScoreValue);
        }
        public static void AddScore(float value)
        {
            SetScoreValue(actualScore + value);
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
            Debug.Log(VisibleScore);
            Debug.Log((value.Multiply(Instance.visibleScoreDivisor)).Floor());

            if (actualScore > HighestScore)
            {
                HighestScore = VisibleScore;
            }
            SendUpdateScoreEvent(VisibleScore, HighestScore);
        }
        private void OnApplicationQuit()
        {
            ApplyHighscoreToSave(LoadHighScore());
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
            QCHelper.ReleaseCloseAction(UnPauseGame);
            QCHelper.ReleaseOpenAction(PauseGame);
            CloseInstance();
        }
        private void Start()
        {
            QCHelper.BindOpenAction(PauseGame);
            QCHelper.BindCloseAction(UnPauseGame);
            IsHighscorePotentiallyOutOfSync = true;
            SetScoreValue(0f);
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
        }
    }
}

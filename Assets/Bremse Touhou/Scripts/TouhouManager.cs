using Bremsengine;
using Core.Extensions;
using Core.Input;
using Dan;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BremseTouhou
{
    #region Boss Kill Sound
    public partial class TouhouManager
    {
        [SerializeField] AudioClipWrapper bossKillSound;
        public static void PlayBossKillSound(Vector2 position)
        {
            if (instance != null)
            {
                instance.bossKillSound.Play(position);
            }
            GeneralManager.FunnyExplosion(position);
        }
    }
    #endregion
    #region Difficulty
    public partial class TouhouManager
    {
        public static string GetDifficultyName(Difficulty d)
        {
            string difficultyName = "";
            switch (d)
            {
                case Difficulty.Easy: difficultyName += "Easy"; break;
                case Difficulty.Normal: difficultyName += "Normal"; break;
                case Difficulty.Hard: difficultyName += "Hard"; break;
                case Difficulty.Lunatic: difficultyName += "Lunatic"; break;
                case Difficulty.Ultra: difficultyName += "Ultra"; break;
                case Difficulty.Extra: difficultyName += "Extra"; break;
                default: difficultyName = "Normal"; SetDifficulty(Difficulty.Normal); break;
            }
            return difficultyName;
        }
        public static Color32 GetDifficultyColor(Difficulty d)
        {
            switch (d)
            {
                case TouhouManager.Difficulty.Easy:
                    return ColorHelper.PastelGreen;
                case TouhouManager.Difficulty.Normal:
                    return ColorHelper.PastelBlue;
                case TouhouManager.Difficulty.Hard:
                    return ColorHelper.PastelYellow;
                case TouhouManager.Difficulty.Lunatic:
                    return ColorHelper.PastelPurple;
                case TouhouManager.Difficulty.Ultra:
                    return ColorHelper.PastelOrange;
                case TouhouManager.Difficulty.Extra:
                    return ColorHelper.PastelRed;
                default:
                    return Color.blue;
            }
        }
        public enum Difficulty
        {
            Easy = 1,
            Normal = 2,
            Hard = 3,
            Lunatic = 4,
            Ultra = 5,
            Extra = 6
        }
        static Difficulty SelectedDifficulty = Difficulty.Normal;
        static string LastDifficultyStringKey = "Last Picked Difficulty";
        public delegate void DifficultyEvent(Difficulty d, string difficultyName);
        public static DifficultyEvent OnDifficultyChange;
        public static int missCount { get; private set; }
        public static void AddMiss()
        {
            missCount++;
        }
        public static Difficulty CurrentDifficulty => SelectedDifficulty;
        [QFSW.QC.Command("-set-difficulty")]
        public static void SetDifficulty(Difficulty d)
        {
            PlayerPrefs.SetInt(LastDifficultyStringKey, (int)d);
            SelectedDifficulty = d;
            RequestDifficultyUpdateEvent();
        }
        private static void SendDifficultyUpdateEvent(Difficulty d)
        {
            OnDifficultyChange?.Invoke(d, GetDifficultyName(d));
        }
        public static void RequestDifficultyUpdateEvent()
        {
            SendDifficultyUpdateEvent(SelectedDifficulty);
        }
        public static void LoadDifficultyFromLastSelected()
        {
            if (PlayerPrefs.HasKey(LastDifficultyStringKey) && PlayerPrefs.GetInt(LastDifficultyStringKey) is int difficulty and >= 0)
            {
                SetDifficulty((Difficulty)difficulty);
            }
            else
            {
                SetDifficulty(Difficulty.Normal);
            }
        }
    }
    #endregion
    #region Game Progress
    public partial class TouhouManager
    {
        static int progress;
        Dictionary<int, System.Action> ProgressDictionary = new();
        Dictionary<int, float> ProgressDuration = new();
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void ResetProgress()
        {
            OnSetProgress = null;
            progress = 0;
        }
        public static void DefineProgressTree(int progress, System.Action action)
        {
            if (!instance.ProgressDictionary.ContainsKey(progress))
                instance.ProgressDictionary.Add(progress, action);
            else
                instance.ProgressDictionary[progress] += action;
        }
        public static void MoveToGreatestProgress(int greatestProgress)
        {
            SetProgress(greatestProgress);
        }
        public static void DefineProgressTreeDuration(int progress, float seconds)
        {
            instance.ProgressDuration[progress] = seconds;
        }
        public static bool TryGetProgressDuration(int progress, out float duration)
        {
            duration = 0f;
            if (instance.ProgressDuration.ContainsKey(progress))
            {
                duration = instance.ProgressDuration[progress];
                return true;
            }
            return false;
        }
        private static void MoveToNextProgressAfter(float seconds)
        {
            IEnumerator CO_WaitAndSetNextProgress(float seconds)
            {
                yield return new WaitForSeconds(seconds);
                MoveToNextProgress();
            }
            instance.StartCoroutine(CO_WaitAndSetNextProgress(seconds));
        }
        [QFSW.QC.Command("-set-progress")]
        private static void SetProgress(int newProgress)
        {
            if (instance.ProgressDictionary.TryGetValue(newProgress, out Action action))
            {
                action?.Invoke();
                OnSetProgress?.Invoke(progress);
                if (TryGetProgressDuration(progress, out float duration))
                {
                    MoveToNextProgressAfter(duration);
                }
                progress = newProgress + 1;
            }
            else
            {
                GameEnd();
                return;
            }
        }
        public delegate void StageProgressEvent(int progress);
        public static StageProgressEvent OnSetProgress;
        public static void MoveToNextProgress()
        {
            SetProgress(progress);
        }
    }
    #endregion
    #region Temporary UI Solutions

    #endregion
    #region Game End
    public partial class TouhouManager
    {
        public static void GameEnd()
        {
            PlayerInputController.actions.Shmup.Disable();
            KillAllBosses();
            float livesScore = 20000000 * PlayerUnit.PlayerExtraLives;
            GeneralManager.AddScore(livesScore);
            GeneralManager.AddScoreAnalysisKey("Lives at Game End", livesScore);
            IEnumerator LoadLeaderboard()
            {
                yield return new WaitForSeconds(1f);
                SetActiveUI(instance.leaderBoardContainer);
            }
            instance.StartCoroutine(LoadLeaderboard());
        }
        public static void MainMenu()
        {
            if ((instance.lastUISelection != instance.MainMenuContainer && instance.lastUISelection != instance.leaderBoardContainer))
            {
                instance.MainMenuMusic.Play();
            }
            PlayerInputController.actions.Shmup.Disable();
            SetActiveUI(instance.MainMenuContainer);
        }
        public static void StartGame()
        {
            PlayerBombAction.Reinitialize();
            GrazeBox.ResetGrazeCount();
            PlayerScoring.Reinitialize();
            PlayerInputController.actions.Shmup.Enable();
            KillAllBosses();
            Projectile.ClearAllProjectiles();
            SetActiveUI(instance.GameUIContainer);
            TouhouManager.SetDifficulty(Difficulty.Ultra);
            GeneralManager.ResetScore();
            TouhouManager.progress = 0;
            TouhouManager.MoveToNextProgress();
            instance.spawnedBosses.Clear();
            missCount = 0;
            PlayerUnit.SetLives(9);
            BaseUnit.Player.transform.position = (BaseUnit.Player.Origin);
        }
        private static void SetActiveUI(GameObject ui)
        {
            instance.GameUIContainer.SetActive(false);
            instance.MainMenuContainer.SetActive(false);
            instance.GameContainer.SetActive(false);
            if (ui != null)
            {
                ui.SetActive(true);
                if (ui == instance.GameUIContainer)
                {
                    instance.GameContainer.SetActive(true);
                }
            }
            if (instance.leaderBoardContainer.GetComponent<LeaderboardUI>() is LeaderboardUI leaderBoard and not null)
            {
                leaderBoard.HideLeaderboard();
                if (ui == leaderBoard.gameObject)
                {
                    leaderBoard.ShowLeaderboard();
                    if ((instance.lastUISelection != instance.MainMenuContainer && instance.lastUISelection != instance.leaderBoardContainer))
                    {
                        instance.MainMenuMusic.Play();
                    }
                }
            }
            instance.lastUISelection = ui;
        }
        [SerializeField] CreditsLoader credits;
        public void ShowCredits()
        {
            SetActiveUI(null);
            credits.StartCredits();
        }
        public void HideCredits()
        {
            SetActiveUI(MainMenuContainer);
            credits.EndCredits();
        }
    }
    #endregion
    public partial class TouhouManager : MonoBehaviour
    {
        [SerializeField] MusicWrapper MainMenuMusic;
        [SerializeField] GameObject GameUIContainer;
        [SerializeField] GameObject GameContainer;
        [SerializeField] GameObject MainMenuContainer;
        [SerializeField] GameObject leaderBoardContainer;
        GameObject lastUISelection;
        HashSet<BaseUnit> spawnedBosses = new HashSet<BaseUnit>();
        public static void AddBoss(BaseUnit boss)
        {
            instance.spawnedBosses.Add(boss);
        }
        public static void KillAllBosses()
        {
            BaseUnit.KillSettings o = new(0);
            o.BypassPhase = true;
            foreach (var item in instance.spawnedBosses)
            {
                if (item != null)
                {
                    item.ForceKill(o);
                }
            }
        }
        static TouhouManager instance;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialze()
        {
            instance = null;
            missCount = 0;
        }
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            transform.SetParent(null);
            Time.maximumDeltaTime = 1f / 60f;
            Time.maximumParticleDeltaTime = 1f / 60f;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 120;
            LoadDifficultyFromLastSelected();
        }
        public static void CloseGame()
        {
            Application.Quit();
        }
        private void Start()
        {
            MainMenu();
        }
    }
}

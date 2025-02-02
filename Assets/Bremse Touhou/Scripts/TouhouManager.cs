using Bremsengine;
using Core.Extensions;
using Core.Input;
using Dan;
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
        static Dictionary<int, System.Action> ProgressDictionary = new();
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void ResetProgress()
        {
            ProgressDictionary.Clear();
            progress = 0;
        }
        public static void DefineProgressTree(int progress, System.Action action)
        {
            if (!ProgressDictionary.ContainsKey(progress))
                ProgressDictionary.Add(progress, action);
            else
                ProgressDictionary[progress] += action;
        }
        public static void MoveToNextProgress()
        {
            if (!ProgressDictionary.ContainsKey(progress))
            {
                GameEnd();
                return;
            }
            ProgressDictionary[progress]?.Invoke();
            progress += 1;
        }
        public static void DefineNextProgress(System.Action action)
        {
            DefineProgressTree(progress, action);
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
            instance.MainMenuMusic.Play();
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
                }
            }
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
        HashSet<GameObject> spawnedBosses = new HashSet<GameObject>();
        public static void AddBoss(GameObject boss)
        {
            instance.spawnedBosses.Add(boss);
        }
        public static void KillAllBosses()
        {
            foreach (var item in instance.spawnedBosses)
            {
                if (item != null)
                {
                    BossHealthbar.
                    Destroy(item);
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
            DontDestroyOnLoad(gameObject);
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

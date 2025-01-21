using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    #region Boss Kill Sound
    public partial class TouhouManager
    {
        [SerializeField] AudioClipWrapper bossKillSound;
        public static void PlayBossKillSound()
        {
            if (instance != null)
            {
                instance.bossKillSound.Play(Vector2.zero);
            }
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
            if (PlayerPrefs.HasKey(LastDifficultyStringKey) && PlayerPrefs.GetInt(LastDifficultyStringKey) is int difficulty and <= 0)
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
    public partial class TouhouManager : MonoBehaviour
    {
        static TouhouManager instance;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialze()
        {
            instance = null;
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
        }
        public static void CloseGame()
        {
            Application.Quit();
        }
    }
}

using Bremsengine;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core.Extensions;

namespace ChurroIceDungeon
{
    #region Graze
    public partial class WakaPlayerUI
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ReinitializeGraze()
        {
            grazeCount = 0;
        }
        [SerializeField] TMP_Text grazeCountText;
        static int grazeCount = 0;
        private void ReceiveGrazeValues(int grazeCount)
        {
            grazeCountText.text = grazeCount.ToString();
        }
    }
    #endregion
    #region Score
    public partial class WakaPlayerUI
    {
        [SerializeField] TMP_Text scoreText;
        [SerializeField] TMP_Text hiScoreText;
        [SerializeField] TMP_Text scoreItemValue;
        static float storedScore;
        static float storedHiscore;
        bool wasChanged;
        private void ReceiveScoreValues(float score, float hiScore)
        {
            storedScore = score;
            storedHiscore = hiScore;
            wasChanged = true;
        }
        private void UpdateScoreUI()
        {
            if (wasChanged)
            {
                ForceScoreUpdate();
            }
        }
        private void ForceScoreUpdate()
        {
            wasChanged = false;
            scoreText.text = storedScore.ToString("F0");
            hiScoreText.text = storedHiscore.ToString("F0");
            if (WakaScoring.HasInstance)
            {
                scoreItemValue.text = WakaScoring.ScoreItemText();
            }
            else
            {
                scoreItemValue.text = "5000 x 1.000";
            }
        }
    }
    #endregion
    #region Lives
    public partial class WakaPlayerUI
    {
        [SerializeField] Slider healthBarSlider;
        void UpdateLivesUI(int newLives, int maxLives)
        {
            healthBarSlider.maxValue = maxLives;
            healthBarSlider.value = newLives;
        }
    }
    #endregion
    #region Difficulty
    public partial class WakaPlayerUI
    {
        [SerializeField] TMP_Text difficultyText;
        private void SetDifficultyUI(GeneralManager.Difficulty d)
        {
            difficultyText.text = GeneralManager.GetDifficultyName(d).Color(GeneralManager.GetDifficultyColor(d));
        }
    }
    #endregion
    public partial class WakaPlayerUI : MonoBehaviour
    {
        private void Start()
        {
            wasChanged = true;
            GeneralManager.OnScoreUpdate += ReceiveScoreValues;
            TickManager.MainTickLightweight += UpdateScoreUI;
            WakaUnit.OnLivesChanged += UpdateLivesUI;
            WakaUnit.RequestLivesRefresh();
            GrazeBox.OnGraze += ReceiveGrazeValues;
            GeneralManager.OnDifficultyChanged += SetDifficultyUI;
            SetDifficultyUI(GeneralManager.CurrentDifficulty);
        }
        private void OnDestroy()
        {
            GeneralManager.OnScoreUpdate -= ReceiveScoreValues;
            TickManager.MainTickLightweight -= UpdateScoreUI;
            WakaUnit.OnLivesChanged -= UpdateLivesUI;
            GrazeBox.OnGraze -= ReceiveGrazeValues;
            GeneralManager.OnDifficultyChanged += SetDifficultyUI;
        }
    }
}

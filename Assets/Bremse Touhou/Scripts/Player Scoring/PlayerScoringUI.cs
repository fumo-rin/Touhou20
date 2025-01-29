using Bremsengine;
using Core.Extensions;
using System.Collections;
using TMPro;
using UnityEngine;

namespace BremseTouhou
{
    public class PlayerScoringUI : MonoBehaviour
    {
        [SerializeField] TMP_Text HighscoreText;
        [SerializeField] TMP_Text ActiveScoreText;
        [SerializeField] TMP_Text ScoreItemText;
        static float storedScore;
        static float storedHiScore;
        bool scoreChanged;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Reinitialize()
        {
            storedScore = 0;
            storedHiScore = 0;
        }
        public IEnumerator Start()
        {
            GeneralManager.OnScoreUpdate += SetNewScore;
            FetchScore();
            while (true)
            {
                SetScoreUI(storedScore, storedHiScore);
                yield return Helper.GetWaitForSeconds(0.05f);
            }
        }
        private void OnDestroy()
        {
            GeneralManager.OnScoreUpdate -= SetNewScore;
        }
        public void SetNewScore(float score, float hiScore)
        {
            scoreChanged = true;
            storedScore = score;
            storedHiScore = hiScore;
        }
        private void SetScoreUI(float score, float hiScore)
        {
            if (!scoreChanged)
            {
                return;
            }
            HighscoreText.text = hiScore.Floor().ToString("F0");
            ActiveScoreText.text = score.Floor().ToString("F0");
            ScoreItemText.text = PlayerScoring.ScoreItemText();
        }
        private void FetchScore()
        {
            SetNewScore(GeneralManager.actualScore, GeneralManager.HighestScore);
        }
    }
}

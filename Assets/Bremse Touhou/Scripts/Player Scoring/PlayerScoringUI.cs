using Bremsengine;
using Core.Extensions;
using TMPro;
using UnityEngine;

namespace BremseTouhou
{
    public class PlayerScoringUI : MonoBehaviour
    {
        [SerializeField] TMP_Text HighscoreText;
        [SerializeField] TMP_Text ActiveScoreText;
        [SerializeField] TMP_Text ScoreItemText;

        public void Start()
        {
            GeneralManager.OnScoreUpdate += SetScoreUI;
            FetchScore();
        }
        private void OnDestroy()
        {
            GeneralManager.OnScoreUpdate -= SetScoreUI;
        }
        public void SetScoreUI(float score, float hiScore)
        {
            HighscoreText.text = hiScore.Floor().ToString("F0");
            ActiveScoreText.text = score.Floor().ToString("F0");
            ScoreItemText.text = PlayerScoring.ScoreItemText();
        }
        private void FetchScore()
        {
            SetScoreUI(GeneralManager.actualScore, GeneralManager.HighestScore);
        }
    }
}

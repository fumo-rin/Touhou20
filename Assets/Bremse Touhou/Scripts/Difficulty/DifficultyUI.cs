using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BremseTouhou
{
    public class DifficultyUI : MonoBehaviour
    {
        [SerializeField] TMP_Text difficultyText;
        private void Start()
        {
            TouhouManager.OnDifficultyChange += LoadDifficultyEvent;
            TouhouManager.RequestDifficultyUpdateEvent();
        }
        private void OnDestroy()
        {
            TouhouManager.OnDifficultyChange += LoadDifficultyEvent;
        }
        private void LoadDifficultyEvent(TouhouManager.Difficulty d, string difficultyText)
        {
            this.difficultyText.text = difficultyText;
            this.difficultyText.color = TouhouManager.GetDifficultyColor(d);
        }
    }
}

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
            Color32 vertexColor = this.difficultyText.color;
            switch (d)
            {
                case TouhouManager.Difficulty.Easy:
                    vertexColor = Color.green;
                    break;
                case TouhouManager.Difficulty.Normal:
                    vertexColor = Color.blue;
                    break;
                case TouhouManager.Difficulty.Hard:
                    vertexColor = Color.yellow;
                    break;
                case TouhouManager.Difficulty.Lunatic:
                    vertexColor = Color.magenta;
                    break;
                case TouhouManager.Difficulty.Ultra:
                    vertexColor = Color.red;
                    break;
                case TouhouManager.Difficulty.Extra:
                    vertexColor = Color.white;
                    break;
                default:
                    break;
            }
            this.difficultyText.color = vertexColor;
        }
    }
}

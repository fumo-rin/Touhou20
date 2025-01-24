using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BremseTouhou
{
    public class GrazeUI : MonoBehaviour
    {
        [SerializeField] TMP_Text grazeText;
        [SerializeField] Slider grazeSlider;
        [SerializeField] Animator grazeSliderAnimator;
        [SerializeField] string grazeSliderMaxValueKey = "MAX_GRAZE";
        static GrazeUI instance;
        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {
            RebuildGraze(GrazeBox.GrazeCount);
            GrazeBox.OnGraze += RebuildGraze;
            PlayerScoring.OnGrazeRefresh += RebuildGrazeSlider;
        }
        private void OnDestroy()
        {
            GrazeBox.OnGraze -= RebuildGraze; 
            PlayerScoring.OnGrazeRefresh -= RebuildGrazeSlider;
        }
        public static void RebuildGraze(int grazeCount)
        {
            if (instance == null || instance.grazeText == null)
                return;
            instance.grazeText.text = grazeCount.ToString();
        }
        public void RebuildGrazeSlider(float value, float min, float max)
        {
            grazeSlider.minValue = min;
            grazeSlider.maxValue = max;
            grazeSlider.value = value;
            grazeSliderAnimator.SetBool(grazeSliderMaxValueKey, value == max);
        }
    }
}

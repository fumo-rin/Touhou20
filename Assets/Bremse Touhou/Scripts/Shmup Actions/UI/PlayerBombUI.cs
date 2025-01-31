using UnityEngine;
using UnityEngine.UI;

namespace BremseTouhou
{
    public class PlayerBombUI : MonoBehaviour
    {
        [SerializeField] Slider bombSlider;
        private void Start()
        {
            PlayerBombAction.OnRefresh += (int current, int max) => { bombSlider.maxValue = max; bombSlider.value = current; };
            PlayerBombAction.RequestRefresh();
        }
        private void OnDestroy()
        {
            PlayerBombAction.OnRefresh -= (int current, int max) => { bombSlider.maxValue = max; bombSlider.value = current; };
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.UI;

namespace BremseTouhou
{
    public class SimplePlayerHealthBar : MonoBehaviour
    {
        [SerializeField] Slider playerHealthSlider;
        private void Start()
        {
            PlayerUnit.OnLivesUpdate += (int current, int max) => { playerHealthSlider.maxValue = max; playerHealthSlider.value = current; };
            PlayerUnit.RequestHealthRefresh();
        }
        private void OnDestroy()
        {
            PlayerUnit.OnLivesUpdate -= (int current, int max) => { playerHealthSlider.maxValue = max; playerHealthSlider.value = current; };
        }
    }
}

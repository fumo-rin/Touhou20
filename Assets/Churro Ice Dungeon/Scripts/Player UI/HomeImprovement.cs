using UnityEngine;
using UnityEngine.UI;

namespace ChurroIceDungeon
{
    [DefaultExecutionOrder(50)]
    public class HomeImprovement : MonoBehaviour
    {
        [SerializeField] Slider slider;
        private void Start()
        {
            ChurroManager.OnDestructionRefresh += SetBar;
            ChurroManager.RequestDestructionBarRefresh();
        }
        private void OnDestroy()
        {
            ChurroManager.OnDestructionRefresh -= SetBar;
        }
        private void SetBar(float current, float stageCap)
        {
            slider.maxValue = stageCap;
            slider.value = current;
        }
    }
}
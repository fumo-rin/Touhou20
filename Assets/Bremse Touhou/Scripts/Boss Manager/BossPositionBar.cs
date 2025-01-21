using Bremsengine;
using UnityEngine;
using UnityEngine.UI;

namespace BremseTouhou
{
    public class BossPositionBar : MonoBehaviour
    {
        BaseUnit trackedBoss;
        [SerializeField] GameObject sliderHandle;
        [SerializeField] Slider slider;
        public void SetTrackedBoss(BaseUnit boss)
        {
            trackedBoss = boss;
        }
        private void Start()
        {
            Bounds bounds = DirectionSolver.GetPaddedBounds(0f);
            slider.minValue = bounds.min.x;
            slider.maxValue = bounds.max.x;
            sliderHandle.SetActive(false);
        }
        void Update()
        {
            if (trackedBoss == null)
            {
                Destroy(gameObject);
                return;
            }
            sliderHandle.SetActive(trackedBoss.gameObject.activeInHierarchy);
            slider.value = trackedBoss.transform.position.x;
        }
    }
}

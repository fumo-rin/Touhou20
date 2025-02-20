using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChurroIceDungeon
{
    public class Bossbar : MonoBehaviour
    {
        static Bossbar instance;
        private void Awake()
        {
            instance = this;
        }
        [SerializeField] Slider bossSlider;
        [SerializeField] GameObject barContainer;
        [SerializeField] TMP_Text bossNameText;
        DungeonUnit activeUnit;
        bool failCondition => activeUnit == null || !activeUnit.IsAlive();
        private void Update()
        {
            barContainer.SetActive(!failCondition);
            if (failCondition)
            {
                return;
            }
            bossSlider.value = activeUnit.Health;
        }
        public static void BindBar(DungeonUnit unit)
        {
            instance.activeUnit = unit;
            if (unit == null)
            {
                return;
            }
            instance.bossNameText.text = unit.name;
            instance.bossSlider.maxValue = unit.Health;
        }
    }
}

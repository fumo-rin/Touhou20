using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core.Extensions;

namespace BremseTouhou
{
    [DefaultExecutionOrder(10)]
    public class BossHealthbar : MonoBehaviour
    {
        [SerializeField] Slider slider;
        [SerializeField] TMP_Text healthText;
        [SerializeField] TMP_Text unitName;
        BaseUnit unit;
        bool needsChange;
        private void Awake()
        {
            unitName.text = "Cool Boss Name";
        }
        private void LateUpdate()
        {
            Refresh();
        }
        private void Refresh()
        {
            if (!needsChange)
                return;

            if (unit == null)
                Destroy(gameObject);

            if (unitName.text == "Cool Boss Name")
            {
                unitName.text = unit.UnitName;
            }

            healthText.text = unit.HealthText;
            slider.maxValue = unit.MaxHealth;
            slider.value = unit.CurrentHealth;

            needsChange = false;
        }
        public void SetHealthUI(BaseUnit unit)
        {
            if (unit == null)
                return;

            this.unit = unit;
            needsChange = true;
        }
    }
}

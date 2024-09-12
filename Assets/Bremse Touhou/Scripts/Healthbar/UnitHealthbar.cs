using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BremseTouhou
{
    public class UnitHealthbar : MonoBehaviour
    {
        [SerializeField] BaseUnit owner;
        [SerializeField] TMP_Text healthText;
        [SerializeField] Slider healthSlider;
        private void SetHealthData(BaseUnit unit)
        {
            healthSlider.maxValue = unit.MaxHealth; 
            healthSlider.value = unit.CurrentHealth.Max(0f);
            if (healthText != null)
            {
                healthText.text = unit.HealthText;
            }
        }
        private void Awake()
        {
            if (owner == null)
            {
                gameObject.SetActive(false);
            }
            owner.OnHealthChange += SetHealthData;
        }
        private void OnDestroy()
        {
            owner.OnHealthChange -= SetHealthData;
        }
    }
}

using TMPro;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class ChurroStatsUI : MonoBehaviour
    {
        [SerializeField] TMP_Text strengthText;
        [SerializeField] TMP_Text braincellsText;
        private void Start()
        {
            ChurroManager.OnStrengthChange += SetStrength;
            ChurroManager.OnBraincellsChange+= SetBraincells;
            ChurroManager.RequestStatsRefresh();
        }
        private void OnDestroy()
        {
            ChurroManager.OnStrengthChange -= SetStrength;
            ChurroManager.OnBraincellsChange -= SetBraincells;
        }
        public void SetStrength(int newValue)
        {
            strengthText.text = newValue.ToString("F0");
        }
        public void SetBraincells(int newValue)
        {
            braincellsText.text = newValue.ToString("F0");
        }
    }
}

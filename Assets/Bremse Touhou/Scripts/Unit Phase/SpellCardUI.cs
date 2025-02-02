using Bremsengine;
using Core.Extensions;
using Core.Input;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace BremseTouhou
{
    public class SpellCardUI : MonoBehaviour
    {
        [SerializeField] TMP_Text spellNameText;
        [SerializeField] TMP_Text spellValueText;
        [SerializeField] TMP_Text spellTimeText;
        [SerializeField] Slider phaseCountSlider;
        public static PhaseEntry currentPhase;
        public static float spellBonus;
        public static float spellBonusIncrease;
        public static float spellBonusCurrentDelay;
        public static float spellTimeRemaining;
        public static SpellCardUI activeRunner;
        [SerializeField] BremseInputEventBus bombAction;
        public static void SetPhase(PhaseEntry e)
        {
            if (e == null)
            {
                Debug.LogError("Set Null Phase");
                return;
            }
            currentPhase = e;
            spellBonus = e.phaseBonus;
            spellBonusIncrease = e.phaseBonusIncrease;
            spellBonusCurrentDelay = e.phaseBonusIncreaseDelay;
            spellTimeRemaining = e.phaseLength;
            activeRunner.RecalculatePhaseVisibility();
        }
        public static void HideUI()
        {
            if (activeRunner == null)
            {
                Debug.LogWarning("Missing Instance of Spellcard UI");
                return;
            }
            activeRunner.spellNameText.gameObject.SetActive(false);
            activeRunner.spellTimeText.gameObject.SetActive(false);
            activeRunner.spellValueText.gameObject.SetActive(false);
            activeRunner.phaseCountSlider.gameObject.SetActive(false);
        }
        public void RecalculatePhaseVisibility()
        {
            if (currentPhase == null || currentPhase.IsLastPhase)
            {
                HideUI();
                return;
            }
            activeRunner.spellNameText.gameObject.SetActive(currentPhase.IsSpellCard);
            activeRunner.spellTimeText.gameObject.SetActive(currentPhase.phaseLength > 0f);
            activeRunner.spellValueText.gameObject.SetActive(currentPhase.IsSpellCard && currentPhase.phaseBonus > 0f);
            activeRunner.phaseCountSlider.gameObject.SetActive(true);
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Reinitialize()
        {
            activeRunner = null;
            spellBonusCurrentDelay = 0f;
            spellBonus = 0f;
            spellBonusIncrease = 0f;
            spellTimeRemaining = 0f;
        }
        private void Awake()
        {
            if (activeRunner == null)
                activeRunner = this;
        }
        private void BombInput()
        {
            FailSpell();
        }
        private void Start()
        {
            InvokeRepeating(nameof(UILoop), 0f, 0.05f);
            RecalculatePhaseVisibility();
            bombAction.BindAction(BremseInputPhase.Performed, BombInput);
        }
        private void OnDestroy()
        {
            bombAction.ReleaseAction(BremseInputPhase.Performed, BombInput);
        }
        private void UILoop()
        {
            if (activeRunner == this)
            {
                ProgressSpell(0.05f);
            }
            BuildPhaseText();
        }
        public static void SetPhaseSlider(int total, int phasesLeft)
        {
            activeRunner.phaseCountSlider.maxValue = total - 1;
            activeRunner.phaseCountSlider.value = phasesLeft - 1;
        }
        private static void ProgressSpell(float time)
        {
            if (currentPhase == null)
                return;
            spellTimeRemaining -= time;
            if (spellTimeRemaining <= 0f)
            {
                spellBonus = 0f;
                currentPhase.ForceNextPhase();
                return;
            }
            if (spellBonus > 0f && spellBonusCurrentDelay > 0f)
            {
                spellBonusCurrentDelay -= time;
                return;
            }
            if (spellBonus > 0 && spellBonusIncrease > 0f)
            {
                spellBonus += time * spellBonusIncrease;
                return;
            }
            spellBonus = 0f;
        }
        public static void CompleteSpell()
        {
            if (currentPhase == null)
                return;
            if (currentPhase.IsSpellCard && spellBonus > 0f && spellTimeRemaining > 0f)
            {
                GeneralManager.AddScore(SpellScore);
                GeneralManager.AddScoreAnalysisKey("Spell Bonus", spellBonus);
            }
            spellBonus = 0f;
        }
        public static int SpellScore => (int)((spellBonus.Multiply(0.001f)).Floor()).Multiply(1000f);
        public static void FailSpell()
        {
            spellBonus = 0;
            activeRunner.UILoop();
        }
        private void BuildPhaseText()
        {
            if (currentPhase == null)
            {
                return;
            }
            spellNameText.text = currentPhase.mainAttack.projectileGraphName;
            spellTimeText.text = spellTimeRemaining < 0.05f ? 0f.ToString("F0") : spellTimeRemaining.Ceil().ToString("F0");
            if (currentPhase.IsSpellCard)
            {
                if (spellBonus > 0f)
                {
                    spellValueText.text = SpellScore.ToString("F0");
                }
                else
                {
                    spellValueText.text = "Failed";
                }
            }
            else
            {
                spellValueText.text = "";
            }
        }
    }
}
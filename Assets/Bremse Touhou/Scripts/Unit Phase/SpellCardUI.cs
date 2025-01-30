using Bremsengine;
using Core.Extensions;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace BremseTouhou
{
    public class SpellCardUI : MonoBehaviour
    {
        [SerializeField] TMP_Text spellText;
        public static PhaseEntry currentPhase;
        public static float spellBonus;
        public static float spellBonusDecay;
        public static SpellCardUI activeRunner;
        public static void SetPhase(PhaseEntry e)
        {
            currentPhase = e;
            spellBonus = e.phaseBonus;
            spellBonusDecay = e.phaseBonusDecay;
            activeRunner.HidePhase();
        }
        public void HidePhase()
        {
            spellText.gameObject.SetActive(currentPhase.IsSpellCard);
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Reinitialize()
        {
            activeRunner = null;
        }
        private void Awake()
        {
            activeRunner = this;
        }
        private void Start()
        {
            InvokeRepeating(nameof(UILoop), 0f, 0.05f);
            HidePhase();
        }
        private void UILoop()
        {
            if (activeRunner == this)
            {
                ProgressSpell(0.05f);
            }
            spellText.text = BuildPhaseText();
        }
        private static void ProgressSpell(float time)
        {
            if (spellBonus > 0)
            {
                spellBonus -= time * spellBonusDecay;
                return;
            }
            spellBonus = 0f;
        }
        public static void CompleteSpell()
        {
            if (currentPhase.IsSpellCard && spellBonus > 0f)
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
        private string BuildPhaseText()
        {
            string spellName = currentPhase.mainAttack.projectileGraphName;
            return spellName + "##".ReplaceLineBreaks("##") + (SpellScore > 0 ? (SpellScore) : "FAILED");
        }
    }
}
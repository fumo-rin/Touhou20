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
            Debug.Log(e.phaseBonus);
            spellBonus = e.phaseBonus;
            spellBonusDecay = e.phaseBonusDecay;
        }
        private void AssignRunner()
        {
            activeRunner = GetComponent<SpellCardUI>();
        }
        private void Start()
        {
            InvokeRepeating(nameof(UILoop), 0f, 0.05f);
        }
        private void UILoop()
        {
            if (activeRunner == null || !activeRunner.gameObject.activeInHierarchy)
            {
                AssignRunner();
            }
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
            if (spellBonus > 0f)
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
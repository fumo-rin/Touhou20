using Core.Extensions;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Bremsengine
{
    public class TestDialogue : Dialogue
    {
        protected override IEnumerator DialogueContents()
        {
            ReDrawDialogue("Test Text");
            SetButton(0, "Yes", TestFeature).SetContinueWhenPressed();
            yield return Wait;
            ReDrawDialogue("Mewo mewo");
            SetButton(0, "Yes").SetContinueWhenPressed();
            SetButton(1, "+100 money", TestFeature);
            SetButton(2, "NOOO", SpawnBoss);
            yield return Wait;
            ReDrawDialogue("yooo");
            SetButton(2, "Bro").SetContinueWhenPressed();
            StartSubroutine("Test Range", TestRange());
            yield return Wait;
            TryEndSubroutine("Test Range");
            ReDrawDialogue("jao");
            SetButton(2, "Close", SpawnBoss).SetContinueWhenPressed();
            yield return Wait;
            ForceEndDialogue();
        }
        private void SpawnBoss()
        {
            DialogueEventBus.TriggerEvent(EventKeys.Skeletron);
        }
        private IEnumerator TestRange()
        {
            string add = " ";
            foreach (var item in 30f.StepFromTo(-100f, 360f))
            {
                add += item + " ";
            }
            foreach (char item in add.StringChop())
            {
                activeText.AddText(item);
                yield return Helper.GetWaitForSeconds(1f / 30f);
            }
        }
        protected override void WhenStartDialogue()
        {

        }
        protected override void WhenEndDialogue()
        {

        }
        private void TestFeature()
        {
            activeText.AddText(" 100 money :)");
            UnityEngine.Debug.Log("100 moneys fortnite burger");
        }

    }
}

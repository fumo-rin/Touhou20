using System.Collections;
using UnityEngine;

namespace Bremsengine
{
    public class TestDialogue : Dialogue
    {
        protected override IEnumerator DialogueContents()
        {
            ReDrawDialogue("Test Text");
            SetButton(0, "Yes", TestFeature).SetContinueWhenPressed(true);
            yield return Wait;
            ReDrawDialogue("Mewo mewo");
            SetButton(0, "Yes").SetContinueWhenPressed(true);
            SetButton(1, "+100 money", TestFeature);
            SetButton(2, "NOOO", SpawnBoss);
            yield return Wait;
            ReDrawDialogue("yooo");
            SetButton(2, "Close", ForceEndDialogue);
        }
        private void SpawnBoss()
        {
            DialogueEventBus.TriggerEvent(EventKeys.Skeletron);
            ForceEndDialogue();
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
            Debug.Log("100 moneys fortnite burger");
        }

    }
}

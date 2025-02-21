using Bremsengine;
using System.Collections;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class PatchouliBooks : Dialogue
    {
        protected override IEnumerator DialogueContents(int progress = 0)
        {
            DrawDialogue(@"You might encounter books on your ""Repair"" Missions.");
            ContinueButton(0);
            yield return new WaitForSeconds(0.15f);

            yield return Wait;
            DrawDialogue("You can use them to increase your Braincells Count");
            ContinueButton(0);
            yield return new WaitForSeconds(0.15f);

            yield return Wait;
            DrawDialogue("Have you already forgotten that you've discovered how to defy death with pure Braincell Power?");
            SetButton(0, "Yes...").SetContinueWhenPressed();
            yield return new WaitForSeconds(0.15f);

            yield return Wait;
            DrawDialogue("Hmmm...");
            ContinueButton(0);
            yield return new WaitForSeconds(0.15f);

            yield return Wait;
            ForceEndDialogue();
        }

        protected override void WhenEndDialogue(int dialogueEnding)
        {

        }

        protected override void WhenStartDialogue(int progress)
        {

        }
    }
}

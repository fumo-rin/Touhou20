using UnityEngine;
using Bremsengine;
using System.Collections;

namespace ChurroIceDungeon
{
    public class Daiyousei : Dialogue
    {
        protected override IEnumerator DialogueContents(int progress = 0)
        {
            DrawDialogue(@"My hero!");
            ContinueButton(0);
            yield return new WaitForSeconds(0.15f);

            yield return Wait;
            ForceEndDialogue();
        }
        private void CancelDialogue()
        {
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

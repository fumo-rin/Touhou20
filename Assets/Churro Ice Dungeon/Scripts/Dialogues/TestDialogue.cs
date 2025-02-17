using UnityEngine;
using Bremsengine;
using System.Collections;

namespace ChurroIceDungeon
{
    public class TestDialogue : Dialogue
    {
        protected override IEnumerator DialogueContents(int progress = 0)
        {
            DrawDialogue("Wow");
            SetButton(3, "Wow").SetContinueWhenPressed();
            ContinueButton(0);
            yield return Wait;
            ForceEndDialogue();
        }

        protected override void WhenEndDialogue(int dialogueEnding)
        {
            Debug.Log("Funny");
        }

        protected override void WhenStartDialogue(int progress)
        {

        }
    }
}

using Bremsengine;
using Core.Extensions;
using System.Collections;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class TewiBombs : Dialogue
    {
        protected override IEnumerator DialogueContents(int progress = 0)
        {
            DrawDialogue("ALL MINE!");
            ContinueButton(0);
            yield return new WaitForSeconds(0.15f);

            yield return Wait;
            DrawDialogue("AAAAAAAAAAAAAAAAAAAAAAAAAAAAA##AAAAAAAAAAAAAAAAAAAAAAAAAAAAA##AAAAAAAAAAAAAAAAAAAAAAAAAAAAA##AAAAAAAAAAAAAAAAAAAAAAAAAAAAA##AAAAAAAAAAAAAAAAA".ReplaceLineBreaks("##"));
            ContinueButton(0);
            yield return new WaitForSeconds(0.15f);

            yield return Wait;
            DrawDialogue("(She's gone insane...)");
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

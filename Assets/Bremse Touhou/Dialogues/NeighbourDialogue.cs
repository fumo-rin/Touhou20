using UnityEngine;
using Bremsengine;
using System.Collections;
using Core.Extensions;
using UnityEngine.Events;

namespace BremseTouhou
{
    public class NeighbourDialogue : Dialogue
    {
        [SerializeField] UnitPhase unitPhase;
        [SerializeField] Sprite PlayerSprite;
        [SerializeField] Sprite BearSprite;
        [SerializeField] UnityEvent OnDefeat;
        private void Start()
        {
            StartDialogue();
        }
        protected override IEnumerator DialogueContents(int progress = 0)
        {
            switch (progress)
            {
                case (> 0):

                    DialogueRunner.SetCharacterSprite(0, PlayerSprite);
                    DialogueRunner.SetCharacterSprite(1, BearSprite);
                    DialogueRunner.SetCharacterFocus(1);
                    DrawDialogue("You truly are strong...".ReplaceLineBreaks("##"));
                    ContinueButton(0);
                    yield return Wait;
                    yield return new WaitForSeconds(0.15f);
                    ForceEndDialogue(1);
                    break;
                default:
                    UnDrawDialogue();
                    yield return new WaitForSeconds(1f);
                    DialogueRunner.SetCharacterSprite(0, PlayerSprite);
                    DialogueRunner.SetCharacterSprite(1, BearSprite);
                    DialogueRunner.SetCharacterFocus(1);
                    DrawDialogue("(Neighbour)####You have bested my minions...##For this you must pay".ReplaceLineBreaks("##"));
                    ContinueButton(0);
                    yield return new WaitForSeconds(0.15f);

                    yield return Wait;
                    DrawDialogue("Your minions attacked me, i was tending to my Sauna!".ReplaceLineBreaks("##"));
                    DialogueRunner.SetCharacterFocus(0);
                    ContinueButton(0);
                    yield return new WaitForSeconds(0.15f);

                    yield return Wait;
                    DrawDialogue("I find that hard to believe.");
                    DialogueRunner.SetCharacterFocus(1);
                    ContinueButton(0);
                    yield return new WaitForSeconds(0.15f);

                    yield return Wait;
                    DrawDialogue("Either way you must be strong, but i wonder if you are as strong as me...");
                    DialogueRunner.SetCharacterFocus(1);
                    ContinueButton(0);
                    yield return new WaitForSeconds(0.15f);

                    yield return Wait;
                    ForceEndDialogue(0);
                    break;
            }
        }
        protected override void WhenEndDialogue(int dialogueEnding)
        {
            if (dialogueEnding == 0)
                unitPhase.StartFight();
            if (dialogueEnding == 1)
            {
                OnDefeat?.Invoke();
                TouhouManager.MoveToNextProgress();
            }
        }

        protected override void WhenStartDialogue(int progress)
        {

        }
    }
}

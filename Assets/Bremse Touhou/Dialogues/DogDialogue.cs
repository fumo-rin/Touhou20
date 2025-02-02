using UnityEngine;
using Bremsengine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using Core.Extensions;
namespace BremseTouhou
{
    public class DogDialogue : Dialogue
    {

        [SerializeField] UnitPhase unitPhase;
        [SerializeField] Sprite PlayerSprite;
        [SerializeField] Sprite dogSprite;
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
                    DialogueRunner.SetCharacterSprite(1, dogSprite);
                    DialogueRunner.SetCharacterFocus(1);
                    DrawDialogue("Woof!");
                    ContinueButton(0);
                    yield return Wait;
                    ForceEndDialogue(1);
                    break;
                default:
                    UnDrawDialogue();
                    yield return new WaitForSeconds(1f);
                    DialogueRunner.SetCharacterSprite(0, PlayerSprite);
                    DialogueRunner.SetCharacterSprite(1, dogSprite);
                    DialogueRunner.SetCharacterFocus(1);
                    DrawDialogue("(Neighbour's Dog)####Woof!".ReplaceLineBreaks("##"));
                    ContinueButton(0);
                    yield return new WaitForSeconds(0.15f);

                    yield return Wait;
                    DrawDialogue("A dog?".ReplaceLineBreaks("##"));
                    DialogueRunner.SetCharacterFocus(0);
                    ContinueButton(0);
                    yield return new WaitForSeconds(0.15f);

                    yield return Wait;
                    DrawDialogue("Woof!");
                    DialogueRunner.SetCharacterFocus(1);
                    ContinueButton(0);

                    yield return new WaitForSeconds(0.15f);
                    yield return Wait;
                    DrawDialogue("It's the neighbours dog!##That bastard is really out to get me!".ReplaceLineBreaks("##"));
                    DialogueRunner.SetCharacterFocus(0);
                    ContinueButton(0);
                    yield return new WaitForSeconds(0.15f);

                    yield return Wait;
                    DrawDialogue("Woof! (Menacing)");
                    DialogueRunner.SetCharacterFocus(1);
                    ContinueButton(0);
                    yield return new WaitForSeconds(0.15f);

                    yield return Wait;
                    DialogueRunner.SetCharacterFocus(0);
                    DrawDialogue("I must defeat you to stop this madness!");
                    ContinueButton(0);
                    yield return new WaitForSeconds(0.15f);

                    yield return Wait;
                    DrawDialogue("Woof!");
                    DialogueRunner.SetCharacterFocus(1);
                    ContinueButton(0);
                    yield return new WaitForSeconds(0.15f);

                    yield return Wait;
                    yield return new WaitForSeconds(0.15f);
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

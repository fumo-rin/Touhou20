using UnityEngine;
using Bremsengine;
using System.Collections;
using Core.Extensions;
using UnityEngine.Events;

namespace BremseTouhou
{
    public class MystiaDialogue : Dialogue
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
                    DrawDialogue("AAAHH!####You have bested me!##I can't believe what my eyes are hearing!".ReplaceLineBreaks("##"));
                    ContinueButton(0);
                    yield return new WaitForSeconds(0.15f);
                    yield return Wait;
                    ForceEndDialogue(1);
                    break;
                default:
                    UnDrawDialogue();
                    yield return new WaitForSeconds(1f);
                    DialogueRunner.SetCharacterSprite(0, PlayerSprite);
                    DialogueRunner.SetCharacterSprite(1, BearSprite);
                    DialogueRunner.SetCharacterFocus(1);
                    DrawDialogue("(Bear)####Yoooo".ReplaceLineBreaks("##"));
                    ContinueButton(0);
                    yield return new WaitForSeconds(0.15f);

                    yield return Wait;
                    DrawDialogue("(Sauna Maiden)####I am the Sauna Maiden, You have entered my Sauna and for that you must pay!".ReplaceLineBreaks("##"));
                    DialogueRunner.SetCharacterFocus(0);
                    ContinueButton(0);
                    yield return new WaitForSeconds(0.15f);

                    yield return Wait;
                    DrawDialogue("Its already over for you...");
                    DialogueRunner.SetCharacterFocus(1);
                    ContinueButton(0);
                    yield return new WaitForSeconds(0.15f);

                    yield return Wait;
                    DrawDialogue("Eat this!");
                    DialogueRunner.SetCharacterFocus(0);
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

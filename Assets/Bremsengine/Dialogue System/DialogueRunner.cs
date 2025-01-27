using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
namespace Bremsengine
{
    public class DialogueRunner : MonoBehaviour
    {
        static DialogueRunner Instance;
        [SerializeField] List<Dialogue.DialogueButton> dialogueButtons;
        [SerializeField] DialogueText dialogueText;
        [SerializeField] TMP_Text dialogueTextComponent;
        [SerializeField] GameObject dialogueContainer;
        public static List<Dialogue.DialogueButton> GetButtons() => Instance.dialogueButtons.ToList(); //lazy it should just copy this as a new list. it is to not affect the original (idk maybe doesnt matter).
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Dialogue.BindDialogueText(dialogueText);
            Dialogue.BindRunner(this);
            DialogueText.SetDialogueTextRenderer(dialogueTextComponent);
            dialogueContainer.SetActive(false);
        }
        public static void SetDialogueVisibility(bool state)
        {
            Instance.dialogueContainer.SetActive(state);
        }
        public void PressButton(int index)
        {
            Dialogue.PressButton(index);
        }
    }
}
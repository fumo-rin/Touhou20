using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
namespace Bremsengine
{
    public class DialogueRunner : MonoBehaviour
    {
        static DialogueRunner Instance;
        [SerializeField] List<DialogueCollection.DialogueButton> dialogueButtons;
        [SerializeField] DialogueText dialogueText;
        [SerializeField] TMP_Text dialogueTextComponent;
        [SerializeField] GameObject dialogueContainer;
        public static List<DialogueCollection.DialogueButton> GetButtons() => Instance.dialogueButtons.ToList(); //lazy it should just copy this as a new list. it is to not affect the original (idk maybe doesnt matter).
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DialogueCollection.BindDialogueText(dialogueText);
            DialogueCollection.BindRunner(this);
            DialogueText.SetDialogueTextRenderer(dialogueTextComponent);
        }
        public static void SetDialogueVisibility(bool state)
        {
            Instance.dialogueContainer.SetActive(state);
        }
        public void PressButton(int index)
        {
            DialogueCollection.PressButton(index);
        }
    }
}
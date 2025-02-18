using UnityEngine;

namespace ChurroIceDungeon
{
    public class TabSelector : MonoBehaviour
    {
        [SerializeField] GameObject inventoryContainer;
        [SerializeField] GameObject gearContainer;
        [SerializeField] GameObject questsContainer;
        [SerializeField] GameObject skillsContainer;
        [SerializeField] GameObject movesContainer;
        [SerializeField] GameObject spellsContainer;
        [SerializeField] GameObject tunesContainer;
        private void Start()
        {
            SetSelection(currentSelection);
        }
        public enum Selection
        {
            Inventory = 1,
            Gear = 2,
            Quests = 3,
            Skills = 4,
            Moves = 5,
            Spells = 6,
            Tunes = 7
        }
        public Selection currentSelection = Selection.Inventory;

        public void SelectInt(int selection)
        {
            SetSelection((Selection)selection);
        }
        public void SetSelection(Selection newSelection)
        {
            currentSelection = newSelection;
            if (inventoryContainer) inventoryContainer.SetActive(false);
            if (gearContainer) gearContainer.SetActive(false);
            if (questsContainer) questsContainer.SetActive(false);
            if (skillsContainer) skillsContainer.SetActive(false);
            if (movesContainer) movesContainer.SetActive(false);
            if (spellsContainer) spellsContainer.SetActive(false);
            if (tunesContainer) tunesContainer.SetActive(false);
            switch (currentSelection)
            {
                case Selection.Inventory:
                    if (inventoryContainer) inventoryContainer.SetActive(true);
                    break;
                case Selection.Gear:
                    if (gearContainer) gearContainer.SetActive(true);
                    break;
                case Selection.Quests:
                    if (questsContainer) questsContainer.SetActive(true);
                    break;
                case Selection.Skills:
                    if (skillsContainer) skillsContainer.SetActive(true);
                    break;
                case Selection.Moves:
                    if (movesContainer) movesContainer.SetActive(true);
                    break;
                case Selection.Spells:
                    if (spellsContainer) spellsContainer.SetActive(true);
                    break;
                case Selection.Tunes:
                    if (tunesContainer) tunesContainer.SetActive(true);
                    break;
                default:
                    break;
            }
        }
    }
}

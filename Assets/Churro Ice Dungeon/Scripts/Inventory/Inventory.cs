using System.Collections.Generic;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class Inventory : MonoBehaviour
    {
        [field: SerializeField] public Canvas canvas { get; private set; }
        public Canvas DraggingCanvas;
        public InventoryItem EmptyItem;
        [SerializeField] int slots = 24;
        [SerializeField] ItemSlot slotPrefab;
        [SerializeField] Transform slotContainer;
        static HashSet<ItemSlot> standardSlotsCache;
        private void Awake()
        {
            Rebuild();
        }
        [QFSW.QC.Command("-give-itemid")]
        public void GiveItem(string guid, float power)
        {
            if (ItemData.TryGetItemByID(guid, out var item) && item != null)
            {
                foreach (var slot in standardSlotsCache)
                {
                    if (slot.IsEmpty())
                    {
                        InventoryItem newItem = new();
                        newItem.containedData = item;
                        newItem.Power = power;
                        slot.SetItem(newItem);
                        break;
                    }
                }
            }
        }
        [QFSW.QC.Command("-give-itemindex")]
        public void GiveItem(int index, float power)
        {
            if (ItemData.TryGetItemByIndex(index, out var item) && item != null)
            {
                foreach (var slot in standardSlotsCache)
                {
                    if (slot.IsEmpty())
                    {
                        InventoryItem newItem = new();
                        newItem.containedData = item;
                        newItem.Power = power;
                        slot.SetItem(newItem);
                        break;
                    }
                }
            }
        }
        private void Rebuild()
        {
            void ClearSlotContainer()
            {
                for (int i = 0; i < slotContainer.childCount; i++)
                {
                    {
                        Destroy(slotContainer.GetChild(i).gameObject);
                    }
                }
            }
            ClearSlotContainer();
            standardSlotsCache = new HashSet<ItemSlot>();
            for (int i = 0; i < slots; i++)
            {
                ItemSlot s = Instantiate(slotPrefab, slotContainer);
                s.Bind(this);
                s.SlotID = i;
                standardSlotsCache.Add(s);
            }
        }
    }
}
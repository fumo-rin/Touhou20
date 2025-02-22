using System.Collections.Generic;
using UnityEngine;

namespace ChurroIceDungeon
{
    [DefaultExecutionOrder(-10)]
    public class Inventory : MonoBehaviour
    {
        public struct itemSlotSnapshot
        {
            public int slotID;
            public InventoryItem item;
        }
        public bool ClearInventory(ref Dictionary<int, itemSlotSnapshot> snapshot)
        {
            snapshot = new();
            foreach (var item in standardSlotsCache)
            {
                snapshot.Add(item.SlotID, new()
                {
                    slotID = item.SlotID,
                    item = EmptyItem
                });
            }
            ReloadLastSnapshot();
            return snapshot != null && snapshot.Count > 0;
        }
        public bool SnapshotCurrent(out Dictionary<int, itemSlotSnapshot> snapshot)
        {
            snapshot = new();
            foreach (var item in standardSlotsCache)
            {
                snapshot.Add(item.SlotID, new itemSlotSnapshot()
                {
                    slotID = item.SlotID,
                    item = item.GetContainedItem()
                });
            }
            return snapshot != null && snapshot.Count > 0;
        }
        public void ReloadLastSnapshot()
        {
            foreach (var item in standardSlotsCache)
            {
                if (slotsIDCache.TryGetValue(item.SlotID, out ItemSlot slot))
                {
                    slot.SetSlotFromSnapshot();
                }
            }
        }
        public bool GetItemFromSnapshot(int id, out itemSlotSnapshot snapshot)
        {
            return ChurroUnit.TryGetFromSnapshot(id, out snapshot);
        }
        [field: SerializeField] public Canvas canvas { get; private set; }
        public Canvas DraggingCanvas;
        public InventoryItem EmptyItem;
        [SerializeField] int slots = 24;
        [SerializeField] ItemSlot slotPrefab;
        [SerializeField] Transform slotContainer;
        static HashSet<ItemSlot> standardSlotsCache;
        static Dictionary<int, ItemSlot> slotsIDCache;
        private void Awake()
        {
            Rebuild();
            ChurroUnit.SetInventory(this);
        }
        public bool SetItemInFirstEmpty(InventoryItem item)
        {
            if (TryFindFirstEmpty(out ItemSlot slot))
            {
                slot.SetItem(item);
                return true;
            }
            return false;
        }
        private bool SetSlotItem(int id, InventoryItem item, out ItemSlot slot)
        {
            if (slotsIDCache.TryGetValue(id, out slot))
            {
                slot.SetItem(item);
                return true;
            }
            Debug.LogWarning("Failed to find Slot : " + id);
            return false;
        }
        private bool TryFindFirstEmpty(out ItemSlot slot)
        {
            slot = null;
            foreach (var item in standardSlotsCache)
            {
                if (item.IsEmpty())
                {
                    slot = item;
                    return true;
                }
            }
            return false;
        }
        [QFSW.QC.Command("-give-itemid")]
        public void TEST_GiveItem(string guid, float power)
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
        public void TEST_GiveItem(int index, float power)
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
            slotsIDCache = new();
            for (int i = 0; i < slots; i++)
            {
                ItemSlot s = Instantiate(slotPrefab, slotContainer);
                s.Bind(this);
                s.SlotID = i;
                slotsIDCache[i] = s;
                standardSlotsCache.Add(s);
            }
        }
    }
}
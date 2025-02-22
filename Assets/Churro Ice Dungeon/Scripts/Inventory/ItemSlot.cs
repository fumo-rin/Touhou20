using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChurroIceDungeon
{
    #region Slot Type
    public partial class ItemSlot
    {
        public enum SlotType
        {
            Item,
            Weapon,
            Helm,
            Wings
        }
    }
    #endregion
    #region Internal Functionality
    public partial class ItemSlot
    {
        public bool IsSlotOfType(SlotType type)
        {
            return type == slotType;
        }
        public bool IsEmpty()
        {
            bool empty = containedItem.containedData == null || containedItem.containedData.ID == inventory.EmptyItem.containedData.ID;
            return empty;
        }
        public void ClearItem()
        {
            SetItem(inventory.EmptyItem);
        }
        public InventoryItem GetContainedItem()
        {
            return containedItem;
        }
        public void SetItem(InventoryItem item)
        {
            InventoryItem newItem = (InventoryItem)item.Clone();
            containedItem = newItem;
            SetItemSprite(newItem.containedData.itemSprite);
        }
        private void SetItemSprite(Sprite sprite)
        {
            if (sprite == null)
            {
                itemImage.enabled = false;
                return;
            }
            itemImage.enabled = true;
            itemImage.sprite = sprite;
        }
    }
    #endregion
    #region Static Drag
    public partial class ItemSlot
    {
        public static class ActiveDrag
        {
            public static float LastDragStartTime;
            public static bool IsDragging => startSlot != null;
            public static ItemSlot startSlot { get; private set; } = null;
            static ItemSlot endSlot = null;
            public static ItemSlot lastHovered;
            public static void CancelDrag()
            {
                if (startSlot != null)
                {
                    //startSlot.SetQualityGraphicsVisibilty(startSlot.containedItem, true);

                    startSlot.dragItemRect.SetParent(startSlot.dragItemRectParent);
                    startSlot.dragItemRect.anchoredPosition = new(0f, 0f);
                }

                startSlot = null;
                endSlot = null;
            }
            public static void StartDrag(ItemSlot startSlot, Vector2 cursorPosition)
            {
                //Reset before doing stuff
                ActiveDrag.startSlot = null;
                endSlot = null;
                LastDragStartTime = Time.time;

                ActiveDrag.startSlot = startSlot;

                ActiveDrag.startSlot.dragItemRect.SetParent(ActiveDrag.startSlot.inventory.DraggingCanvas.transform);
                ActiveDrag.startSlot.dragItemRect.position = cursorPosition;
            }
            public static void PerformDrag(PointerEventData eventData, float scaleFactor)
            {
                if (startSlot == null)
                    return;
                startSlot.dragItemRect.anchoredPosition += eventData.delta / scaleFactor;
            }
            public static void EndDrag(ItemSlot endDragSlotPacket)
            {
                endSlot = endDragSlotPacket;

                if (startSlot != null && endSlot != null)
                {
                    SwapSlotContents(startSlot, endSlot);
                }

                CancelDrag();
            }

            private static void SwapSlotContents(ItemSlot startSlot, ItemSlot endSlot)
            {
                if (startSlot.IsEmpty())
                {
                    return;
                }
                Debug.Log("Start Slot Empty : " + startSlot.IsEmpty());

                InventoryItem firstSlotPacket = startSlot.containedItem;
                InventoryItem secondSlotPacket = endSlot.containedItem;

                if (endSlot.IsSlotOfType(SlotType.Item))
                {
                    if (!endSlot.IsEmpty() &&
                        !endSlot.containedItem.containedData.IsOfType(startSlot.slotType) &&
                        !startSlot.IsSlotOfType(SlotType.Item))
                    {
                        return;
                    }
                }
                if (!endSlot.IsSlotOfType(SlotType.Item))
                {
                    if (!startSlot.containedItem.containedData.IsOfType(endSlot.slotType))
                    {
                        return;
                    }
                }

                startSlot.ClearItem();
                endSlot.ClearItem();

                startSlot.SetItem(secondSlotPacket);
                endSlot.SetItem(firstSlotPacket);
            }
        }
    }
    #endregion
    public partial class ItemSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler, IPointerClickHandler
    {
        public static bool IsDragging => ActiveDrag.startSlot != null;
        [field: SerializeField] public int SlotID;
        [SerializeField] RectTransform dragItemRect;
        [SerializeField] Image itemImage;
        Inventory inventory;
        InventoryItem containedItem;
        float ScaleFactor => inventory.canvas.scaleFactor;
        public SlotType slotType;
        Transform dragItemRectParent;
        private void Start()
        {
            dragItemRectParent = dragItemRect.parent;
            containedItem = inventory.EmptyItem;
            SetSlotFromSnapshot();
        }
        public void SetSlotFromSnapshot()
        {
            if (inventory.GetItemFromSnapshot(SlotID, out Inventory.itemSlotSnapshot snapshot))
            {
                SetItem(snapshot.item);
            }
            else
            {
                SetItem(containedItem);
            }
        }
        public void Bind(Inventory toBind)
        {
            this.inventory = toBind;
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            ActiveDrag.StartDrag(this, (Vector2)eventData.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            ActiveDrag.PerformDrag(eventData, ScaleFactor);
        }

        public void OnDrop(PointerEventData eventData)
        {

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
        }
        private void DropContainedItem()
        {
            if (IsEmpty())
            {
                return;
            }
            //ClearItem();
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            /*if (eventData.button == PointerEventData.InputButton.Left)
            {
                DropContainedItem();
            }*/
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsDragging && ChurroUnit.Player != null && ChurroUnit.Player.IsAlive())
            {
                if (containedItem.containedData.UseItem(out ItemData.UseResult useResult))
                {
                    if (useResult == ItemData.UseResult.UseLimited)
                    {
                        ClearItem();
                    }
                }
            }
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            if (ActiveDrag.IsDragging)
            {
                #region Conditions
                bool dropDragObjectCondition = eventData.pointerEnter != null
                && eventData.pointerEnter.name == "Drop Item";
                #endregion
                if (dropDragObjectCondition)
                {
                    ActiveDrag.EndDrag(null);
                    DropContainedItem();

                    return;
                }
            }
            if (ActiveDrag.IsDragging && eventData.pointerEnter != null && eventData.pointerEnter.GetComponent<ItemSlot>() is ItemSlot slot)
            {
                if (slot == this)
                {
                    ActiveDrag.EndDrag(null);

                    return;
                }
                ActiveDrag.EndDrag(slot);
                return;
            }

            if (ActiveDrag.IsDragging)
            {
                ActiveDrag.EndDrag(null);
            }
        }
    }
}

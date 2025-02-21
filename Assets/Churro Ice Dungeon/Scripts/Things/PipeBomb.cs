using UnityEngine;

namespace ChurroIceDungeon
{
    public class PipeBomb : GroundItem
    {
        [SerializeField] InventoryItem containedItem;
        protected override bool OnTouch(Collider2D other, object playerData)
        {
            if (ChurroUnit.TryGetInventory(out Inventory i))
            {
                if (i.SetItemInFirstEmpty(containedItem))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChurroIceDungeon
{
    [System.Serializable]
    public partial struct InventoryItem : ICloneable
    {
        public ItemData containedData;
        public float Power;
        public object Clone()
        {
            InventoryItem c = new();
            c.containedData = this.containedData;
            c.Power = this.Power;
            return c;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bremsengine
{
    public enum BremseFaction
    {
        None,
        Player,
        Enemy
    }
    public interface IFaction
    {
        public BremseFaction Faction { get; protected set; }
        public bool IsOfFaction(BremseFaction f)
        {
            if (f == BremseFaction.None)
            {
                return false;
            }
            if (f == Faction)
            {
                return true;
            }
            return false;
        }
        public bool IsFriendsWith(BremseFaction f)
        {
            return IsOfFaction(f);
        }
        public void SetFaction(BremseFaction f) => Faction = f;
    }
}

using UnityEngine;

namespace ChurroIceDungeon
{
    public class Book : GroundItem
    {
        protected override bool OnTouch(Collider2D other, object playerData)
        {
            if (other != null && (ChurroUnit)playerData is ChurroUnit player and not null)
            {
                ChurroManager.ChangeBraincells(1);
                return true;
            }
            return false;
        }
    }
}

using UnityEngine;

namespace ChurroIceDungeon
{
    public class Book : GroundItem
    {
        [Range(1, 50)]
        [SerializeField] int lives = 1;
        protected override bool OnTouch(Collider2D other, object playerData)
        {
            if (other != null && (ChurroUnit)playerData is ChurroUnit player and not null)
            {
                ChurroManager.ChangeBraincells(lives);
                return true;
            }
            return false;
        }
    }
}

using UnityEngine;

namespace ChurroIceDungeon
{
    public class UnitScanner : MonoBehaviour
    {
        [SerializeField] EnemyUnit owner;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (owner.HasTarget)
                return;
            if (collision.GetComponent<DungeonUnit>() is DungeonUnit unit && !unit.FactionInterface.IsFriendsWith(owner.Faction))
            {
                owner.SetKnownTarget(unit);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!owner.HasTarget)
                return;
            if (collision.GetComponent<DungeonUnit>() is DungeonUnit unit && !owner.FactionInterface.IsFriendsWith(unit.Faction))
            {
                owner.ForgetTarget();
            }
        }
    }
}

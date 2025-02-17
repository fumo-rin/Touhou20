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
                Debug.Log("Owner :" + owner.FactionInterface.Faction.ToString());
                Debug.Log("Other : " + unit.Faction.ToString());
                owner.SetKnownTarget(unit);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!owner.HasTarget)
                return;
            Debug.Log(owner.FactionInterface.Faction.ToString());
            if (collision.GetComponent<DungeonUnit>() is DungeonUnit unit && !owner.FactionInterface.IsFriendsWith(unit.Faction))
            {
                owner.ForgetTarget();
            }
        }
    }
}

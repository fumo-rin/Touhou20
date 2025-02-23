using UnityEngine;

namespace ChurroIceDungeon
{
    [CreateAssetMenu(fileName = "ShottypeItem", menuName = "Churro/Items/Shottype Item")]
    public partial class ShottypeItem : ItemData
    {
        [field: SerializeField] public ChurroBaseAttack containedAttackPrefab { get; private set; }
        protected override void OnUse()
        {
            base.OnUse();
        }
    }
}

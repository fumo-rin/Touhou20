using Core.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace ChurroIceDungeon
{
    #region Target Resolver
    public partial class ChurroAttackHandler
    {
        public override bool ResolveTarget(out DungeonUnit target)
        {
            target = null;
            if (EnemyUnit.AutoAimTowardEnemy(Owner.CurrentPosition, 45f, out EnemyUnit enemy))
            {
                target = enemy;
            }
            return target != null;
        }
    }
    #endregion
    public partial class ChurroAttackHandler : AttackHandler
    {
        [SerializeField] Transform shotTypeAnchor;
        List<GameObject> spawnedShots = new List<GameObject>();
        int iteration = 0;
        private void SetShottype(ItemData item)
        {
            Debug.Log("T");
            if (item is ShottypeItem shot)
            {
                SpawnItem(shot.containedAttackPrefab);
            }
        }
        private void SpawnItem(ChurroBaseAttack attack)
        {
            DestroySpawnedItems();
            OnAttack = null;
            ChurroBaseAttack spawn = Instantiate(attack, shotTypeAnchor);
            spawnedShots.Add(spawn.gameObject);
            spawn.gameObject.SetActive(true);
            spawn.SetOwner(Owner);
            spawn.SetHandler(this);
        }
        public void RegisterWeapon(ChurroBaseAttack attack)
        {
            spawnedShots.AddIfDoesntExist(attack.gameObject);
        }
        private void DestroySpawnedItems()
        {
            for (int i = 0; i < spawnedShots.Count; i++)
            {
                if (spawnedShots[i] == null)
                {
                    spawnedShots.RemoveAt(i);
                    i--;
                }
                Destroy(spawnedShots[i].gameObject);
                spawnedShots.RemoveAt(i);
                i--;
            }
        }
        protected override void WhenStart()
        {
            ItemData.BindEvent(SetShottype, ItemActionKey.Shottype);
        }
        protected override void WhenDestroy()
        {
            ItemData.ReleaseEvent(SetShottype, ItemActionKey.Shottype);
        }
    }
}

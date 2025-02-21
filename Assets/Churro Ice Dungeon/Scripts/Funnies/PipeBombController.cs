using Bremsengine;
using Core.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class PipeBombController : ItemEventListener
    {
        [SerializeField] AudioClipWrapper pipeBomb;
        [SerializeField] bool GM_Explosion = true;
        [SerializeField] DungeonUnit.CollectionSettings unitBomber;
        protected override void Payload(DungeonUnit owner)
        {
            base.Payload(owner);
            TriggerPipeBomb(owner);
        }
        public void TriggerPipeBomb(DungeonUnit owner)
        {
            unitBomber.position = owner.CurrentPosition;
            GeneralManager.FunnyExplosion(transform.position, 4f);
            if (DungeonUnit.CollectCircle(unitBomber, out HashSet<DungeonUnit> output, out HashSet<DestructionItem> otherItems))
            {
                HashSet<IDamageable> damaged = new();
                foreach (var item in output)
                {
                    if (item.TryGetComponent(out IDamageable d))
                    {
                        damaged.Add(item);
                        d.Hurt(1000f, item.CurrentPosition);
                    }
                }
                foreach (var item in otherItems)
                {
                    if (item.GetComponent<IDamageable>() is IDamageable d)
                    {
                        if (damaged.Contains(d))
                        {
                            continue;
                        }
                        damaged.Add(d);
                    }
                    item.DestroyItem();
                }
            }
            /*
            RaycastHit2D[] hit = Physics2D.CircleCastAll(destructableBomber.position, destructableBomber.radius, Vector2.zero, 0f, destructableBomber.mask);
            foreach (var item in hit)
            {
                if (item.transform == null || (item.collider.isTrigger && destructableBomber.skipTriggers))
                    continue;

                if (item.transform.GetComponent<DestructionItem>() is DestructionItem d and not null && !d.transform.GetComponent<DungeonUnit>())
                {
                    d.
                }
            }*/
        }
    }
}

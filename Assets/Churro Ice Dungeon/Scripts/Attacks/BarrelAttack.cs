using Core.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class BarrelAttack : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile prefab;
        [SerializeField] ChurroProjectile bigPrefab;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            Debug.Log("Barrel Attack");
            ChurroProjectile.ArcSettings bigArc = new(0f, 360f, 72f, 2.5f);
            ChurroProjectile.ArcSettings smallArc = new(0f, 360f, 30f, 2.5f);

            foreach (var item in ChurroProjectile.SpawnArc(bigPrefab, input, bigArc))
            {
                item.Action_AddPosition(item.CurrentVelocity.ScaleToMagnitude(1.25f));
            }
            ChurroProjectile.SpawnArc(prefab, input, smallArc);
        }
    }
}

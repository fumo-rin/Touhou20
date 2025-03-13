using Bremsengine;
using Core.Extensions;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class MushiFairyAttack : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile prefab;
        [SerializeField] ChurroProjectile pelletPrefab;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            if (GeneralManager.CurrentDifficulty == GeneralManager.Difficulty.Ultra)
            {
                ChurroProjectile.SingleSettings pellet = new(0f, 7.5f);
                for (int i = 0; i < 5; i++)
                {
                    if (ChurroProjectile.SpawnSingle(pelletPrefab, input, pellet, out ChurroProjectile spawned))
                    {
                        spawned.Action_AddRotation(15f.Spread(100f));
                        spawned.Action_MultiplyVelocity(1f.Spread(5f));
                        spawned.Action_SetSpriteLayerIndex(-50);
                        spawned.Action_AddPosition(spawned.CurrentVelocity.ScaleToMagnitude(0.35f));
                        ChurroEventAccelerate accelerate = new(new(2f, 0.05f), 2.5f, 11f);
                        spawned.AddEvent(accelerate);
                    }
                }
            }
            Arc(-60f, 60f, 30f, 5.5f * 3f).Spawn(input, prefab, out iterationList);
            foreach (var iteration in iterationList)
            {
                ChurroEventAccelerate accelerate = new(new(1f, 0.05f), 5.5f, 30f);
                iteration.Action_AddPosition(iteration.CurrentVelocity.ScaleToMagnitude(0.75f));
                iteration.AddEvent(accelerate);
            }
        }
    }
}

using Bremsengine;
using Core.Extensions;
using NUnit.Framework;
using System.Collections;
using UnityEngine;

namespace BremseTouhou
{
    public class PointsOutcome : WheelOutcome
    {
        Coroutine co;
        public override void ApplyEffect(BaseUnit unit)
        {
            co = StartCoroutine(CO_Spawn(500));
        }

        public override void GameReset(BaseUnit unit)
        {
            if (co != null)
            {
                StopCoroutine(co);
            }
        }
        private IEnumerator CO_Spawn(int count  )
        {
            int attempts = 500;
            while (count >= 0 && attempts > 0)
            {
                attempts = attempts -1;
                Bounds b = DirectionSolver.TopOfScreenBounds(3f, 2f);
                Vector2 position;
                for (int i = 0; i < 5; i++)
                {
                    position = b.RandomWithin(b.center);
                    PlayerScoring.SpawnPickup(position);
                    count--;
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
        public override float GetDuration()
        {
            return 10f;
        }

        public override void RemoveEffect(BaseUnit unit)
        {

        }
    }
}

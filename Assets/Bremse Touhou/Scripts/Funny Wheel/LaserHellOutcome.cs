using Bremsengine;
using Core.Extensions;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    public class LaserHellOutcome : WheelOutcome
    {
        [SerializeField] LaserHell laserPrefab;
        List<LaserHell> lasers = new();
        [SerializeField] int laserCount = 15;

        public override void ApplyEffect(BaseUnit unit)
        {
            for (int i = 0; i < laserCount; i++)
            {
                LaserHell prefab = Instantiate(laserPrefab);
                Vector2 center = (Vector2)DirectionSolver.GetPaddedBounds(0f).center + Random.insideUnitCircle.ScaleToMagnitude(Random.Range(0f,3f));
                laserPrefab.Build(center);
                lasers.Add(prefab);
            }
        }
        private void ClearAllLasers()
        {
            foreach (LaserHell l in lasers)
            {
                l.Clear();
            }
            lasers.Clear();
        }
        public override void GameReset(BaseUnit unit)
        {
            ClearAllLasers();
        }

        public override float GetDuration()
        {
            return 6f;
        }

        public override void RemoveEffect(BaseUnit unit)
        {
            ClearAllLasers();
        }
    }
}
using Core.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace ChurroIceDungeon
{
    [System.Serializable]
    public class StagePath
    {
        public StagePath(Vector2 origin, Vector2[] relativeDestinations)
        {
            vectorPath = new();
            Vector2 target = origin + relativeDestinations[0];
            AddToPath(origin, target);
            for (int i = 1; i < relativeDestinations.Length; i++)
            {
                AddToPath(origin + relativeDestinations[i - 1], origin + relativeDestinations[i]);
            }
        }
        static List<Vector2> lineIteration;
        public List<Vector2> vectorPath { get; private set; }
        public Vector2 EndPoint => vectorPath[vectorPath.Count - 1];
        public Vector2 CurrentPoint => vectorPath[0];
        public bool IsPathValid => vectorPath.Count > 0;
        public void AddToPath(Vector2 origin, Vector2 target)
        {
            if (LineFromTo(origin, target, out lineIteration, 5f))
            {
                foreach (var line in lineIteration)
                {
                    vectorPath.Add(line);
                }
            }
        }
        public void DrawPath()
        {
            for (int i = 0; i < vectorPath.Count; i++)
            {
                Debug.DrawLine(vectorPath[i], vectorPath[(i + 1).Clamp(0, vectorPath.Count - 1)], ColorHelper.DeepGreen, 0.01f);
            }
        }

        const int pathAttempts = 5;
        public bool PerformPath(DungeonUnit unit)
        {
            if (vectorPath == null || vectorPath.Count <= 2)
            {
                return false;
            }
            int currentAttempts = 0;
            while (currentAttempts < pathAttempts && IsPathValid && unit.CurrentPosition.SquareDistanceToLessThan(CurrentPoint, 0.5f))
            {
                currentAttempts++;
                vectorPath.RemoveAt(0);
            }
            Vector2 direction = vectorPath[0] - unit.CurrentPosition;
            unit.ExternalMove(direction, out DungeonMotor.MotorOutput moveResult);
            Debug.DrawLine(unit.CurrentPosition, unit.CurrentPosition + direction);
            return !moveResult.Failed;
        }
        private bool LineFromTo(Vector2 origin, Vector2 end, out List<Vector2> line, float randomStepAngle = 5f, float stepDistance = 0.25f)
        {
            line = new List<Vector2>();
            Vector2 iteration = origin;
            float distance = (end - origin).magnitude;
            for (float i = 0; i < distance; i += stepDistance)
            {
                iteration += (end - iteration).ScaleToMagnitude(stepDistance).Rotate2D(randomStepAngle.RandomPositiveNegativeRange());
                line.Add(iteration);
            }
            return line != null && line.Count > 0;
        }
    }
}

using Core.Extensions;
using UnityEngine;

namespace Bremsengine
{
    #region Heading Calculation & movement Weights
    public partial class DirectionSolver
    {
        public enum HeadingToTarget
        {
            Towards,
            Away
        }
        [System.Serializable]
        public struct MovementWeights
        {
            public MovementWeights(int towardsTargetWeight, int awayFromTargetWeight)
            {
                this.towardsTarget = towardsTargetWeight;
                this.awayFromTarget = awayFromTargetWeight;
            }
            [SerializeField] int towardsTarget;
            [SerializeField] int awayFromTarget;
            public HeadingToTarget Heading => CalculateHeading();
            private HeadingToTarget CalculateHeading()
            {
                int totalWeight = towardsTarget + awayFromTarget;
                int selectedWeight = 0.RandomBetween(0, totalWeight);
                if (selectedWeight <= towardsTarget)
                {
                    return HeadingToTarget.Towards;
                }
                else
                {
                    return HeadingToTarget.Away;
                }
            }
        }
    }
    #endregion
    public partial class DirectionSolver : MonoBehaviour
    {
        static Bounds worldBounds = new();
        static Transform knownTarget;
        public static void SetKnownTarget(Transform t)
        {
            knownTarget = t;
        }
        public static void SetBounds(Vector2 center, Vector2 size)
        {
            worldBounds = new(center, size);
        }
        public static Bounds TopOfScreenBounds(float maxDistanceToTop, float padding)
        {
            Bounds worldBounds = GetPaddedBounds(padding);
            Vector2 center = new(worldBounds.center.x, worldBounds.max.y - maxDistanceToTop.Multiply(0.5f));
            return new(center, new(worldBounds.size.x, maxDistanceToTop));
        }
        public static Bounds GetPaddedBounds(float padding)
        {
            return new(worldBounds.center, new Vector2(worldBounds.size.x - padding.Multiply(2f), worldBounds.size.y - padding.Multiply(2f)));
        }
        private static Vector2 VectorToCenter(Transform t)
        {
            Vector2 v = worldBounds.center - t.position;
            return v.normalized;
        }
        public static Vector2 BuildDashVector(Transform t, Dash d)
        {
            return BuildDashVector(t, d.weights, d.forceRange, d.maxVerticalDistanceToTop, d.edgePadding, d.verticality);
        }
        private static Vector2 BuildDashVector(Transform t, MovementWeights weights, Vector2 forceRange, float maxVerticalDistanceToTop, float edgePadding, float verticality)
        {
            HeadingToTarget heading = weights.Heading;
            Vector2 centerVector = VectorToCenter(t);
            Vector2 processedVector = new(centerVector.x, centerVector.y);
            processedVector.x = processedVector.x.Sign();
            processedVector.y = Random.Range(-verticality,verticality);
            if (!IsWithinBounds(t.position, edgePadding, maxVerticalDistanceToTop))
            {
                return processedVector.ScaleToMagnitude(forceRange.RandomBetweenXY());
            }
            if (knownTarget == null)
                return new(0f, 0f);

            processedVector.x = (knownTarget.position.x - t.position.x).Sign();
            switch (heading)
            {
                case HeadingToTarget.Towards:
                    break;
                case HeadingToTarget.Away:
                    processedVector.x *= -1f;
                    break;
                default:
                    break;
            }
            return processedVector.ScaleToMagnitude(forceRange.RandomBetweenXY());
        }
        public static bool IsWithinBounds(Vector2 v, float edgePadding, float maxDistanceToTop = -1000f)
        {
            Bounds iteration = TopOfScreenBounds(maxDistanceToTop == -1000f ? worldBounds.size.y : maxDistanceToTop, edgePadding);
            return iteration.Contains(v);
        }
    }
}
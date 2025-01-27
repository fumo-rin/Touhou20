using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using Core.Extensions;

namespace Bremsengine
{
#if UNITY_EDITOR
    using UnityEditor;
    [CustomEditor(typeof(SplinePathMover))]
    public class SplineScriptableObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Clear World Center Override"))
            {
                if (target is SplinePathMover mover)
                {
                    mover.ClearWorldCenterOverride();
                }
            }
            if (GUILayout.Button("Set World Center To Transform Position"))
            {
                if (target is SplinePathMover mover)
                {
                    mover.RecalculateWorldCenterToPosition();
                }
            }
        }
    }
#endif
    [DefaultExecutionOrder(5)]
    public class SplinePathMover : MonoBehaviour
    {
        float elapsedTime;
        Spline activePath;
        [SerializeField] SplineSO containedPath;
        Vector2 targetPosition;
        [SerializeField] Rigidbody2D rb;
        [SerializeField] SplinePathSettings pathSettings;
        bool hasMovedThisFrame;
        [ContextMenu("Recalculate World Center Override To Transform Position")]
        public void RecalculateWorldCenterToPosition()
        {
            pathSettings.UseTransformAsWorldCenter(transform.position, true);
        }
        [ContextMenu("Clear World Center Override")]
        public void ClearWorldCenterOverride()
        {
            pathSettings.ClearWorldCenterOverride();
        }
        private void Start()
        {
            activePath = containedPath;
            pathSettings.UseTransformAsWorldCenter(transform.position, false);
        }
        public void PerformCurrentPath()
        {
            if (activePath == null)
            {
                return;
            }
            hasMovedThisFrame = true;
            rb.RunAlongSpline(activePath, pathSettings);
        }
        private void Update()
        {
            hasMovedThisFrame = false;
        }
        private void LateUpdate()
        {
            if (!hasMovedThisFrame)
            {
                rb.VelocityTowards(Vector2.zero, 4f);
            }
        }
        const int Gizmos_DrawSamplesPerSegment = 50;
        private void OnDrawGizmosSelected()
        {
            Vector3 AddVector2ToFloat3(float3 f3, Vector2 v2)
            {
                return new Vector3(f3.x + v2.x, f3.y + v2.y, f3.z);
            }
            if (containedPath != null && containedPath.containedSpline!= null)
            {
                Gizmos.color = Color.blue;

                Spline spline = containedPath;
                if (spline == null)
                    return;

                // Iterate through the spline and draw lines between sampled points
                for (int i = 0; i < spline.Count - 1; i++)
                {
                    BezierKnot startKnot = spline[i];
                    BezierKnot endKnot = spline[i + 1];

                    // Draw sampled points along the segment
                    Vector3 previousPoint =  AddVector2ToFloat3(startKnot.Position, pathSettings.WorldCenter);
                    for (int j = 1; j <= Gizmos_DrawSamplesPerSegment; j++)
                    {
                        float t = j / (float)Gizmos_DrawSamplesPerSegment;
                        Vector3 nextPoint = AddVector2ToFloat3(SplineUtility.EvaluatePosition(spline, i + t), pathSettings.WorldCenter);
                        Gizmos.DrawLine(previousPoint, nextPoint);
                        previousPoint = nextPoint;
                    }
                }

                // If the spline is closed, connect the last knot to the first knot
                if (spline.Closed)
                {
                    BezierKnot lastKnot = spline[spline.Count - 1];
                    BezierKnot firstKnot = spline[0];
                    Vector3 previousPoint = AddVector2ToFloat3(lastKnot.Position, pathSettings.WorldCenter);
                    for (int j = 1; j <= Gizmos_DrawSamplesPerSegment; j++)
                    {
                        float t = j / (float)Gizmos_DrawSamplesPerSegment;
                        Vector3 nextPoint = AddVector2ToFloat3(SplineUtility.EvaluatePosition(spline, spline.Count - 1 + t), pathSettings.WorldCenter);
                        Gizmos.DrawLine(previousPoint, nextPoint);
                        previousPoint = nextPoint;
                    }
                }
            }
        }
    }
}

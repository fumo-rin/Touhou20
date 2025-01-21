using Core.Extensions;
using System;
using System.Collections;
using UnityEngine;

namespace Bremsengine
{
    [System.Serializable]
    public struct Dash : ICloneable
    {
        public DirectionSolver.MovementWeights weights;
        public Vector2 forceRange;
        public float maxVerticalDistanceToTop;
        public float edgePadding;
        public float dashDuration;
        public float actionLockoutTime;
        public float drag;
        public float verticality;
        public float dashStartTime { get; private set; }
        public void SetDashStartTime()
        {
            dashStartTime = Time.time;
        }
        public bool ShouldPerform(Rigidbody2D rb)
        {
            if (Time.time >= dashStartTime + dashDuration)
            {
                return false;
            }
            if (rb.linearVelocity.magnitude <= 0.5f)
            {
                return false;
            }
            return true;
        }
        public object Clone()
        {
            Dash clone = new()
            {
                weights = this.weights,
                forceRange = this.forceRange,
                maxVerticalDistanceToTop = this.maxVerticalDistanceToTop,
                edgePadding = this.edgePadding,
                dashDuration = this.dashDuration,
                actionLockoutTime = this.actionLockoutTime,
                drag = this.drag,
                verticality = this.verticality
            };
            return clone;
        }
    }
    public class BossDashAction : MonoBehaviour
    {
        public delegate void DashAction(Dash dash, float nextActionTime);
        public DashAction OnDash;
        float nextDashTime;
        [SerializeField] Rigidbody2D rb;
        public float CooldownRemaining => nextDashTime - Time.time;
        public bool CanDash => CooldownRemaining <= 0f;
        public void SetNextCooldown(float delay)
        {
            nextDashTime = delay + Time.time;
        }
        public bool TryDash(Dash dash)
        {
            if (!CanDash)
            {
                return false;
            }
            return ForceDash(dash);
        }
        public bool ForceDash(Dash newDash)
        {
            Vector2 dashVector = DirectionSolver.BuildDashVector(rb.transform, newDash);
            SetNextCooldown(newDash.dashDuration);
            newDash.SetDashStartTime();
            OnDash?.Invoke(newDash, newDash.actionLockoutTime);
            StartCoroutine(CO_Dash(newDash, dashVector));
            return true;
        }
        private IEnumerator CO_Dash(Dash d, Vector2 dashVector)
        {
            rb.linearVelocity = dashVector;
            Bounds bounds = DirectionSolver.GetPaddedBounds(d.edgePadding - 0.05f);
            while (d.ShouldPerform(rb))
            {
                dashVector = dashVector.LerpTowards(Vector2.zero, d.drag);
                if (dashVector.magnitude < 1f)
                {
                    dashVector.MoveTowards(Vector2.zero, 1.5f);
                }
                rb.linearVelocity = dashVector;
                Bounds clamp = DirectionSolver.TopOfScreenBounds(d.maxVerticalDistanceToTop, d.edgePadding);
                Vector2 clampedPosition = rb.position.ClampInside(clamp);
                rb.position = clampedPosition;
                yield return null;
            }
            rb.linearVelocity = new(0f, 0f);
        }
    }
}

using Core.Extensions;
using UnityEngine;

namespace ChurroIceDungeon
{
    #region attack
    public partial class Funnyscript
    {
        [SerializeField] ChurroBaseAttack attack;
        [SerializeField] float attackRepeatDelay = 0.3f;
        float nextAttackTime = 0.3f;
        float stopShootingTime;
        private void AttackLoop()
        {
            if (Time.time < nextAttackTime)
            {
                return;
            }
            if ((TrackedTarget == null || !TrackedTarget.IsAlive()))
            {
                if (Time.time < stopShootingTime)
                {
                    nextAttackTime = Time.time + attackRepeatDelay;
                    attack.ExternalForcedAttack(trackedPosition);
                }
                return;
            }
            stopShootingTime = Time.time + 3f;
            nextAttackTime = Time.time + attackRepeatDelay;
            attack.ExternalForcedAttack(trackedPosition);
        }
    }
    #endregion
    #region Tracking
    public partial class Funnyscript
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out ChurroUnit player))
            {
                SetTrackedTarget(player);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out ChurroUnit player) && TrackedTarget == player)
            {
                SetTrackedTarget(null);
            }
        }
    }
    #endregion
    public partial class Funnyscript : MonoBehaviour
    {
        Vector2 trackedPosition;
        public DungeonUnit TrackedTarget => hitboxTarget == null ? null : hitboxTarget;
        DungeonUnit hitboxTarget;
        [SerializeField] Transform rotationAnchor;
        [Range(0.1f, 5f)]
        [SerializeField] float distanceRotationSpeed = 0.65f;
        bool TryGetDirection(out Vector2 direction)
        {
            direction = trackedPosition - (Vector2)transform.position;
            if (TrackedTarget == null)
            {
                return false;
            }
            direction = TrackedTarget.CurrentPosition - (Vector2)transform.position;
            return true;
        }
        private void Start()
        {
            nextAttackTime = attackRepeatDelay.Multiply(0.5f).Spread(50f);
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.transform.TryGetComponent(out DungeonUnit unit))
            {
                unit.ExternalKill();
            }
            if (collision.transform.GetComponent<DungeonUnit>() is DungeonUnit unit2 and not null)
            {
                unit2.ExternalKill();
            }
        }
        public void SetTrackedTarget(DungeonUnit t) => hitboxTarget = t;
        private void Update()
        {
            float lerp = 1f;
            if (TryGetDirection(out Vector2 direction))
            {
                Debug.DrawLine(transform.position, (Vector2)transform.position + direction, ColorHelper.Peach, 0.05f);
                lerp = 0.25f + ((1f + direction.sqrMagnitude) * 0.01f * distanceRotationSpeed);
            }
            if (LerpTowardsTarget(trackedPosition, lerp, TrackedTarget, out Vector2 worldPosition))
            {
                rotationAnchor.Lookat2D(worldPosition);
                trackedPosition = worldPosition;
            }
            AttackLoop();
        }
        public bool LerpTowardsTarget(Vector2 vector, float speed, DungeonUnit target, out Vector2 v)
        {
            v = vector;

            if (target == null) return false;
            v = vector.LerpTowards(target.CurrentPosition, speed);
            return true;
        }
    }
}
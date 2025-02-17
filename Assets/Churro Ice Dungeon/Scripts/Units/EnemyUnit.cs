using Bremsengine;
using Core.Extensions;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class EnemyUnit : DungeonUnit
    {
        [SerializeField] float Power = 0f;
        public override float DamageScale(float damage)
        {
            return ((1f + Power / 100f).Max(1f)) * damage;
        }
        DungeonUnit pathingTarget;
        DungeonUnit KnownTarget;
        [SerializeField] LayerMask targetBlockingLayer;
        Vector2[] addedScanPoints = new Vector2[5]
        {
            new(-0.65f,-0.65f),
            new(-0.65f,0.65f),
            new(0.65f,0.65f),
            new(0.65f,-0.65f),
            new(0f,0f)
        };
        RaycastHit2D targettingHit;

        private bool FindTarget(Vector2 scanPosition, out DungeonUnit output)
        {
            output = null;
            foreach (Vector2 point in addedScanPoints)
            {
                targettingHit = Physics2D.Raycast(CurrentPosition + point, scanPosition - (CurrentPosition + point), 30f, targetBlockingLayer);
                if (targettingHit.transform != null && targettingHit.transform.GetComponent<DungeonUnit>() is DungeonUnit validUnit and not null)
                {
                    output = validUnit;
                    return true;
                }
            }
            return false;
        }
        protected override void WhenAwake()
        {
            FactionInterface.SetFaction(BremseFaction.Enemy);
        }

        protected override void WhenDestroy()
        {
            TickManager.MainTickLightweight -= Tick;
        }
        public void SetKnownTarget(DungeonUnit target)
        {
            KnownTarget = target;
        }
        public bool HasTarget => KnownTarget != null;
        public void ForgetTarget()
        {
            KnownTarget = null;
        }

        protected override void WhenStart()
        {
            TickManager.MainTickLightweight += Tick;
        }
        private void Tick()
        {
            if (gameObject.activeInHierarchy)
            {
                if (pathingTarget == null && KnownTarget != null && FindTarget(KnownTarget.CurrentPosition, out DungeonUnit foundTarget))
                {
                    Debug.Log("Found Target : " + foundTarget.transform.name);
                    pathingTarget = foundTarget;
                }
                if (pathingTarget != null)
                {
                    pather.StartPathing(pathingTarget.CurrentPosition);
                }
            }
        }
        private void Update()
        {
            if (pather.HasPath && pather.PerformPath(out Vector2 pathVector))
            {
                MoveMotor(pathVector, out DungeonMotor.MotorOutput result);
            }
        }
    }
}

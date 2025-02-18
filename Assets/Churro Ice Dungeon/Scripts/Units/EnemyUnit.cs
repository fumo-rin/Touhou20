using Bremsengine;
using Core.Extensions;
using System.Collections;
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
        float loseTargetTime;
        [SerializeField] float targetMemoryTime = 3f;
        [SerializeField] LayerMask targetBlockingLayer;
        [SerializeField] float targetVisionRange = 5f;
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
                targettingHit = Physics2D.Raycast(CurrentPosition + point, scanPosition - (CurrentPosition + point), targetVisionRange, targetBlockingLayer);
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
            StartPatrol(Origin, 5f);
        }
        private void Tick()
        {
            if (gameObject.activeInHierarchy)
            {
                bool hasFoundTarget = false;
                DungeonUnit scanTarget = null;
                if (KnownTarget != null)
                {
                    hasFoundTarget = FindTarget(KnownTarget.CurrentPosition, out scanTarget);
                }
                if (hasFoundTarget)
                {
                    loseTargetTime = Time.time + targetMemoryTime;
                    if (pathingTarget == null && KnownTarget != null)
                    {
                        Debug.Log("Found Target : " + scanTarget.transform.name);
                        pathingTarget = scanTarget;
                    }
                }
                if (pathingTarget != null && !pather.isAwaitingPath)
                {
                    SetDestination(pathingTarget.CurrentPosition);
                }
                if (pathingTarget != null && Time.time > loseTargetTime)
                {
                    pathingTarget = null;
                    Debug.Log("Test");
                    pather.ClearPath();
                    SetDestination(Origin);
                    QueuePatrol();
                    if (!CheckNavmeshPosition(Origin))
                    {
                        transform.position = Origin;
                    }
                }
            }
        }
        public void QueuePatrol()
        {
            Debug.Log("Queueing Patrol");
            pather.BindReachPathEndAction(QueuePatrolCoroutine);
        }
        private void QueuePatrolCoroutine()
        {
            Debug.Log("Running Patrol Queue");
            StartPatrol(Origin, 5f);
        }
        private void StartPatrol(Vector2 target, float patrolRange)
        {
            Vector2 RandomPatrol()
            {
                return target + Random.insideUnitCircle * patrolRange;
            }
            IEnumerator CO_Patrol(float delay)
            {
                yield return new WaitForSeconds(delay);
                if (pathingTarget == null)
                {
                    bool hitNavmesh = false;
                    Vector2 patrol = Origin;
                    for (int i = 0; i < 10; i++)
                    {
                        patrol = RandomPatrol();
                        if (base.NavmeshContains(patrol))
                        {
                            hitNavmesh = true;
                        }
                        if (hitNavmesh)
                            break;
                        else
                        {
                            patrol = Origin;
                        }
                    }
                    pather.StartPathing(patrol);
                    QueuePatrol();
                }
            }
            StartCoroutine(CO_Patrol(1.5f.Spread(30f)));
        }
        private void Update()
        {
            DungeonMotor.MotorOutput result = new();
            if (pather.HasPath && pather.PerformPath(out Vector2 pathVector))
            {
                MoveMotor(pathVector, out result);
                return;
            }
            MoveMotor(Vector2.zero, out result);
        }
    }
}

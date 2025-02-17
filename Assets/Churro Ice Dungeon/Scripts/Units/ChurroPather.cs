using Core.Extensions;
using Pathfinding;
using Pathfinding.RVO;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class ChurroPathBuilder
    {
        public ChurroPather pather;
        public void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                pather.SetPath(p);
            }
        }
        public void PathTo(Vector2 start, Vector2 target)
        {
            pather.seeker.StartPath(start, target, OnPathComplete);
        }
    }
    public static class RVOExtension
    {
        public static Vector2 SolveRVO(this RVOController rvo, Vector2 direction, float maxSpeed)
        {
            if (rvo != null)
            {
                Vector2 velocity = direction.normalized * maxSpeed;
                rvo.Move(velocity);
                direction = rvo.CalculateMovementDelta(rvo.transform.position, Time.deltaTime);
            }
            return direction.normalized;
        }
    }
    [System.Serializable]
    public class ChurroPather
    {
        ChurroPathBuilder pathBuilder;
        [field: SerializeField] public Seeker seeker { get; private set; }
        [SerializeField] float patherRadius = 0.75f;
        Vector2 position => owner.CurrentPosition;
        private Vector2 CurrentDirection => GetPathDirection(this.path, this.owner.CurrentPosition);
        public bool isAwaitingPath { get; set; }
        DungeonUnit owner;
        public Path path { get; private set; }
        int currentWaypoint;
        [field: SerializeField] public RVOController rvo { get; private set; }
        [SerializeField] float collapseDistance = 0.8f;
        public bool HasPath => path != null && currentWaypoint < path.vectorPath.Count - 2;
        public void StartPathing(Vector2 target)
        {
            pathBuilder.PathTo(seeker.transform.position, target);
        }
        public bool PerformPath(out Vector2 pathDirection)
        {
            pathDirection = Vector2.zero;
            if (!HasPath)
            {
                return false;
            }
            Collapse();
            if (CurrentDirection == Vector2.zero)
            {
                return false;
            }
            Vector2 rvoVector = rvo.SolveRVO(CurrentDirection, owner.CollapseMotor().MaxSpeed);
            pathDirection = rvoVector;
            return true;
        }
        public void ValidatePather(DungeonUnit owner)
        {
            this.owner = owner;
            if (pathBuilder == null)
            {
                pathBuilder = new();
                pathBuilder.pather = this;
            }

            if (rvo == null)
            {
                if (owner.TryGetComponent(out RVOController found))
                {
                    rvo = found;
                }
                if (rvo == null && owner.GetComponentInChildren<RVOController>() is RVOController foundChild)
                {
                    rvo = foundChild;
                }
            }
            if (rvo != null)
            {
                rvo.radius = patherRadius;
            }
        }
        public void Collapse()
        {
            while (position.SquareDistanceToLessThan(path.vectorPath[currentWaypoint], collapseDistance))
            {
                if (currentWaypoint >= path.vectorPath.Count - 1)
                {
                    ClearPath();
                    return;
                }
                currentWaypoint++;
            }
        }
        public void SetPath(Path p)
        {
            SetAwaitingPath(false);
            if (p == null)
            {
                if (path != null)
                {
                    Debug.Log("Clear Path");
                }
                path = null;
                currentWaypoint = 0;
                return;
            }
            if (!p.error)
            {
                path = p;
                currentWaypoint = 0;
                return;
            }
        }
        public Vector2 GetPathDirection(Path p, Vector2 position)
        {
            if (p == null || p.vectorPath.Count <= 1)
                return Vector2.zero;
            return ((Vector2)p.vectorPath[currentWaypoint.Clamp(0, p.vectorPath.Count - 1)] - position).normalized;
        }
        public void SetAwaitingPath(bool state)
        {
            isAwaitingPath = state;
        }
        public void ClearPath() => SetPath(null);
    }
}
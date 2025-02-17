using Bremsengine;
using Core.Extensions;
using Pathfinding;
using UnityEngine;

namespace ChurroIceDungeon
{
    #region Motor
    public partial class DungeonUnit
    {
        [SerializeField] DungeonMotor unitMotor;
        float nextMovetime;
        public virtual DungeonMotor CollapseMotor()
        {
            return unitMotor;
        }
        protected void MoveMotor(Vector2 input, out DungeonMotor.MotorOutput result)
        {
            bool FailCondition(out DungeonMotor m)
            {
                m = CollapseMotor();
                if (Time.time < nextMovetime)
                    return true;
                if (m == null)
                    return true;
                return false;
            }
            if (FailCondition(out DungeonMotor m))
            {
                result = new();
                result.Failed = true;
                return;
            }
            m.PerformMotor(this, input, out result);
            if (!result.Failed)
            {
                nextMovetime = result.NextMoveTime;
            }
        }
    }
    #endregion
    #region Faction
    public partial class DungeonUnit : IFaction
    {
        BremseFaction IFaction.Faction { get; set; }
        public BremseFaction Faction => ((IFaction)this).Faction;
        public IFaction FactionInterface => (IFaction)this;
    }
    #endregion
    #region Damage Scaling
    public abstract partial class DungeonUnit
    {
        public abstract float DamageScale(float damage);
    }
    #endregion
    #region Pathing
    public abstract partial class DungeonUnit
    {
        [SerializeField] protected ChurroPather pather;
        public void SetDestination(Vector2 target)
        {
            if (pather == null)
                return;
            pather.StartPathing(target);
        }
        public bool IsOnNavmesh(float scanSize = 2f)
        {
            return CheckNavmeshPosition(CurrentPosition);
        }
        public static bool CheckNavmeshPosition(Vector2 position, float scanSize = 2f)
        {
            if (((Vector2)(Vector3)AstarPath.active.GetNearest(position, NNConstraint.Walkable)).SquareDistanceToGreaterThan(position, scanSize))
            {
                return false;
            }
            return true;
        }
    }
    #endregion
    [SelectionBase]
    public abstract partial class DungeonUnit : MonoBehaviour
    {
        [SerializeField] Rigidbody2D assignedRB;
        public Rigidbody2D RB => assignedRB;
        public static DungeonUnit Player { get; private set; }
        public Vector2 CurrentPosition => transform.position;

        protected abstract void WhenAwake();
        protected abstract void WhenDestroy();
        protected abstract void WhenStart();
        private void Awake()
        {
            if (this is ChurroUnit c)
            {
                Player = c;
                FactionInterface.SetFaction(BremseFaction.Player);
            }
            pather.ValidatePather(this);
            WhenAwake();
        }
        private void Start()
        {
            WhenStart();
        }
        private void OnDestroy()
        {
            WhenDestroy();
        }
    }
}
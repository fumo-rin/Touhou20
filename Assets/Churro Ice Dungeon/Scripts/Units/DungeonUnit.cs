using Bremsengine;
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
using Bremsengine;
using Core.Extensions;
using Pathfinding;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ChurroIceDungeon
{
    #region Conveyor
    public partial class DungeonUnit : IConveyorable
    {
        public void Push(Conveyor c)
        {
            RB.VelocityTowards(c.DirectionWithForce, c.Settings.Acceleration);
        }
    }
    #endregion
    #region Damageable
    public partial class DungeonUnit : IDamageable
    {
        public abstract bool IsAlive();
        protected IDamageable Damageable => (IDamageable)this;
        float IDamageable.CurrentHealth { get; set; }
        public float Health => Damageable.CurrentHealth;
        void StartDamageable(float startingHealth)
        {
            Damageable.CurrentHealth = startingHealth;
        }
        void IDamageable.Hurt(float damage, Vector2 damagePosition)
        {
            OnHurt(damage, damagePosition);
        }
        protected abstract void OnHurt(float damage, Vector2 damagePosition);
    }
    #endregion
    #region Targetbox Integration
    public partial class DungeonUnit
    {
        [SerializeField] List<TargetBox> targetBoxes = new List<TargetBox>();
        private void StartTargetBoxes()
        {
            foreach (var item in targetBoxes)
            {
                item.OnTakeDamage += Damageable.Hurt;
            }
        }
        private void EndTargetBoxes()
        {
            foreach (var item in targetBoxes)
            {
                item.OnTakeDamage -= Damageable.Hurt;
            }
        }
    }
    #endregion
    #region Motor
    public partial class DungeonUnit
    {
        HashSet<MotorZone> knownModifiers = new();
        [SerializeField] DungeonMotor unitMotor;
        public HashSet<MotorZone> MotorZones => knownModifiers;
        public void BindMotorZone(MotorZone m)
        {
            knownModifiers.Add(m);
        }
        public void ReleaseMotorZone(MotorZone m)
        {
            knownModifiers.Remove(m);
        }
        float nextMovetime;
        public virtual DungeonMotor CollapseMotor()
        {
            return unitMotor;
        }
        float speedmod = 1f;
        public DungeonUnit Action_SpeedmodTowards(float speed, float step)
        {
            speedmod.MoveTowards(speed, step);
            return this;
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
                m.ApplyFailFriction(this);
                return;
            }
            DungeonMotor.Settings settings = new(this);
            settings.SpeedMod = speedmod;
            m.RunMotor(this, input, settings, out result, ref nextMovetime);
            if (!result.Failed)
            {
                SetNextMovetime(result.NextMoveTime);
            }
        }
        public void SetNextMovetime(float time)
        {
            nextMovetime = time;
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
            if (pather == null || pather.isAwaitingPath)
                return;

            testVector = target;
            pather.StartPathing(target);
        }
        [SerializeField] Vector2 testVector;
        public bool NavmeshContains(float scanSize = 2f)
        {
            return CheckNavmeshPosition(CurrentPosition, scanSize);
        }
        public bool NavmeshContains(Vector2 position, float scanSize = 2f)
        {
            return CheckNavmeshPosition(position, scanSize);
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
    #region Collect
    public partial class DungeonUnit
    {
        [System.Serializable]
        public struct CollectionSettings
        {
            public CollectionSettings(Vector2 position, float radius, LayerMask mask, bool skipTriggers, BremseFaction friends)
            {
                this.position = position;
                this.radius = radius;
                this.mask = mask;
                this.skipTriggers = skipTriggers;
                this.friends = friends;
            }
            [HideInInspector] public Vector2 position;
            public float radius;
            public LayerMask mask;
            public bool skipTriggers;
            public BremseFaction friends;
        }
        public static bool CollectCircle(CollectionSettings settings, out HashSet<DungeonUnit> units, out HashSet<DestructionItem> boxes)
        {
            units = new();
            boxes = new();
            RaycastHit2D[] hit = Physics2D.CircleCastAll(settings.position, settings.radius, Vector2.zero, 0f, settings.mask);
            foreach (var item in hit)
            {
                if (item.transform == null || (item.collider.isTrigger && settings.skipTriggers))
                    continue;
                if (item.transform.GetComponent<DungeonUnit>() is DungeonUnit unit and not null && !unit.FactionInterface.IsFriendsWith(settings.friends))
                {
                    units.Add(unit);
                }
                if (item.transform.GetComponent<DestructionItem>() is DestructionItem box and not null)
                {
                    if (box.Faction == BremseFaction.None || box.Faction != settings.friends)
                    {
                        boxes.Add(box);
                    }
                }
            }
            return (units != null && units.Count > 0) || (boxes != null && boxes.Count > 0);
        }
    }
    #endregion
    [SelectionBase]
    public abstract partial class DungeonUnit : MonoBehaviour
    {
        [SerializeField] AttackHandler optionalAttackHandler;
        [SerializeField] Rigidbody2D assignedRB;
        public Rigidbody2D RB => assignedRB;
        [SerializeField] Transform ai_AimTransformOverride;
        public Vector2 AimTarget => ai_AimTransformOverride == null ? CurrentPosition : ai_AimTransformOverride.position;
        public static DungeonUnit Player { get; private set; }
        public Vector2 CurrentPosition => transform.position;
        public Vector2 Origin { get; private set; }
        public void ExternalKill()
        {
            OnHurt(1000000000f, CurrentPosition);
        }
        protected abstract void WhenAwake();
        protected abstract void WhenDestroy();
        protected abstract void WhenStart();
        [SerializeField] float startingHealth = 100;
        private void Awake()
        {
            if (optionalAttackHandler != null) optionalAttackHandler.SetOwner(this);
            StartDamageable(startingHealth);
            Origin = transform.position;
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
            StartTargetBoxes();
            WhenStart();
        }
        private void OnDestroy()
        {
            EndTargetBoxes();
            WhenDestroy();
        }
        public Collider2D[] AllColliders => transform.root.GetComponentsInChildren<Collider2D>();
    }
}
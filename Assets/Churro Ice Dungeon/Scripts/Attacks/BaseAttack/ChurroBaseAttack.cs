using Bremsengine;
using Core.Extensions;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json.Linq;
using UnityEngine.UIElements;

namespace ChurroIceDungeon
{
    public abstract partial class ChurroBaseAttack : MonoBehaviour
    {
        protected Vector2 DownDirection => (owner.CurrentPosition + Vector2.down) - owner.CurrentPosition;
        [SerializeField] bool shouldOverrideSettings;
        [SerializeField] AttackHandler.AttackTimeSettings overrideSettings;
        protected ChurroProjectile.ArcSettings Arc(float startAngle, float endAngle, float angleInterval, float projectileSpeed) => new(startAngle, endAngle, angleInterval, projectileSpeed);
        protected ChurroProjectile.crawlerPacket CrawlerPacket(float delay, float aimAngle, float repeatAngle, int repeatCount, float repeatTimeInterval) => new(delay, aimAngle, repeatAngle, repeatCount, repeatTimeInterval);
        public bool TryGetOverrideSettings(out AttackHandler.AttackTimeSettings output)
        {
            output = null;
            if (shouldOverrideSettings && overrideSettings != null)
            {
                output = overrideSettings;
            }
            return shouldOverrideSettings && output != null;
        }
        protected bool Hardmode => ChurroManager.HardMode;
        [SerializeField] float baseDamage = 5f;
        [SerializeField] int projectileLayerIndex = 100;
        [Header("Optional")]
        [field: SerializeField] protected DungeonUnit owner { get; private set; }
        [SerializeField] protected AttackHandler handler;
        [SerializeField] protected AudioClipWrapper attackSound;
        public void TriggerAttackLoad()
        {
            if (handler != null && TryGetOverrideSettings(out AttackHandler.AttackTimeSettings settings))
            {
                handler.settings.ApplySettings(settings);
            }
            OnAttackLoad();
        }
        protected virtual void OnAttackLoad()
        {

        }
        private void PerformContainedAttack(Vector2 target)
        {
            if (gameObject.activeInHierarchy)
            {
                PerformContainedAttack(target, false);
            }
        }
        private void PerformContainedAttack(Vector2 target, bool bypassAliveCheck = false)
        {
            if (gameObject == null || (!gameObject.activeInHierarchy && !bypassAliveCheck))
            {
                return;
            }
            if (handler != null && !handler.settings.IsAttackTimeAllowed())
            {
                return;
            }
            if (handler != null)
            {
                if (!handler.settings.IsAttackTimeAllowed())
                {

                    return;
                }
                handler.settings.TriggerAttackTime();
            }
            ChurroProjectile.InputSettings s = new(owner == null ? transform.position : owner.CurrentPosition, target - (owner == null ? transform.position : (Vector2)owner.CurrentPosition));
            s.OnSpawn += ProjectileSpawnCallback;
            attackSound.Play(transform.position);
            AttackPayload(s);
        }
        public void ExternalForcedAttack()
        {
            PerformContainedAttack((Vector2)transform.position + Vector2.down, true);
        }
        protected abstract void AttackPayload(ChurroProjectile.InputSettings input);
        private void ProjectileSpawnCallback(ChurroProjectile p)
        {
            if (owner != null)
            {
                p.Action_SetFaction(owner.Faction);
                p.Action_SetDamage(owner.DamageScale(baseDamage));
                p.Action_SetOwnerReference(owner);
            }
            else
            {
                p.Action_SetFaction(BremseFaction.None);
                p.Action_SetDamage(baseDamage);
                p.Action_SetOwnerReference(null);
            }
            p.Action_SetSpriteLayerIndex(projectileLayerIndex);
        }
        public void SetHandler(AttackHandler handler)
        {
            this.handler = handler;
            handler.OnAttack = null;
            handler.OnAttack += PerformContainedAttack;
            TriggerAttackLoad();
        }
        public void ClearHandler()
        {
            if (handler != null)
            {
                handler.OnAttack -= PerformContainedAttack;
            }
            handler = null;
        }
        public void SetOwner(DungeonUnit unit)
        {
            owner = unit;
        }
        private void Start()
        {
            TriggerAttackLoad();
            WhenStart();
            if (handler != null) handler.OnAttack += PerformContainedAttack;
            if (handler is ChurroAttackHandler churro)
            {
                churro.RegisterWeapon(this);
            }
        }
        protected virtual void WhenStart()
        {

        }
        private void OnDestroy()
        {
            if (handler != null) handler.OnAttack -= PerformContainedAttack;
        }
    }
}

using Bremsengine;
using Core.Extensions;
using UnityEngine;

namespace ChurroIceDungeon
{
    #region Modifiers
    public partial class ChurroBaseAttack
    {
        [SerializeField] bool useDifficultyModifiers;
        public float Mod_DifficultyBulletDensity => useDifficultyModifiers ? 1f / GeneralManager.DifficultyMultipliers.GetMultiplier(GeneralManager.DifficultyMultipliers.Modifier.AttackDensity, GeneralManager.CurrentDifficulty) : 1f;
        public float Mod_DifficultyBulletSpeed => useDifficultyModifiers ? GeneralManager.DifficultyMultipliers.GetMultiplier(GeneralManager.DifficultyMultipliers.Modifier.Speed, GeneralManager.CurrentDifficulty) : 1f;
        public float Mod_DifficultyBulletDamage => useDifficultyModifiers ? GeneralManager.DifficultyMultipliers.GetMultiplier(GeneralManager.DifficultyMultipliers.Modifier.Damage, GeneralManager.CurrentDifficulty) : 1f;
    }
    #endregion
    public abstract partial class ChurroBaseAttack : MonoBehaviour
    {
        [SerializeField] float baseDamage = 5f;
        [SerializeField] int projectileLayerIndex = 100;
        [Header("Optional")]
        [field: SerializeField] protected DungeonUnit owner { get; private set; }
        [SerializeField] AttackHandler handler;
        [SerializeField] AudioClipWrapper attackSound;
        private void PerformContainedAttack(Vector2 target)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            ChurroProjectile.InputSettings s = new(owner == null ? transform.position : owner.CurrentPosition, target - (owner == null ? transform.position : (Vector2)owner.CurrentPosition));
            s.OnSpawn += ProjectileSpawnCallback;
            attackSound.Play(transform.position);
            AttackPayload(s);
        }
        public void ExternalForcedAttack()
        {
            PerformContainedAttack((Vector2)transform.position + Vector2.down);
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
        private void Start()
        {
            if (handler != null) handler.OnAttack += PerformContainedAttack;
        }
        private void OnDestroy()
        {
            if (handler != null) handler.OnAttack -= PerformContainedAttack;
        }
    }
}

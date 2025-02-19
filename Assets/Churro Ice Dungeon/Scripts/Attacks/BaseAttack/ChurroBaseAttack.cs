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
        [SerializeField] DungeonUnit owner;
        [SerializeField] float baseDamage = 5f;
        [SerializeField] AudioClipWrapper attackSound;
        private void PerformContainedAttack(Vector2 target)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            ChurroProjectile.InputSettings s = new(owner.CurrentPosition, target - (Vector2)owner.CurrentPosition);
            s.OnSpawn += ProjectileSpawnCallback;
            attackSound.Play(transform.position);
            AttackPayload(s);
        }
        protected abstract void AttackPayload(ChurroProjectile.InputSettings input);
        [SerializeField] int projectileLayerIndex = 100;
        [SerializeField] AttackHandler handler;
        private void ProjectileSpawnCallback(ChurroProjectile p)
        {
            p.Action_SetFaction(owner.Faction);
            p.Action_SetDamage(owner.DamageScale(baseDamage));
            p.Action_SetSpriteLayerIndex(projectileLayerIndex);
            p.Action_SetOwnerReference(owner);
        }
        private void Start()
        {
            handler.OnAttack += PerformContainedAttack;
        }
        private void OnDestroy()
        {
            handler.OnAttack -= PerformContainedAttack;
        }
    }
}

using Bremsengine;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class ProjectileTestAttack : MonoBehaviour
    {
        [SerializeField] DungeonUnit owner;
        [SerializeField] BremseFaction faction;
        [SerializeField] ChurroProjectile testPrefab;
        [SerializeField] float baseDamage = 5f;
        private void PerformContainedAttack(Vector2 target)
        {
            ChurroProjectile.InputSettings s = new(owner.UnitPosition, target - (Vector2)owner.UnitPosition);
            ChurroProjectile.ArcSettings arc = new(-45f, 45f, 15f, 5f);
            ChurroProjectile.SingleSettings single = new(0f, 5f);
            s.OnSpawn += ProjectileSpawnCallback;

            ChurroProjectile.SpawnArc(testPrefab, s, arc);
        }
        [SerializeField] int projectileLayerIndex = 100;
        [SerializeField] AttackHandler handler;
        private void ProjectileSpawnCallback(ChurroProjectile p)
        {
            p.Action_SetFaction(faction);
            p.Action_SetDamage(owner.DamageScale(baseDamage));
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

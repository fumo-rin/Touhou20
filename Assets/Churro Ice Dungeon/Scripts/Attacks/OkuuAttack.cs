using System.Collections;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class OkuuAttack : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile okuuProjectile;
        [SerializeField] float initialGrowModifier = 0.95f;
        [SerializeField] float growPerSecond = 1.25f;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            IEnumerator CO_Okuu(ChurroProjectile p)
            {
                while (p.transform != null && p.transform.localScale.x < 6f)
                {
                    p.transform.localScale += p.transform.localScale * (Time.deltaTime * initialGrowModifier);
                    p.transform.localScale = new(p.transform.localScale.x, p.transform.localScale.y, 1f);
                    yield return null;
                }
                while (p.transform != null && p.transform.localScale.x < 22f)
                {
                    p.transform.localScale += Vector3.one * (Time.deltaTime * growPerSecond);
                    p.transform.localScale = new(p.transform.localScale.x, p.transform.localScale.y, 1f);
                    yield return null;
                }
            }
            ChurroProjectile.SingleSettings settings = new(0f, 0f);
            ChurroProjectile p = ChurroProjectile.SpawnSingle(okuuProjectile, input, settings);
            p.StartCoroutine(CO_Okuu(p));
            if (p.ProjectileCollider != null)
            {
                foreach (var item in this.owner.AllColliders)
                {
                    Physics2D.IgnoreCollision(item, p.ProjectileCollider);
                }
            }
            p.Action_SetFaction(Bremsengine.BremseFaction.None);
            p.Action_SetDamage(1000f);
            p.Action_SetBounceLives(10000);
        }
    }
}

using Core.Extensions;
using System.Collections;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class OkuuAttack : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile okuuProjectile;
        [SerializeField] float initialGrowModifier = 0.95f;
        [SerializeField] float growPerSecond = 1.25f;
        private bool OwnerAlive => owner != null && owner.IsAlive();
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            IEnumerator CO_Okuu(ChurroProjectile p)
            {
                p.Action_AddPosition(Random.insideUnitCircle * 1.5f);
                while (p.transform != null && p.transform.localScale.x < 6f && OwnerAlive)
                {
                    p.transform.localScale += p.transform.localScale * (Time.deltaTime * initialGrowModifier);
                    p.transform.localScale = new(p.transform.localScale.x, p.transform.localScale.y, 1f);
                    yield return null;
                }
                while (p.transform != null && p.transform.localScale.x < 22f && OwnerAlive)
                {
                    p.transform.localScale += Vector3.one * (Time.deltaTime * growPerSecond);
                    p.transform.localScale = new(p.transform.localScale.x, p.transform.localScale.y, 1f);
                    yield return null;
                }
                yield return new WaitUntil(() => !OwnerAlive);
                while (p.transform != null && p.transform.localScale.x > 0.1f)
                {
                    p.transform.localScale -= (Vector3.one.Multiply(2.5f) + p.transform.localScale) * (Time.deltaTime * initialGrowModifier);
                    p.transform.localScale = new(p.transform.localScale.x, p.transform.localScale.y, 1f);
                    yield return null;
                }
                Destroy(p.gameObject);
            }
            ChurroProjectile.SingleSettings settings = new(0f, 0f);
            if (ChurroProjectile.SpawnSingle(okuuProjectile, input, settings, out ChurroProjectile p))
            {
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
            if (Hardmode)
            {
                handler.settings.SetSwingDuration(handler.settings.SwingDuration.MoveTowards(0.4f, 0.15f));
                handler.settings.SetStallDuration(handler.settings.StallDuration.MoveTowards(0.1f, 0.02f));
                owner.Action_SpeedmodTowards(3f, 0.075f);
            }
            else
            {
                handler.settings.SetSwingDuration(handler.settings.SwingDuration.MoveTowards(1.25f, 0.1f));
            }
        }
    }
}

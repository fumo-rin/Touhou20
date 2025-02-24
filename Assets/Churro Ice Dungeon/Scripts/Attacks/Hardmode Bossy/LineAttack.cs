using System.Collections;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class LineAttack : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile projectile;
        [SerializeField] ChurroProjectile crawlerPrefab;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            handler.settings.SetSwingDuration(1.6f);
            handler.settings.SetStallDuration(0.4f);
            foreach (var item in Arc(-20f, 20f, 40f, 2.5f).Spawn(input, projectile))
            {
                item.Action_AttachCrawlerEvent(crawlerPrefab, Arc(-60f, 60f, 30f, 1.5f), CrawlerPacket(1.5f, 180f, 20f, 180 / 20, 0.25f));
            }
        }
    }
}

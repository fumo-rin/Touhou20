using System.Collections;
using System.Collections.Generic;
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
            Arc(-20f, 20f, 20f, 2.5f).Spawn(input, projectile, out List<ChurroProjectile> output);
            foreach (var item in output)
            {
                item.Action_AttachCrawlerEvent(crawlerPrefab, Arc(-60f, 60f, 20f, 1.5f), CrawlerPacket(1.5f, 180f, 20f, 180 / 20, 0.25f));
            }
        }
    }
}

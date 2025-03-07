using Core.Extensions;
using System.Collections;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class TestEventsAttack : ChurroBaseAttack
    {
        [SerializeField] ChurroProjectile prefab;
        [SerializeField] ChurroProjectile crawlerPrefab;
        [SerializeField] float angle = 160f;
        [SerializeField] int arms = 4;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            void ApplyEvent(ChurroProjectile projectile)
            {
                projectile.Action_ClearDistance(20f);
            }
            IEnumerator CO_Emit()
            {
                var packet = CrawlerPacket(0.2f, 180f, 24f, 15, 0.1f);
                for (int i = 0; i < 2; i++)
                {
                    Arc(-angle.Multiply(0.5f), angle.Multiply(0.5f), angle / (arms - 1), 7f).Spawn(input, prefab, out iterationList);
                    foreach (var item in iterationList)
                    {
                        ApplyEvent(item);
                        packet.OnSpawn += ApplyEvent;
                        item.Action_AttachCrawlerEvent(crawlerPrefab, new(-5f, 5f, 10f, 1.6f), packet);
                    }
                    yield return new WaitForSeconds(0.3f);
                }
            }
            StartCoroutine(CO_Emit());
        }
    }
}
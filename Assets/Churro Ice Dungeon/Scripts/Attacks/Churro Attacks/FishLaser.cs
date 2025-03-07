using Core.Extensions;
using System.Collections;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class FishLaser : ChurroBaseAttack
    {
        [SerializeField] float verticalOffset = 0.35f;
        [SerializeField] float horizontalSeperation = 0.5f;
        [SerializeField] ChurroProjectile projectile;
        [SerializeField] ChurroProjectile arcProjectile;
        [SerializeField] AudioClipWrapper spamSound;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            IEnumerator CO_Burst()
            {
                WaitForSeconds repeatStall = new(0.03f);
                ChurroProjectile.SingleSettings single = new(0f, 40f);
                for (int i = 0; i < 5; i++)
                {
                    input.SetOrigin(owner.CurrentPosition);
                    ChurroProjectile.SpawnSingle(projectile, input, single).Action_AddPosition(new(-horizontalSeperation.Multiply(0.5f), verticalOffset));
                    ChurroProjectile.SpawnSingle(projectile, input, single).Action_AddPosition(new(horizontalSeperation.Multiply(0.5f), verticalOffset));
                    spamSound.Play(input.Origin);
                    yield return repeatStall;
                }
            }
            Arc(-25f, 25f, 50f / 8f, 35f).Spawn(input, arcProjectile);
            StartCoroutine(CO_Burst());
        }
    }
}

using UnityEngine;
using UnityEngine.Rendering;

namespace ChurroIceDungeon
{
    public class BowapAttack : ChurroBaseAttack
    {
        //Works Pretty well with 0.03 Fire Rate and -0.3f Spincrement
        float spin = 20f;
        [SerializeField] float spinIncrement = -0.3f;
        int spinDex = -15;
        [SerializeField] ChurroProjectile prefab;
        protected override void AttackPayload(ChurroProjectile.InputSettings input)
        {
            spin += (spinIncrement * spinDex);
            //spin += Mathf.Sin(Time.deltaTime * 30f) + -3f;
            spin = spin % 360f;
            spinDex++;
            ChurroProjectile.ArcSettings bowap = new(0f + spin, 360f + spin, 72f, 7f);
            ChurroProjectile.SpawnArc(prefab, input, bowap);
        }
    }
}

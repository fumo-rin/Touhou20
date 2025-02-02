using Bremsengine;
using Core.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    [System.Serializable]
    public class GunLoadout
    {
        public ProjectileGraphSO spreadShot;
        public ProjectileGraphSO FrontalLeft;
        public ProjectileGraphSO FrontalRight;
    }
    public class NewGunOutcome : WheelOutcome
    {
        public GunLoadout Current => currentGun == null ? DefaultLoadout : currentGun;
        [SerializeField] List<GunLoadout> guns = new();
        [SerializeField] ProjectileAttack Spreadshot;
        [SerializeField] ProjectileAttack FrontalLeft;
        [SerializeField] ProjectileAttack FrontalRight;
        [SerializeField] GunLoadout DefaultLoadout;
        [SerializeField] GunLoadout currentGun;
        public override void ApplyEffect(BaseUnit unit)
        {
            int attempts = 10;
            int random = 0.RandomBetween(0,guns.Count);
            GunLoadout p = currentGun;
            while (attempts > 0 && currentGun == p)
            {
                SetLoadout(guns[random]);
                attempts = attempts -1;
            }
        }
        private void SetLoadout(GunLoadout l)
        {
            Spreadshot.SetAttackGraph(l.spreadShot == null ? null: l.spreadShot);
            FrontalLeft.SetAttackGraph(l.FrontalLeft == null ? null : l.FrontalLeft);
            FrontalRight.SetAttackGraph(l.FrontalRight == null ? null : l.FrontalRight);
            currentGun = l;
        }

        public override void GameReset(BaseUnit unit)
        {
            SetLoadout(DefaultLoadout);
        }

        public override float GetDuration()
        {
            return 0f;
        }

        public override void RemoveEffect(BaseUnit unit)
        {

        }
    }
}

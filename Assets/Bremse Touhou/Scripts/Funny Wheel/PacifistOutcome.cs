using UnityEngine;
using Bremsengine;
using Core.Extensions;

namespace BremseTouhou
{
    public class PacifistOutcome : WheelOutcome
    {
        [SerializeField] ProjectileAttack Spreadshot;
        [SerializeField] ProjectileAttack FrontalLeft;
        [SerializeField] ProjectileAttack FrontalRight;
        [SerializeField] GunLoadout DefaultLoadout;
        [SerializeField] GunLoadout pacifistLoadout; 
        GunLoadout previousGun;
        [SerializeField] NewGunOutcome NewGunOutcome;
        public override void ApplyEffect(BaseUnit unit)
        {
            previousGun = NewGunOutcome.Current;
            SetLoadout(pacifistLoadout);
        }
        private void SetLoadout(GunLoadout l)
        {
            Spreadshot.SetAttackGraph(l.spreadShot == null ? null : l.spreadShot);
            FrontalLeft.SetAttackGraph(l.FrontalLeft == null ? null : l.FrontalLeft);
            FrontalRight.SetAttackGraph(l.FrontalRight == null ? null : l.FrontalRight);
        }
        private void ResetGun()
        {
            if (previousGun != null)
            {
                SetLoadout(previousGun);
            }
            else
            {
                SetLoadout(DefaultLoadout);
            }
        }
        public override void GameReset(BaseUnit unit)
        {
            previousGun = DefaultLoadout;
            ResetGun();
        }

        public override float GetDuration()
        {
            return 8f;
        }

        public override void RemoveEffect(BaseUnit unit)
        {
            ResetGun();
        }
    }
}

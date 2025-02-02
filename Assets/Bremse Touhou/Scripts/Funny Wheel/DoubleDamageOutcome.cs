using Bremsengine;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    public class DoubleDamageOutcome : WheelOutcome
    {
        [SerializeField] static List<DoubleDamageOutcome> damageList = new();
        [SerializeField] UnitDamageScaler scaler;
        public static float DamageModifier()
        {
            if (damageList == null || damageList.Count <= 0f)
            {
                return 1f;
            }
            return 2f;
        }

        public override void ApplyEffect(BaseUnit unit)
        {
            damageList.Add(this);
            scaler.ExternalDamageScale = DamageModifier();
        }

        public override void GameReset(BaseUnit unit)
        {
            damageList.Clear();
            scaler.ExternalDamageScale = DamageModifier();
        }

        public override float GetDuration()
        {
            return 10f;
        }

        public override void RemoveEffect(BaseUnit unit)
        {
            damageList.Remove(this);
            scaler.ExternalDamageScale = DamageModifier();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    public interface IRetargetAttack
    {
        public void AttackWithRetargetting(BaseUnit owner, BaseUnit target, Vector2 origin, float addedAngle);
    }
}

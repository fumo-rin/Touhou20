using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    [CreateAssetMenu(menuName = "Bremse Touhou/Aim Assist/Nearest")]
    public class AimAssistNearest : AimAssist
    {
        public override bool FetchTarget(BaseUnit owner, out BaseUnit target)
        {
            #region Sort By Distance
            int SortByDistance(BaseUnit a, BaseUnit b)
            {
                if (a == null && b == null)
                {
                    return 0;
                }
                if (a == null)
                {
                    return 1;
                }
                if (b == null)
                {
                    return -1;
                }
                return (a.Center.SquareDistanceTo(owner.Center)-(b.Center.SquareDistanceTo(owner.Center))).SignInt();
            }
            #endregion
            target = CollectTarget(owner, blockingLayer, SortByDistance);
            return target != null;
        }
    }
}
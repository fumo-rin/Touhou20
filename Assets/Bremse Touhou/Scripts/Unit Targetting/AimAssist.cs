using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Bremsengine;
using Core.Extensions;

namespace BremseTouhou
{
    #region Sorting Methods
    public partial class AimAssist
    {
        #region Sort By Max Health
        protected int SortByMaxHealth(BaseUnit a, BaseUnit b)
        {
            return (b.MaxHealth - a.MaxHealth).SignInt();
        }
        #endregion
    }
    #endregion
    #region Collection
    public partial class AimAssist
    {
        protected List<BaseUnit> Collect(BaseUnit owner, LayerMask layer, Comparison<BaseUnit> sort = null)
        {
            #region Check if anything is even nearby
            if (Physics2D.BoxCast(owner.Center, Box, 0f, Vector2.zero, Size, BaseUnit.EnemyLayer)
                .transform == null)
            {
                return new List<BaseUnit>();
            }
            #endregion
            hit = new RaycastHit2D[scanCount];
            if (Physics2D.BoxCastNonAlloc(owner.transform.position, Box, 0f, Vector2.zero, hit, size, layer) > 0)
            {
                List<BaseUnit> found = new();
                foreach (var entry in hit)
                {
                    if (entry.transform != null && entry.transform.GetComponent<BaseUnit>() is not null and BaseUnit u)
                    {
                        found.Add(u);
                    }
                }
                if (sort != null)
                {
                    found.Sort(sort);
                }
                return found.Where(x => x != null && x.Alive).ToList();
            }
            return new List<BaseUnit>();
        }
        protected BaseUnit CollectTarget(BaseUnit owner, LayerMask layer, Comparison<BaseUnit> sort = null)
        {
            BaseUnit target = null;
            List<BaseUnit> found = Collect(owner, BaseUnit.EnemyLayer, sort);
            int iteration = 0;
            while (target == null && iteration < found.Count)
            {
                target = found[iteration];
                if (target != null)
                    break;
            }
            return target;
        }
    }
    #endregion
    public abstract partial class AimAssist : ScriptableObject
    {
        [SerializeField] protected LayerMask layer;
        public float Size => size;
        public float Radius => size * 0.5f;
        protected Vector2 Box => new(Size, Size);
        [SerializeField] float size = 30f;
        [SerializeField] protected int scanCount = 30;
        protected RaycastHit2D[] hit;
        public abstract bool FetchTarget(BaseUnit owner, out BaseUnit target);
    }
}
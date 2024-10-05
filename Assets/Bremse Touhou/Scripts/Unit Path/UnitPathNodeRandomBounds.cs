using Core.Extensions;
using UnityEngine;

namespace BremseTouhou
{
    [CreateAssetMenu(menuName = "Bremse Touhou/Unit Path Node/Random Bounds")]
    public class UnitPathNodeRandomBounds : UnitPathNodeOverride
    {
        [SerializeField] Bounds bounds;
        protected override Vector2 RelativePositionToWorldCenter()
        {
            Vector2 v = bounds.center;
            v += bounds.RandomWithin(v);
            return v;
        }
    }
}

using Core.Extensions;
using UnityEngine;

namespace BremseTouhou
{
    [CreateAssetMenu(fileName = "Unit Path Wait", menuName = "Bremse Touhou/Unit Path Movement/Wait")]
    public class UnitPathWait : UnitPathAction
    {
        [SerializeField] float friction = 6f;
        public override void RunAction(BaseUnit unit, BaseUnit target, Vector2 position)
        {
            unit.Friction(friction);
        }
        public override void StartAction(BaseUnit unit, BaseUnit target, Vector2 position)
        {

        }
    }
}

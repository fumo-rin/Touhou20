using Core.Extensions;
using UnityEngine;

namespace BremseTouhou
{
    [CreateAssetMenu(fileName = "Unit Path Walk", menuName = "Bremse Touhou/Unit Path Movement/Walk")]
    public class UnitPathWalk : UnitPathAction
    {
        [SerializeField] float stoppingDistance = 1f;
        [SerializeField] UnitMotor motor;
        public override void RunAction(BaseUnit owner, BaseUnit target, Vector2 position)
        {
            if (owner.Center.SquareDistanceToLessThan(position, stoppingDistance))
            {
                owner.Move(motor, Vector2.zero);
                return;
            }
            Vector2 direction = position - owner.Center;
            owner.Move(motor, direction);
            Debug.DrawLine(owner.Center, position);
        }

        public override void StartAction(BaseUnit unit, BaseUnit target, Vector2 position)
        {

        }
    }
}

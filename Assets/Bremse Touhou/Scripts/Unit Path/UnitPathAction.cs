using UnityEngine;

namespace BremseTouhou
{
    public abstract class UnitPathAction : ScriptableObject
    {
        [field: SerializeField] public float Duration { get; protected set; } = 1f;
        public abstract void RunAction(BaseUnit owner, BaseUnit target, Vector2 position);
        public abstract void StartAction(BaseUnit unit, BaseUnit target, Vector2 position);
    }
}

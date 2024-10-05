using UnityEngine;

namespace BremseTouhou
{
    public abstract class UnitPathNodeOverride : ScriptableObject
    {
        protected abstract Vector2 RelativePositionToWorldCenter();
        public Vector2 Position => StageWorldCenter.Center + RelativePositionToWorldCenter();
    }
}

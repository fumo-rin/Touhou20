using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    [CreateAssetMenu(fileName = "UnitPath", menuName = "Bremse Touhou/Unit Path/Path")]
    public class UnitPath : ScriptableObject
    {
        [SerializeField] float noPathFriction = 6f;
        [SerializeField] List<PathEntry> path = new List<PathEntry>();
        public void StartPath(BaseUnit owner, BaseUnit target)
        {
            owner.StartUnitPathCoroutine(owner.StartCoroutine(CO_PerformPath(path, owner, target)));
        }
        IEnumerator CO_PerformPath(List<PathEntry> path, BaseUnit owner, BaseUnit target)
        {
            if (path == null || path.Count <= 0)
            {
                yield break;
            }
            float progress;
            foreach (PathEntry entry in path)
            {
                progress = 0f;
                Vector2 destination = entry.Position;
                entry.action.StartAction(owner, target, destination);
                bool actionEndCondition = true;
                while (progress < entry.action.Duration && actionEndCondition)
                {
                    entry.action.RunAction(owner, target, destination);
                    progress += Time.deltaTime;
                    yield return null;
                }
            }
            owner.StartCoroutine(CO_SlowDown(owner));
        }
        IEnumerator CO_SlowDown(BaseUnit owner)
        {
            while (owner.CurrentVelocity.magnitude > 0.5f)
            {
                owner.Friction(noPathFriction);
                yield return null;
            }
            owner.StopMovement();
            yield break;
        }
    }
    [System.Serializable]
    public class PathEntry
    {
        public UnitPathAction action;
        public Vector2 Position => (positionNodeOverride == null ? positionRelativeToWorldCenter + StageWorldCenter.Center : positionNodeOverride.Position);
        [SerializeField] private Vector2 positionRelativeToWorldCenter;
        [SerializeField] private UnitPathNodeOverride positionNodeOverride;
    }
}

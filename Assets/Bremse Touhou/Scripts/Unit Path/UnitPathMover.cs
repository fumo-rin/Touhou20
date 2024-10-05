using UnityEngine;

namespace BremseTouhou
{
    public class UnitPathMover : MonoBehaviour
    {
        [SerializeField] BaseUnit owner;
        [SerializeField] UnitPath path;
        [ContextMenu("Try Path")]
        private void TryPath()
        {
            if (path != null && owner != null && Application.isPlaying)
            {
                StartContainedPath();
            }
        }
        public void StartContainedPath()
        {
            path.StartPath(owner, owner.Target);
        }
        public void SetPath(UnitPath p)
        {
            p = path;
        }
        public void SetAndStartPath(UnitPath p)
        {
            SetPath(p);
            StartContainedPath();
        }
    }
}

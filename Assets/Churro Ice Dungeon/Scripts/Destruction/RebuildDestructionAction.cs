using UnityEngine;

namespace ChurroIceDungeon
{
    [DefaultExecutionOrder(1000)]
    public class RebuildDestructionAction : MonoBehaviour
    {
        private void Start()
        {
            ChurroManager.SLOW_BuildBar();
        }
    }
}

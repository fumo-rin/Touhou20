using UnityEngine;

namespace BremseTouhou
{
    [DefaultExecutionOrder(10)]
    public class SimpleStage : MonoBehaviour
    {
        [SerializeField] GameObject boss1;
        [SerializeField] GameObject boss2;
        [SerializeField] GameObject boss3;
        private void Awake()
        {
            TouhouManager.DefineProgressTree(0, () => Instantiate(boss1, transform.position, Quaternion.identity));
            TouhouManager.DefineProgressTree(0, () => SimpleScenery.SetScenery(0));
            TouhouManager.DefineProgressTree(1, () => Instantiate(boss2, transform.position, Quaternion.identity));
            TouhouManager.DefineProgressTree(1, () => SimpleScenery.SetScenery(1));
            TouhouManager.DefineProgressTree(2, () => Instantiate(boss3, transform.position, Quaternion.identity));
            TouhouManager.DefineProgressTree(2, () => SimpleScenery.SetScenery(2));
        }
    }
}

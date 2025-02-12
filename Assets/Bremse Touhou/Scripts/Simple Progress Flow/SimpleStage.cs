using Bremsengine;
using UnityEngine;

namespace BremseTouhou
{
    [DefaultExecutionOrder(10)]
    public class SimpleStage : MonoBehaviour
    {
        [SerializeField] BaseUnit boss1;
        [SerializeField] BaseUnit boss2;
        [SerializeField] BaseUnit boss3;
        [SerializeField] GameObject testStagePortion1;
        [SerializeField] GameObject testStagePortion2;
        [SerializeField] GameObject testStagePortion3;
        [SerializeField] MusicWrapper stageMusic1;
        private void Awake()
        {
            TouhouManager.DefineProgressTree(0, () => SimpleScenery.SetScenery(0));
            TouhouManager.DefineProgressTree(0, () => stageMusic1.Play());
            TouhouManager.DefineProgressTree(0, () => Instantiate(testStagePortion1, transform.position, Quaternion.identity));
            TouhouManager.DefineProgressTreeDuration(0, 25f);
            TouhouManager.DefineProgressTree(1, () => Instantiate(testStagePortion2, transform.position, Quaternion.identity));
            TouhouManager.DefineProgressTreeDuration(1, 25f);
            TouhouManager.DefineProgressTree(2, () => Instantiate(testStagePortion3, transform.position, Quaternion.identity));
            TouhouManager.DefineProgressTreeDuration(2, 40f);
            TouhouManager.DefineProgressTree(3, () => Instantiate(boss1, transform.position, Quaternion.identity).SetProgressDeletion(4));
            TouhouManager.DefineProgressTree(4, () => Instantiate(boss2, transform.position, Quaternion.identity).SetProgressDeletion(5));
            TouhouManager.DefineProgressTree(4, () => SimpleScenery.SetScenery(1));
            TouhouManager.DefineProgressTree(5, () => Instantiate(boss3, transform.position, Quaternion.identity).SetProgressDeletion(6));
            TouhouManager.DefineProgressTree(5, () => SimpleScenery.SetScenery(2));
        }
    }
}
using System.Collections;
using UnityEngine;

namespace Bremsengine
{
    public class SpriteMaterialQueue : MonoBehaviour
    {
        Coroutine activeRoutine;
        [field:SerializeField] public SpriteRenderer SR { get; private set; }
        public Material StandardMaterial { get; private set; }
        private void Awake()
        {
            StandardMaterial = SR.sharedMaterial;
        }
        public void RunMaterialQueue(IEnumerator Coroutine)
        {
            if (activeRoutine != null)
            {
                StopCoroutine(activeRoutine);
            }
            activeRoutine = GeneralManager.Instance.StartCoroutine(Coroutine);
        }
        public void TriggerOnComplete()
        {
            activeRoutine = null;
            SR.sharedMaterial = StandardMaterial;
        }
    }
}

using System.Collections;
using UnityEngine;

namespace Bremsengine
{
    public class SpriteMaterialQueue : MonoBehaviour
    {
        [field: SerializeField] public SpriteRenderer SR { get; private set; }
        public Material StandardMaterial { get; private set; }
        static Coroutine activeroutine;
        [SerializeField] Material flashMaterial;
        [SerializeField] float flashInterval;
        private void Awake()
        {
            StandardMaterial = SR.sharedMaterial;
        }
        public void RunMaterialQueue(float duration)
        {
            SR.sharedMaterial = StandardMaterial;
            if (activeroutine != null)
            {
                StopCoroutine(activeroutine);
            }
            activeroutine = StartCoroutine(CO_FlashMaterial(flashMaterial, duration, flashInterval));
        }
        public IEnumerator CO_FlashMaterial(Material flashMaterial, float duration, float flashInterval)
        {
            SR.sharedMaterial = StandardMaterial;
            float endTime = Time.time + duration;
            while (Time.time < endTime)
            {
                Material determinedMaterial = SR.sharedMaterial != flashMaterial ? flashMaterial : StandardMaterial;
                SR.sharedMaterial = determinedMaterial;
                yield return new WaitForSeconds(flashInterval);
            }
            TriggerOnComplete();
        }
        public void TriggerOnComplete()
        {
            SR.sharedMaterial = StandardMaterial;
        }
    }
}
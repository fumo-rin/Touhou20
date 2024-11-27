using System.Collections;
using UnityEngine;

namespace Bremsengine
{
    public class SpriteFlashMaterial : MonoBehaviour
    {
        [SerializeField] SpriteMaterialQueue materialQueue;
        [SerializeField] Material flashMaterial;
        [SerializeField] float duration;
        [SerializeField] float flashInterval;

        [ContextMenu("Activate Flash")]
        public void TriggerFlashMaterial()
        {
            materialQueue.RunMaterialQueue(CO_FlashMaterial(flashMaterial, duration, flashInterval));
        }
        public IEnumerator CO_FlashMaterial(Material flashMaterial, float duration, float flashInterval)
        {
            float endTime = Time.time + duration;
            while (Time.time < endTime)
            {
                Material determinedMaterial = materialQueue.SR.sharedMaterial != flashMaterial ? flashMaterial : materialQueue.StandardMaterial;
                materialQueue.SR.sharedMaterial = determinedMaterial;
                yield return new WaitForSeconds(flashInterval);
            }
            materialQueue.TriggerOnComplete();
        }
    }
}

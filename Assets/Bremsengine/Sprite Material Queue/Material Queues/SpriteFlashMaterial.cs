using System.Collections;
using UnityEngine;

namespace Bremsengine
{
    public class SpriteFlashMaterial : MonoBehaviour
    {
        [SerializeField] SpriteMaterialQueue materialQueue;

        [ContextMenu("Activate Flash")]
        public void TriggerFlashMaterial(float duration)
        {
            Debug.Log(duration);
            materialQueue.RunMaterialQueue(duration);
        }
    }
}

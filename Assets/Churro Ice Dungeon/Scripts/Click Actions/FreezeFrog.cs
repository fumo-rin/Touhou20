using UnityEngine;
using Bremsengine;
using System.Collections;
using Core.Extensions;

namespace ChurroIceDungeon
{
    public class FreezeFrog : RenderTextureClickAction
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        Coroutine freezeRoutine;
        public override void ClickPayload()
        {
            if (freezeRoutine != null)
            {
                StopCoroutine(freezeRoutine);
            }
            freezeRoutine = StartCoroutine(CO_FreezeFrog(5f));
        }
        private IEnumerator CO_FreezeFrog(float thawTime)
        {
            spriteRenderer.color = ColorHelper.DeepBlue;
            yield return new WaitForSeconds(thawTime);
            spriteRenderer.color = Color.white;
        }
    }
}

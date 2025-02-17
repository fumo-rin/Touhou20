using Mono.CSharp;
using TMPro;
using UnityEngine;

namespace Bremsengine
{
    public class RenderTextureHoverTooltip : MonoBehaviour
    {
        [SerializeField] Collider2D tooltipHoverCollider;
        [SerializeField] string TooltipText;
        public string Tooltip => TooltipText;
        public void Hover()
        {
            RenderTextureHoverTooltipUI.SetTooltipText(this);
        }
    }
}

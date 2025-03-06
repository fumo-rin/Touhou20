using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace Bremsengine
{
    public class RenderTextureHoverTooltipUI : MonoBehaviour, IPointerMoveHandler
    {
        private static RenderTextureHoverTooltipUI instance;
        [SerializeField] TMP_Text tooltipText;
        [SerializeField] RectTransform tooltipRect;
        static RenderTextureHoverTooltip activeTooltip;
        Vector2 cursorPosition;
        private void Awake()
        {
            instance = this;
        }
        public static void ClearTooltipText()
        {
            if (instance != null)
            {
                instance.tooltipText.enabled = false;
                activeTooltip = null;
            }
        }
        public static void SetTooltipText(RenderTextureHoverTooltip text)
        {
            if (instance != null)
            {
                instance.tooltipText.text = text.Tooltip;
                instance.tooltipText.enabled = true;
            }
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            cursorPosition = eventData.position;
        }
    }
}

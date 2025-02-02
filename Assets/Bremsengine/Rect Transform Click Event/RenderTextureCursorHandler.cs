using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Bremsengine
{
    public class RenderTextureCursorHandler : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler, IPointerExitHandler, IPointerEnterHandler
    {
        [SerializeField] RectTransform renderTexture;
        [SerializeField] Camera renderCamera;

        public delegate void OnClick(Vector2 worldPosition);
        public static event OnClick StaticRectWorldPositionClick;
        private static Vector2 lastCursorPosition;
        public static Vector2 CursorPosition => lastCursorPosition;
        public static bool IsHovering { get; private set; }
        private void Start()
        {
            SceneManager.activeSceneChanged += (Scene s, Scene ss) => { renderCamera = Camera.main; };
        }
        private void OnDestroy()
        {
            
        }
        public void OnPointerMove(PointerEventData eventData)
        {
            if (RenderTextureContainsMousePosition(out Vector2 click, eventData, renderTexture))
            {
                ScaleRenderClickToCameraWorldPosition(out Vector2 worldPosition, click, renderCamera);
                lastCursorPosition = worldPosition;
            }
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            IsHovering = false;
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            IsHovering = true;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (RenderTextureContainsMousePosition(out Vector2 click, eventData, renderTexture))
            {
                ScaleRenderClickToCameraWorldPosition(out Vector2 worldPosition, click, renderCamera);
                StaticRectWorldPositionClick?.Invoke(worldPosition);
            }
        }
        private void ScaleRenderClickToCameraWorldPosition(out Vector2 worldPosition, Vector2 normalizedClick, Camera camera)
        {
            Vector2 cameraSize = Vector2.zero;
            cameraSize.y = renderCamera.orthographicSize * 2f;
            cameraSize.x = cameraSize.y * renderCamera.aspect;
            worldPosition = new Vector2(normalizedClick.x * cameraSize.x, normalizedClick.y * cameraSize.y) + (Vector2)renderCamera.transform.position;
            worldPosition -= cameraSize * 0.5f;
        }
        private bool RenderTextureContainsMousePosition(out Vector2 normalizedPosition, PointerEventData pointer, RectTransform rendererRect)
        {
            normalizedPosition = Vector2.zero;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rendererRect, pointer.position, pointer.pressEventCamera, out var localPosition))
            {
                return false;
            }
            normalizedPosition = Rect.PointToNormalized(rendererRect.rect, localPosition);
            return true;
        }
    }
}

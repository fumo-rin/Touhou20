using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Bremsengine
{
    public enum PointerButton
    {
        Left,
        Right,
        Middle
    }
    public class RenderTextureCursorHandler : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler, IPointerExitHandler, IPointerEnterHandler, IPointerUpHandler
    {
        [SerializeField] RectTransform renderTexture;
        [SerializeField] Camera renderCamera;

        public delegate void OnClick(Vector2 worldPosition, PointerButton pressType);
        public static event OnClick ClickDown;
        public static event OnClick ClickUp;
        static bool IsPressed;
        private static Vector2 lastCursorPosition;
        static PointerEventData lastPointerData;
        public static Vector2 CursorPosition => lastCursorPosition;
        public static bool IsHovering { get; private set; }
        private void Start()
        {
            SceneManager.activeSceneChanged += (Scene s, Scene ss) => { renderCamera = Camera.main; };
        }
        private void RebuildCurrentPosition()
        {
            if (lastPointerData == null)
                return;
            if (RenderTextureContainsMousePosition(out Vector2 click, lastPointerData, renderTexture))
            {
                ScaleRenderClickToCameraWorldPosition(out Vector2 w, click, Camera.main);
                lastCursorPosition = w;
            }
        }
        private void OnDestroy()
        {

        }
        private void Update()
        {
            RebuildCurrentPosition();
        }
        private void Awake()
        {
            IsPressed = false;
        }
        public void OnPointerMove(PointerEventData eventData)
        {
            if (RenderTextureContainsMousePosition(out Vector2 click, eventData, renderTexture))
            {
                ScaleRenderClickToCameraWorldPosition(out Vector2 worldPosition, click, Camera.main);
                lastCursorPosition = worldPosition;
                lastPointerData = eventData;
            }
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            IsHovering = false;
            if (IsPressed)
            {
                TriggerPressEvent(eventData, ClickUp);
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            IsHovering = true;
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            TriggerPressEvent(eventData, ClickUp);
            IsPressed = false;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            TriggerPressEvent(eventData, ClickDown);
            IsPressed = true;
        }
        private void TriggerPressEvent(PointerEventData eventData, OnClick action)
        {
            if (RenderTextureContainsMousePosition(out Vector2 click, eventData, renderTexture))
            {
                ScaleRenderClickToCameraWorldPosition(out Vector2 worldPosition, click, Camera.main);
                PointerButton pressType = PointerButton.Left;
                switch (eventData.button)
                {
                    case PointerEventData.InputButton.Left:
                        break;
                    case PointerEventData.InputButton.Right:
                        pressType = PointerButton.Right;
                        break;
                    case PointerEventData.InputButton.Middle:
                        pressType = PointerButton.Middle;
                        break;
                    default:
                        break;
                }
                action?.Invoke(worldPosition, pressType);
            }
        }
        private void ScaleRenderClickToCameraWorldPosition(out Vector2 worldPosition, Vector2 normalizedClick, Camera fallbackCamera)
        {
            if (renderCamera == null)
            {
                renderCamera = fallbackCamera;
            }
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

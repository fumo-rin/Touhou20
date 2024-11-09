using Core.Input;
using UnityEngine;
using Core.Extensions;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Bremsengine;

namespace BremseTouhou
{
    public class CameraClickTest : MonoBehaviour
    {
        [SerializeField] GameObject spawnOnClick;
        private void Start()
        {
            RenderTextureCursorHandler.StaticRectWorldPositionClick += OnWorldClick;
        }
        private void OnDestroy()
        {
            RenderTextureCursorHandler.StaticRectWorldPositionClick -= OnWorldClick;
        }
        private void OnWorldClick(Vector2 position)
        {
            Instantiate(spawnOnClick, position, Quaternion.identity);
        }
    }
}

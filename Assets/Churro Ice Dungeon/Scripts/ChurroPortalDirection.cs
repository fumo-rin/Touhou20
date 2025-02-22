using Core.Extensions;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class ChurroPortalDirection : MonoBehaviour
    {
        static ChurroPortalDirection instance;
        [SerializeField] Transform rotationAnchor;
        [SerializeField] GameObject hideAnchor;
        static Vector2 storedWorldPosition;
        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {
            SetVisibility(false);
        }
        public static void SetVisibility(bool state)
        {
            if (instance == null)
                return;

            instance.hideAnchor.SetActive(state);
        }
        private void Update()
        {
            if (hideAnchor.activeInHierarchy)
            {
                instance.rotationAnchor.Lookat2D(storedWorldPosition);
            }
        }
        public static void SetDirection(Vector2 worldTarget)
        {
            storedWorldPosition = worldTarget;
            if (instance == null)
                return;
            instance.rotationAnchor.Lookat2D(worldTarget);
        }
    }
}

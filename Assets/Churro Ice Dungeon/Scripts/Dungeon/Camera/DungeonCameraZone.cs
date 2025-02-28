using UnityEngine;

namespace ChurroIceDungeon
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class DungeonCameraZone : MonoBehaviour
    {
        [Range(-100, 100)]
        public int ZoneIndex = 0;
        [SerializeField] bool useHeightAsCameraSize;
        [Range(-1, 12)]
        [SerializeField] int overrideCameraSize = -1;
        public float OverrideSize => useHeightAsCameraSize ? zoneConfiner.bounds.extents.y : overrideCameraSize;
        public Collider2D zoneConfiner { get; private set; }
        private void Awake()
        {
            zoneConfiner = GetComponent<BoxCollider2D>();
            zoneConfiner.enabled = true;
            zoneConfiner.isTrigger = true;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out ChurroUnit u))
            {
                DungeonCamera.BindZone(this);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out ChurroUnit u))
            {
                DungeonCamera.ReleaseZone(this);
            }
        }
        public static int SortByIndex(DungeonCameraZone a, DungeonCameraZone b)
        {
            return b.ZoneIndex.CompareTo(a.ZoneIndex);
        }
    }
}

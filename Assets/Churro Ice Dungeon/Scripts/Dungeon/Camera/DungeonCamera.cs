using Core.Extensions;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class DungeonCamera : MonoBehaviour
    {
        static DungeonCamera instance;
        [SerializeField] List<DungeonCameraZone> currentZones = new();
        [SerializeField] CinemachineCamera cam;
        [SerializeField] CinemachineConfiner2D confiner;
        float defaultCameraSize;
        public static float MinCameraSize => instance == null ? 6f : instance.defaultCameraSize;
        private void Recalculate()
        {
            Debug.Log("Recalculate");
            if (currentZones.Count > 0)
            {
                currentZones.Sort(DungeonCameraZone.SortByIndex);
                foreach (DungeonCameraZone zone in currentZones)
                {
                    Debug.Log(zone.zoneConfiner);
                }
                DungeonCameraZone selection = currentZones[0];
                confiner.BoundingShape2D = selection.zoneConfiner == null ? null : selection.zoneConfiner;
                cam.Lens.OrthographicSize = selection.OverrideSize > 0f ? selection.OverrideSize : defaultCameraSize;
            }
            else
            {
                confiner.BoundingShape2D = null;
            }
        }
        private void Awake()
        {
            instance = this;
            defaultCameraSize = cam.Lens.OrthographicSize;
        }
        public static void BindZone(DungeonCameraZone z)
        {
            if (instance == null)
            {
                Debug.Log("No Dungeon Camera Instance");
                return;
            }
            instance.currentZones.AddIfDoesntExist(z);
            instance.Recalculate();
        }
        public static void ReleaseZone(DungeonCameraZone z)
        {
            if (instance == null)
            {
                Debug.Log("No Dungeon Camera Instance");
                return;
            }
            instance.currentZones.Remove(z);
            instance.Recalculate();
        }
    }
}

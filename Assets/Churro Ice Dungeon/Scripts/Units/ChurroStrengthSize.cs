using Core.Extensions;
using Unity.Cinemachine;
using UnityEngine;
using System.Collections.Generic;

namespace ChurroIceDungeon
{
    public class ChurroStrengthSize : MonoBehaviour
    {
        [SerializeField] Transform scaleAnchor;
        [SerializeField] float standardCameraSize = 6f;
        [SerializeField] CinemachineCamera vCam;
        [SerializeField] List<Transform> extraScaleObjects;
        [SerializeField] List<Transform> extraCameraScaleObjects;
        private void SetStrength(int value)
        {
            float fvalue = ((float)value).Clamp(0f, 3000f);
            float size = ((1f + (fvalue).Percentify() * 0.25f)).Max(1f);
            scaleAnchor.localScale = new(size, size, 1f);
            float cameraMultiplier = Mathf.Sqrt(size).Max(1f);
            float cameraSize = standardCameraSize * cameraMultiplier;
            vCam.Lens.OrthographicSize = cameraSize;
            foreach (Transform t in extraScaleObjects)
            {
                t.localScale = new(size, size, 1f);
            }
            foreach (Transform t in extraCameraScaleObjects)
            {
                t.localScale = new(cameraMultiplier, cameraMultiplier, 1f);
            }
        }
        private void Start()
        {
            ChurroManager.OnStrengthChange += SetStrength;
            ChurroManager.RequestStatsRefresh();
        }
        private void OnDestroy()
        {
            ChurroManager.OnStrengthChange -= SetStrength;
        }
    }
}

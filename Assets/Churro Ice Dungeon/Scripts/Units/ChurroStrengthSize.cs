using Core.Extensions;
using Unity.Cinemachine;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class ChurroStrengthSize : MonoBehaviour
    {
        [SerializeField] Transform scaleAnchor;
        [SerializeField] float standardCameraSize = 6f;
        [SerializeField] CinemachineCamera vCam;
        private void SetStrength(int value)
        {
            float size = ((1f + ((float)value).Percentify() * 0.25f)).Max(1f);
            scaleAnchor.localScale = new(size, size, 1f);
            vCam.Lens.OrthographicSize = standardCameraSize * Mathf.Sqrt(size).Max(1f);
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

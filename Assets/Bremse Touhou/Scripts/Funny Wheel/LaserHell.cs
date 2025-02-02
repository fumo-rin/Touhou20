using Core.Extensions;
using System.Collections;
using UnityEngine;

namespace BremseTouhou
{
    public class LaserHell : MonoBehaviour
    {
        public LineRenderer lineRenderer;
        [SerializeField] LayerMask targetLayer;
        [SerializeField] int count = 6;
        [SerializeField] float maxRange = 25f;
        [SerializeField] float minRange = 15f;
        float delay = 1f;
        public void Clear()
        {
            Destroy(gameObject);
        }
        public void Build(Vector2 origin)
        {
            delay = 1f;
            lineRenderer.positionCount = 0;
            Vector2 iteration = origin;
            for (int i = 0; i < count; i++)
            {
                lineRenderer.positionCount = count;
                lineRenderer.SetPosition(i, iteration);
                iteration += Random.insideUnitCircle.ScaleToMagnitude(1f.AddRandomBetween(minRange, maxRange));
            }
            lineRenderer.widthMultiplier = 0.2f;
        }
        private void LerpTowards(float size, float speed)
        {
            lineRenderer.widthMultiplier = Mathf.Lerp(lineRenderer.widthMultiplier, size, speed * Time.deltaTime);
        }
        private void Update()
        {
            delay -= Time.deltaTime;
            if (delay <= 0.3f)
            {
                LerpTowards(1.35f, 1f);
                if (delay <= 0f)
                {
                    RaycastHit2D hit;
                    for (int i = 0; i < lineRenderer.positionCount - 1; i++)
                    {
                        Vector2 origin = lineRenderer.GetPosition(i);
                        Vector2 direction = (Vector2)(lineRenderer.GetPosition(i + 1)) - origin;
                        hit = Physics2D.Raycast(origin, direction, direction.magnitude, targetLayer);
                        if (hit.transform != null)
                        {
                            PlayerUnit p = PlayerUnit.Player as PlayerUnit;
                            p.ForceHit();
                        }
                    }
                }
                return;
            }
        }
    }
}

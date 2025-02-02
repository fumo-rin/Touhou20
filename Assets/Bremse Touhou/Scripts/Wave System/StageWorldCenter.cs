using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bremsengine;
using UnityEngine.UI;
using Core.Extensions;

namespace BremseTouhou
{
    [DefaultExecutionOrder(-10)]
    public class StageWorldCenter : MonoBehaviour
    {
        public static Vector2 Center { get; private set; }
        private void Awake()
        {
            Center = transform.position;
            StageSpawnPoint.SetStageCenter(this);
        }
        private void Start()
        {
            DirectionSolver.SetBounds(Center, CalculateSize());
            SplinePathSettings.SetWorldCenter(Center);
        }
        private static Vector2 CalculateSize()
        {
            Vector2 size = new(10f, 10f);
            if (Camera.main is not null and Camera c)
            {
                Debug.Log("T");
                float height = (c.orthographicSize * 2f);
                size = new(height * c.aspect, height);
            }
            else
            {
                Debug.Log("Failed to find camera main to calculate stage world size");
            }
            return size;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}

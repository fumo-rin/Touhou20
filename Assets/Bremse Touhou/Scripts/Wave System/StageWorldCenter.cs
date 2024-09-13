using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    [DefaultExecutionOrder(-10)]
    public class StageWorldCenter : MonoBehaviour
    {
        private void Awake()
        {
            StageSpawnPoint.SetStageCenter(this);
        }
    }
}

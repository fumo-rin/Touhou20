using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    [CreateAssetMenu(menuName ="Bremse Touhou/Stageset/Spawn Point")]
    public class StageSpawnPoint : ScriptableObject
    {
        private static Vector2 worldPosition;
        [SerializeField] Vector2 spawnPointOffset;
        public Vector2 spawnOffset => worldPosition + spawnPointOffset;
        public static void SetStageCenter(StageWorldCenter center)
        {
            worldPosition = center.transform.position;
        }
    }
}

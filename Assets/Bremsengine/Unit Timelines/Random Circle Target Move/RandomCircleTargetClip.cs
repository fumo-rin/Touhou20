using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Bremsengine
{
    public class RandomCircleTargetClip : PlayableAsset
    {
        [System.Serializable]
        public class data : ICloneable
        {
            public bool useStageOffset;
            public static Vector2 stageOffset => DirectionSolver.CurrentCenter;
            public Vector2? nullableStartingPosition;
            public Vector2 targetPosition;
            public Vector2 LerpTargetPosition;
            public float radius;
            public Vector2 GetNewPosition()
            {
                return (Vector2)(useStageOffset ? stageOffset : Vector2.zero) + (Vector2)(targetPosition + UnityEngine.Random.insideUnitCircle * radius);
            }

            public object Clone()
            {
                return new data()
                {
                    useStageOffset = this.useStageOffset,
                    nullableStartingPosition = this.nullableStartingPosition,
                    targetPosition = this.targetPosition,
                    LerpTargetPosition = this.LerpTargetPosition,
                    radius = this.radius
                };
            }
        }
        [SerializeField] data clipData;
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<RandomCircleTargetClipRunner>.Create(graph);

            RandomCircleTargetClipRunner handler = playable.GetBehaviour();
            handler.data = (data)clipData.Clone();
            return playable;
        }
    }
}

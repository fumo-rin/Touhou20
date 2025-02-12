using UnityEngine;
using UnityEngine.Playables;

namespace Bremsengine
{
    public class RandomCircleTargetClipRunner : PlayableBehaviour
    {
        public RandomCircleTargetClip.data data;
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            float EaseInOut(float t)
            {
                t = Mathf.Clamp01(t); // Ensure t is between 0 and 1
                return t * t * (3f - 2f * t); // Smoothstep formula
            }
            Transform owner = playerData as Transform;
            if (owner is null)
                return;
            if (!Application.isPlaying)
                return;
            Vector2 startingPosition = owner.position;
            if (data.nullableStartingPosition == null)
            {
                startingPosition = owner.position;
                data.nullableStartingPosition = startingPosition;
                data.LerpTargetPosition = data.GetNewPosition();
            }
            else
            {
                startingPosition = (Vector2)data.nullableStartingPosition;
            }
            float duration = (float)playable.GetDuration();
            float remainingTime = (float)(playable.GetDuration() - playable.GetTime());
            float lerp = (duration - remainingTime) / duration;
            owner.position = Vector2.Lerp(startingPosition, data.LerpTargetPosition, EaseInOut(lerp));
        }
    }
}

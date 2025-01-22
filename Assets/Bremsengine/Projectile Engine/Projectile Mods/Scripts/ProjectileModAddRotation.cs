using Core.Extensions;
using System.Collections;
using UnityEngine;

namespace Bremsengine
{
    [CreateAssetMenu(menuName = "Bremsengine/Projectile Mods/AddRotation", fileName = "Add Rotation")]
    public class ProjectileModAddRotation : ProjectileMod
    {
        [SerializeField] AnimationCurve addedRotation = Helper.InitializedAnimationCurve;
        protected override IEnumerator CO_ModifierPayload(Projectile p, WaitForSeconds delay)
        {
            yield return delay;
            float iterationTime = 0;
            float lastEvaluation = 0f;
            float evaluation = 0f;
            float lastTime = Time.time;
            float deltaTime;
            while (iterationTime < addedRotation.length)
            {
                deltaTime = Time.time - lastTime;
                iterationTime += deltaTime;
                evaluation = addedRotation.Evaluate(iterationTime);
                p.Post_AddRotation(evaluation - lastEvaluation);
                lastEvaluation = evaluation;
                lastTime = Time.time;
                yield return GetTickratedDelay();
            }
        }
    }
}

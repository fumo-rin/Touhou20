using Core.Extensions;
using System.Collections;
using UnityEngine;

namespace Bremsengine
{
    [CreateAssetMenu(menuName ="Bremsengine/Projectile Mods/AddRotation",fileName ="Add Rotation")]
    public class ProjectileModAddRotation : ProjectileMod
    {
        [SerializeField] AnimationCurve addedRotation = Helper.InitializedAnimationCurve;

        protected override IEnumerator CO_ModifierPayload(Projectile p, WaitForSeconds delay)
        {
            yield return delay;
            float iterationTime = 0;
            float lastEvaluation = 0f;
            float evaluation = 0f;
            while (iterationTime < addedRotation.length)
            {
                evaluation = addedRotation.Evaluate(iterationTime);
                p.Post_AddRotation(evaluation - lastEvaluation);
                lastEvaluation = evaluation;
                iterationTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}

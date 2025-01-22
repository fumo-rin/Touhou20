using Core.Extensions;
using System.Collections;
using UnityEngine;

namespace Bremsengine
{
    [CreateAssetMenu(menuName = "Bremsengine/Projectile Mods/AddRotation", fileName = "Add Rotation")]
    public class ProjectileModAddRotation : ProjectileMod
    {
        [SerializeField] AnimationCurve addedRotation = Helper.InitializedAnimationCurve;

        public override void RunMod(Projectile p, float remainingDuration, out float newDuration)
        {
            newDuration = remainingDuration - Time.deltaTime;
            if (duration < 0f)
            {
                return;
            }
            float lastEvaluation = addedRotation.Evaluate(remainingDuration);
            addedRotation.Evaluate(duration - remainingDuration);
            p.Post_AddRotation(lastEvaluation - lastEvaluation);
        }

        protected override void AddModTo(Projectile p)
        {
            p.AddMod(this);
        }

        protected override IEnumerator CO_ModifierPayload(Projectile p, WaitForSeconds delay)
        {
            yield return delay;
            AddModTo(p);
            /*
            float iterationTime = 0;
            float lastEvaluation = 0f;
            float evaluation = 0f;
            while (iterationTime < addedRotation.length)
            {
                iterationTime += Time.deltaTime;
                evaluation = addedRotation.Evaluate(iterationTime);
                p.Post_AddRotation(evaluation - lastEvaluation);
                lastEvaluation = evaluation;
                yield return GetTickratedDelay();
            }*/
        }
    }
}

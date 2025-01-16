using Bremsengine;
using UnityEngine;

namespace BremseTouhou
{
    public class PlayerScoring : MonoBehaviour
    {
        float grazeMultiplier = 1f;
        const float grazeModifier = 0.0001f;
        [SerializeField] float baseScorePerGraze = 1000f;
        private void Start()
        {
            GrazeBox.OnGraze += GrazeAction;
        }
        private void OnDestroy()
        {
            GrazeBox.OnGraze -= GrazeAction;
        }
        private void GrazeAction(int grazeCount)
        {
            grazeMultiplier = 1f + (grazeCount * grazeModifier);
            GrazeScore(1000f);
        }
        private void GrazeScore(float scorePreMultiplier = 1000f)
        {
            GeneralManager.AddScore(scorePreMultiplier * grazeMultiplier);
        }
    }
}

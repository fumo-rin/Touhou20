using UnityEngine;
using Bremsengine;

namespace BremseTouhou
{
    public class TargetBoxDropLoot : MonoBehaviour
    {
        [SerializeField] TargetBox box;
        private void SpawnScore(float damage, Vector2 position)
        {
            PlayerScoring.SpawnPickup(position);
        }
        private void Start()
        {
            box.OnTakeDamage += SpawnScore;
        }
        private void OnDestroy()
        {
            box.OnTakeDamage -= SpawnScore;
        }
    }
}

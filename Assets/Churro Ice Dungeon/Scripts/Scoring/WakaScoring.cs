using Bremsengine;
using Core.Extensions;
using UnityEngine;

namespace ChurroIceDungeon
{
    #region Score Items
    public partial class WakaScoring
    {
        [SerializeField] Pickup pickupPrefab;

        [QFSW.QC.Command("spawnpickup")]
        public static void Command_Pickups(int amount, Vector2 worldSpace)
        {
            for (int i = 0; i < amount; i++)
            {
                SpawnPickup(worldSpace + Random.insideUnitCircle);
            }
        }
        public static void SpawnPickup(Vector2 worldSpace)
        {
            if (instance == null)
                return;
            Pickup.SpawnPickup(instance.pickupPrefab, worldSpace);
        }
    }
    #endregion
    [DefaultExecutionOrder(15)]
    public partial class WakaScoring : MonoBehaviour
    {
        [Header("Base Score")]
        private static WakaScoring instance;
        float scoreMultiplier = 1f;
        const float grazeModifier = 0.0001f;
        static int grazeCount = 0;
        static int scoreItemAddedValue = 0;
        [SerializeField] float baseScorePerGraze = 1000f;
        [SerializeField] float baseScorePerPickup = 5000f;
        public static bool HasInstance => instance != null && instance.gameObject != null;
        public static string ScoreItemText()
        {
            string s = $"{scoreItemAddedValue + instance.baseScorePerPickup} x {GrazeMultiplier.ToString("F3")}";
            return s;
        }
        public static float GrazeMultiplier => (1f + (grazeCount * grazeModifier));
        private float MultiplyScore(float score)
        {
            scoreMultiplier = GrazeMultiplier;
            return scoreMultiplier * score;
        }
        public static void AddPickupScore()
        {
            scoreItemAddedValue += 1;
            float addedScore = instance.AddScore(instance.baseScorePerPickup + scoreItemAddedValue);
            if (GeneralManager.ShouldAddScoreKey)
            {
                GeneralManager.AddScoreAnalysisKey("Pickups", addedScore);
            }
        }
        private float AddScore(float addedScore)
        {
            return GeneralManager.AddScore(MultiplyScore(addedScore));
        }
        public static void PlayerDeathRecalculateScoreValue(float multiplier)
        {
            scoreItemAddedValue = (int)(((float)scoreItemAddedValue) * multiplier.Clamp(0f, 1f));
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Reinitialize()
        {
            scoreItemAddedValue = 0;
            grazeCount = 0;
        }
        private void FixedUpdate()
        {

        }
        private void Awake()
        {
            instance = this;
        }
        private void BulletClearSpawnPickup(Vector2 position)
        {
            if (Helper.SeededRandomInt256 < 150)
                return;

        }
        private void Start()
        {
            GrazeBox.OnGraze += GrazeAction;
        }
        private void OnDestroy()
        {
            GrazeBox.OnGraze -= GrazeAction;
        }
        private void GrazeAction(int graze)
        {
            grazeCount = graze;
            float score = AddScore(baseScorePerGraze);
            if (GeneralManager.ShouldAddScoreKey)
            {
                GeneralManager.AddScoreAnalysisKey("Grazing", score);
            }
        }
    }
}

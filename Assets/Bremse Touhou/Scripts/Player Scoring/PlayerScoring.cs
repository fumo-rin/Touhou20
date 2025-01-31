using Bremsengine;
using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BremseTouhou
{
    #region Graze Auto Loot
    public partial class PlayerScoring
    {
        static bool grazeAutoLoot => grazedRecently >= instance.grazedRecentlyToAutoloot;
        static float grazedRecently = 0;
        static float grazeDecayFreezeEndTime;
        [SerializeField] float grazedRecentlyToAutoloot = 40f;
        [SerializeField] float grazeDecayRate = 5f;
        [SerializeField] float grazeDecayMaxFreezeTime = 1.66f;
        public delegate void GrazeBarDataEvent(float value, float min, float max);
        public static GrazeBarDataEvent OnGrazeRefresh;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ResetGrazeAutoLoot()
        {
            grazedRecently = 0;
            grazeDecayFreezeEndTime = 0f;
        }
        private static void RunGrazeAutoLootLoop()
        {
            if (Time.time > grazeDecayFreezeEndTime && grazedRecently > 0)
            {
                grazedRecently -= Time.deltaTime * instance.grazeDecayRate;
            }
            OnGrazeRefresh?.Invoke(grazedRecently, 0f, instance.grazedRecentlyToAutoloot);
        }
        private static void AddGrazeAndRefreshAutoLoot()
        {
            grazedRecently = (grazedRecently + 1f).Min(instance.grazedRecentlyToAutoloot);
            if (grazeAutoLoot)
            {
                grazeDecayFreezeEndTime = ((grazeDecayFreezeEndTime + 0.5f).Max(Time.time + 0.5f)).Min(Time.time + instance.grazeDecayMaxFreezeTime);
            }
        }
        private static void SetMaxGrazeTimer()
        {
            grazeDecayFreezeEndTime = Time.time + instance.grazeDecayMaxFreezeTime;
            grazedRecently = instance.grazedRecentlyToAutoloot;
        }
    }
    #endregion
    #region Pickups
    public partial class PlayerScoring
    {
        [Header("Pickups")]
        [SerializeField] Transform ScoreItemPrefab;
        [SerializeField] float scorePullForce = 1.5f;
        [SerializeField] float pickupDistance = 4f;
        [SerializeField] float verticalSpawnForce = 4f;
        [Range(0f, 50f)]
        [SerializeField] float forceSpreadPercent = 20f;
        [SerializeField] LayerMask pickupsLayer;
        [SerializeField] AudioClipWrapper pickupSound;
        Collider2D[] pickupsIteration;
        HashSet<Transform> lootedPickups = new HashSet<Transform>();
        static HashSet<Transform> spawnedPickups = new();
        static Queue<Vector2> pickupQueue = new();
        static Bounds? nullableWorldBounds;
        static float minimumYPositionForAutoLoot;
        static float bottomOfScreen;
        static bool autoLoot => grazeAutoLoot || PlayerTopOfScreenAutoloot();
        private static bool PlayerTopOfScreenAutoloot()
        {
            bool state = PlayerUnit.Player.Center.y >= minimumYPositionForAutoLoot;
            if (state)
            {
                SetMaxGrazeTimer();
            }
            return state;
        }
        private static void OnSceneChange(Scene oldScene, Scene newScene)
        {
            nullableWorldBounds = null;
            minimumYPositionForAutoLoot = 1000f;
            InitializeAutoLootWorldBounds();
        }
        private static void InitializeAutoLootWorldBounds()
        {
            if (nullableWorldBounds == null)
            {
                Bounds b = DirectionSolver.GetPaddedBounds(0f);
                bottomOfScreen = b.min.y - 2f;
                Bounds autoLoot = DirectionSolver.TopOfScreenBounds(5f, 0f);
                nullableWorldBounds = b;
                minimumYPositionForAutoLoot = autoLoot.min.y;
            }
        }
        public static void SpawnPickup(Vector2 position)
        {
            InitializeAutoLootWorldBounds();
            if (nullableWorldBounds != null && ((Bounds)nullableWorldBounds).Contains(position))
            {
                pickupQueue.Enqueue(position);
            }
        }
        public static IEnumerator CO_RunPickupQueue(int amountPerIteration)
        {
            int i;
            Vector2 position;
            while (true)
            {
                if (instance == null)
                {
                    continue;
                }
                for (i = 0; i < amountPerIteration; i++)
                {
                    if (pickupQueue.Count <= 0)
                        continue;

                    position = pickupQueue.Dequeue();
                    Transform pickup = Instantiate(instance.ScoreItemPrefab, position + Random.insideUnitCircle, Quaternion.identity);
                    Destroy(pickup.gameObject, 10f);
                    if (pickup.GetComponent<Rigidbody2D>() is not null and Rigidbody2D rb)
                    {
                        rb.linearVelocity = new Vector2(0f, instance.verticalSpawnForce).Rotate2D(-5f).Rotate2D(5f.Spread(100f));
                    }
                    spawnedPickups.Add(pickup);
                    if (autoLoot)
                    {
                        instance.PickupTransform(pickup);
                    }
                }
                yield return null;
            }
        }
        private void PickupLoop()
        {
            if (!gameObject.activeInHierarchy)
                return;
            if (autoLoot)
            {
                foreach (var pickup in spawnedPickups)
                {
                    PickupTransform(pickup);
                }
                return;
            }

            pickupsIteration = Physics2D.OverlapCircleAll(transform.position, pickupDistance, pickupsLayer);
            if (pickupsIteration == null || pickupsIteration.Length <= 0)
                return;

            foreach (Collider2D col in pickupsIteration)
            {
                Pickup(col);
            }
        }
        private void Pickup(Collider2D col)
        {
            if (col.transform == null)
            {
                return;
            }
            if (!gameObject.activeInHierarchy)
                return;
            col.enabled = false;
            PickupTransform(col.transform);
        }
        private void PickupTransform(Transform t)
        {
            if (t == null)
            {
                return;
            }
            if (!gameObject.activeInHierarchy)
                return;
            if (lootedPickups.Contains(t))
                return;
            lootedPickups.Add(t);
            if (t.position.y < bottomOfScreen)
            {
                return;
            }
            StartCoroutine(CO_Pickup(t.root));
        }
        private IEnumerator CO_Pickup(Transform pickup)
        {
            float pullTime = 0f;
            Vector2 startPosition = pickup.position;
            Vector2 startScale = pickup.localScale;
            Vector2 targetScale = startScale * 0.2f;
            float force = scorePullForce.Spread(forceSpreadPercent);
            if (pickup.GetComponent<Rigidbody2D>() is not null and Rigidbody2D rb)
            {
                rb.gravityScale = 0f;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
            while (pullTime < 1f)
            {
                if (pickup == null)
                    yield break;
                pullTime += Time.deltaTime * force.Squared().Max(0.3f);
                pickup.position = Vector2.Lerp(startPosition, transform.position, pullTime);
                pickup.localScale = Vector2.Lerp(startScale, targetScale, ((pullTime - 0.4f) * 2f).Clamp(0f, 1f));
                yield return null;
            }
            Destroy(pickup.gameObject);
            lootedPickups.Remove(pickup);
            pickupSound.Play(transform.position);
            scoreItemAddedValue++;
            PlayerBombAction.AddBombValue(1);
            float score = AddScore(baseScorePerPickup);
            if (GeneralManager.ShouldAddScoreKey)
            {
                GeneralManager.AddScoreAnalysisKey("Pickups", score);
            }
        }
    }
    #endregion
    public partial class PlayerScoring : MonoBehaviour
    {
        [Header("Base Score")]
        private static PlayerScoring instance;
        float scoreMultiplier = 1f;
        const float grazeModifier = 0.0001f;
        static int grazeCount = 0;
        static int scoreItemAddedValue = 0;
        [SerializeField] float baseScorePerGraze = 1000f;
        [SerializeField] float baseScorePerPickup = 5000f;
        float nextpickupLoopTime;
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
        private void Update()
        {
            RunGrazeAutoLootLoop();
            if (Time.time >= nextpickupLoopTime)
            {
                PickupLoop();
                nextpickupLoopTime = Time.time + 0.1f;
            }
        }
        private void Awake()
        {
            lootedPickups.Clear();
            spawnedPickups.Clear();
            instance = this;
        }
        private void BulletClearSpawnPickup(Vector2 position)
        {
            if (Helper.SeededRandomInt256 < 150)
                return;
            SpawnPickup(position);
        }
        private void Start()
        {
            GrazeBox.OnGraze += GrazeAction;
            SceneManager.activeSceneChanged += OnSceneChange;
            InitializeAutoLootWorldBounds();
        }
        private void OnEnable()
        {
            StartCoroutine(CO_RunPickupQueue(35));
        }
        private void OnDestroy()
        {
            GrazeBox.OnGraze -= GrazeAction;
            SceneManager.activeSceneChanged -= OnSceneChange;
        }
        private void GrazeAction(int graze)
        {
            grazeCount = graze;
            float score = AddScore(baseScorePerGraze);
            if (GeneralManager.ShouldAddScoreKey)
            {
                GeneralManager.AddScoreAnalysisKey("Grazing", score);
            }
            AddGrazeAndRefreshAutoLoot();
        }
    }
}

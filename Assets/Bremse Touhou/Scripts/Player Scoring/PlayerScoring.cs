using Bremsengine;
using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BremseTouhou
{
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
        static bool autoLoot => PlayerUnit.Player.Center.y >= minimumYPositionForAutoLoot;
        private static void OnSceneChange(Scene oldScene, Scene newScene)
        {
            nullableWorldBounds = null;
            minimumYPositionForAutoLoot = 1000f;
        }
        public static void SpawnPickup(Vector2 position)
        {
            if (nullableWorldBounds == null)
            {
                Bounds b = DirectionSolver.GetPaddedBounds(0f);
                Bounds autoLoot = DirectionSolver.TopOfScreenBounds(5f, 0f);
                nullableWorldBounds = b;
                minimumYPositionForAutoLoot = autoLoot.min.y;
            }
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
                    Debug.LogWarning("Missing Player Scoring Instance");
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
            col.enabled = false;
            PickupTransform(col.transform);
        }
        private void PickupTransform(Transform t)
        {
            if (t == null)
            {
                return;
            }
            if (lootedPickups.Contains(t))
                return;
            lootedPickups.Add(t);
            StartCoroutine(CO_Pickup(t.root));
        }
        private IEnumerator CO_Pickup(Transform pickup)
        {
            float pullTime = 0f;
            Vector2 startPosition = pickup.position;
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
                yield return null;
            }
            Destroy(pickup.gameObject);
            lootedPickups.Remove(pickup);
            pickupSound.Play(transform.position);
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
        int grazeCount = 0;
        [SerializeField] float baseScorePerGraze = 1000f;
        [SerializeField] float baseScorePerPickup = 50000f;
        private float MultiplyScore(float score)
        {
            scoreMultiplier = (1f + (grazeCount * grazeModifier));
            return scoreMultiplier * score;
        }
        private float AddScore(float addedScore)
        {
            return GeneralManager.AddScore(MultiplyScore(addedScore));
        }
        private void FixedUpdate()
        {

        }
        private void Update()
        {

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
            InvokeRepeating(nameof(PickupLoop), 0.1f, 0.1f);
            StartCoroutine(CO_RunPickupQueue(150));
            SceneManager.activeSceneChanged += OnSceneChange;
        }
        private void OnDestroy()
        {
            GrazeBox.OnGraze -= GrazeAction;
            SceneManager.activeSceneChanged -= OnSceneChange;
        }
        private void GrazeAction(int grazeCount)
        {
            this.grazeCount = grazeCount;
            float score = AddScore(baseScorePerGraze);
            if (GeneralManager.ShouldAddScoreKey)
            {
                GeneralManager.AddScoreAnalysisKey("Grazing", score);
            }
        }
    }
}

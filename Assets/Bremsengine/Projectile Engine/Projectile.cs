using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bremsengine
{
    #region Set Projectile
    public partial class Projectile
    {
        [SerializeField] SpriteRenderer mainSprite;
        [SerializeField] BoxCollider2D mainCollider;
        public Projectile SetProjectile(Projectile proj)
        {
            mainSprite.transform.name = proj.transform.name;
            mainSprite.sprite = proj.mainSprite.sprite;
            mainSprite.transform.localScale = proj.mainSprite.transform.localScale;
            mainSprite.transform.localRotation = proj.mainSprite.transform.localRotation;
            transform.localScale = proj.transform.localScale;
            mainCollider.size = proj.mainCollider.size;
            mainSprite.color = proj.mainSprite.color;
            return this;
        }
    }
    #endregion
    #region Queue
    public partial class Projectile
    {
        public static Queue<Projectile> ProjectileQueue = new Queue<Projectile>();
        const int MaxQueueClearAttempts = 2500;
        public static Projectile CreateFromQueue(Projectile proj, Vector2 position, ProjectileDirection direction)
        {
            Projectile projectile = null;
            int iterations = 0;
            while (ProjectileQueue.Count > 0 && ProjectileQueue.Peek() == null)
            {
                if (iterations >= MaxQueueClearAttempts)
                {
                    Debug.LogError("Something bad happened");
                    ProjectileQueue.Clear();
                    break;
                }
                iterations++;
                ProjectileQueue.Dequeue();
            }

            if (ProjectileQueue.Count > 0)
            {
                projectile = ProjectileQueue.Dequeue();
                projectile.gameObject.SetActive(true);
            }
            else
            {
                projectile = Instantiate(proj, position, Quaternion.identity);
            }
            projectile.SetPosition(position).SetDirection(direction);
            projectile.SetProjectile(proj);
            return projectile;
        }
    }
    #endregion
    #region Direction
    public partial class Projectile
    {
        ProjectileDirection currentDirection;
        public Projectile SetPosition(Vector3 position)
        {
            transform.position = position;
            return this;
        }
        public Projectile SetDirection(ProjectileDirection direction)
        {
            currentDirection = direction.Clone();
            rb.velocity = direction.Vector;
            rotationAnchor.Lookat2D((Vector2)rotationAnchor.position + rb.velocity);
            return this;
        }
    }
    [System.Serializable]
    public struct ProjectileDirection
    {
        public ProjectileDirection Clone()
        {
            return new ProjectileDirection(projectile, direction).SetSpeedMod(speedMod);
        }
        public float Speed => SpeedRange.RandomBetweenXY();
        float speedMod;
        public Vector2 Direction => direction.Rotate2D(AngleOffset).Rotate2D(projectile.Spread);
        public Vector2 Vector => Direction.normalized * Speed * speedMod;
        public ProjectileDirection(ProjectileSO projectile, Vector2 direction)
        {
            this.projectile = projectile;
            this.SpeedRange = projectile.SpeedRange;
            this.direction = direction;
            this.AngleOffset = 0f;
            this.speedMod = 1f;
        }
        public ProjectileDirection SetSpeedMod(float speedModifier)
        {
            speedMod = speedModifier;
            return this;
        }
        public ProjectileDirection AddAngle(float angle)
        {
            AngleOffset += angle;
            return this;
        }
        ProjectileSO projectile;
        [SerializeField] public Vector2 SpeedRange;
        Vector2 direction;
        [SerializeField] public float AngleOffset;
    }
    #endregion
    #region Spawning
    public partial class Projectile
    {
        public delegate void SpawnCallback(Projectile p);
        public static Projectile SpawnProjectile(ProjectileSO proj, Vector2 position, ProjectileDirection direction, SpawnCallback callback)
        {
            if (proj.projectilePrefab == null)
            {
                return null;
            }

            Projectile spawnedProjectile = CreateFromQueue(proj.projectilePrefab, position, direction);
            AddToFolder(spawnedProjectile);

            spawnedProjectile.projectile = proj;
            callback?.Invoke(spawnedProjectile);
            return spawnedProjectile;

            //return CreateProjectile(proj.projectilePrefab, position, direction.AddAngle(proj.Spread));
        }
        /*
        public static Projectile SpawnProjectileTowardsUnit(ProjectileSO proj, Vector2 position, ProjectileDirection direction, Rigidbody2D target)
        {
            if (proj == null)
            {
                return null;
            }

            float travelTime = (target.position - position).TravelTime(direction.Speed);
            Vector2 targetPosition = (Vector2)target.PredictPosition(travelTime, 0.5f);
            Vector2 targetVector = targetPosition - position;
            ProjectileDirection newDirection = new(direction.SpeedRange, targetVector.normalized, direction.AngleOffset);

            return SpawnProjectile(proj, position, newDirection);
        }*/
    }
    #endregion
    #region OffScreen
    public partial class Projectile
    {
        bool isOffScreen;
        [SerializeField] float maxDistanceToCamera = 25f;
        protected bool RecalculateOffscreen => (transform.position.Z(0f).SquareDistanceToGreaterThan(Camera.main.transform.position.Z(0f), maxDistanceToCamera));
        public void ClearProjectile()
        {
            gameObject.SetActive(false);
            ProjectileQueue.Enqueue(this);
        }
    }
    #endregion
    #region Projectile Folder
    public partial class Projectile
    {
        private static GameObject projectileFolder = null;
        [ContextMenu("Clear All Projectiles Globally")]
        public void ClearAllProjectiles()
        {
            projectileFolder.transform.position = Vector2.zero;
            if (projectileFolder != null)
            {
                Destroy(projectileFolder);
            }
        }
        public static void AddToFolder(Projectile proj)
        {
            #region Validate
            if (projectileFolder == null || !projectileFolder.activeInHierarchy)
            {
                if (projectileFolder != null)
                {
                    Destroy(projectileFolder);
                }
                projectileFolder = new GameObject("Projectile Folder");
            }
            #endregion
            if (projectileFolder != null)
            {
                proj.transform.SetParent(projectileFolder.transform);
            }
        }
    }
    #endregion
    #region Hit Event
    public partial class Projectile
    {
        float projectileDamage = 10f;
        public Projectile SetDamage(float newDamage)
        {
            projectileDamage = newDamage;
            return this;
        }
        public float Damage => projectileDamage;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.GetComponent<Transform>() is Transform t && t != null)
            {
                if (t.TryGetComponent<IProjectileHitListener>(out IProjectileHitListener listener))
                {
                    if (listener.OnProjectileHit(this))
                    {
                        ClearProjectile();
                        return;
                    }
                }
                if (t.TryGetComponent<IFaction>(out IFaction faction))
                {
                    if (faction.Faction == Faction)
                    {
                        return;
                    }
                }
            }
            if (!collision.isTrigger)
            {
                ClearProjectile();
                return;
            }
        }
    }
    #endregion
    #region Faction
    public partial class Projectile
    {
        public BremseFaction Faction { get; protected set; }
        public Projectile SetFaction(BremseFaction f)
        {
            Faction = f;
            return this;
        }
    }
    #endregion
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public partial class Projectile : MonoBehaviour
    {
        ProjectileSO projectile;
        [SerializeField] Rigidbody2D rb;
        [SerializeField] Transform rotationAnchor;
        [SerializeField] float gravityModifier = 0f;
        protected delegate void OnUpdate(float deltaTime);
        protected OnUpdate UpdateEvent;
        protected OnUpdate FixedUpdateEvent;
        private void Update()
        {
            UpdateEvent?.Invoke(Time.deltaTime);
        }
        private void LateUpdate()
        {
            isOffScreen = RecalculateOffscreen;
            if (isOffScreen)
            {
                ClearProjectile();
            }
        }
        private void FixedUpdate()
        {
            FixedUpdateEvent?.Invoke(Time.fixedDeltaTime);
        }
        private void OnValidate()
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody2D>();
                rb.gravityScale = gravityModifier;
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                rb.interpolation = RigidbodyInterpolation2D.Interpolate;

                mainCollider = GetComponent<BoxCollider2D>();
                mainCollider.isTrigger = true;
            }
        }
    }
}
using Bremsengine;
using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bovine
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
        public Projectile SetPosition(Vector3 position)
        {
            transform.position = position;
            return this;
        }
        public Projectile SetDirection(ProjectileDirection direction)
        {
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
            return new ProjectileDirection(SpeedRange, direction, AngleOffset).AimTowardsTarget(aimTowardsTarget);
        }
        public float Speed => SpeedRange.RandomBetweenXY();
        public Vector2 Direction => direction.Rotate2D(AngleOffset);
        public Vector2 Vector => Direction.normalized * Speed;

        public ProjectileDirection AimTowardsTarget(bool state)
        {
            aimTowardsTarget = state;
            return this;
        }
        public ProjectileDirection(Vector2 speedRange, Vector2 direction, float angleOffset)
        {
            this.SpeedRange = speedRange;
            this.direction = direction;
            this.AngleOffset = angleOffset;
            aimTowardsTarget = false;
        }
        public ProjectileDirection AddAngle(float angle)
        {
            AngleOffset += angle;
            return this;
        }

        [SerializeField] public Vector2 SpeedRange;
        [SerializeField] Vector2 direction;
        [SerializeField] public float AngleOffset;
        public bool aimTowardsTarget;
    }
    #endregion
    #region Spawning
    public partial class Projectile
    {
        public static Projectile SpawnProjectile(ProjectileSO proj, Vector2 position, ProjectileDirection direction)
        {
            if (proj.projectilePrefab == null)
            {
                return null;
            }

            Projectile spawnedProjectile = CreateFromQueue(proj.projectilePrefab, position, direction.AddAngle(proj.Spread));
            AddToFolder(spawnedProjectile);

            return spawnedProjectile;
            
            //return CreateProjectile(proj.projectilePrefab, position, direction.AddAngle(proj.Spread));
        }
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
        }
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
            }
            if (!collision.isTrigger)
            {
                ClearProjectile();
                return;
            }
        }
    }
    #endregion
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public partial class Projectile : MonoBehaviour
    {
        [SerializeField] Rigidbody2D rb;
        [SerializeField] Transform rotationAnchor;
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
                rb.gravityScale = 0f;
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                rb.interpolation = RigidbodyInterpolation2D.Interpolate;

                mainCollider = GetComponent<BoxCollider2D>();
                mainCollider.isTrigger = true;
            }
        }
    }
}
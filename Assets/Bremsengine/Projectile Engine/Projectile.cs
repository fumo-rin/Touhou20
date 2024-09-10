using Bovine;
using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bremsengine
{
    #region Set Projectile
    public partial class Projectile
    {
        [SerializeField] ProjectileSprite projectileSprite;
        [SerializeField] Collider2D mainCollider;
        public Projectile SetProjectile(Projectile proj)
        {
            if (mainCollider != null)
            {
                Destroy(mainCollider);
                if (proj.mainCollider is BoxCollider2D box)
                {
                    mainCollider = gameObject.AddComponent<BoxCollider2D>();
                    ((BoxCollider2D)mainCollider).size = box.size;
                }
                if (proj.mainCollider is CircleCollider2D circle)
                {
                    mainCollider = gameObject.AddComponent<CircleCollider2D>();
                    ((CircleCollider2D)mainCollider).radius = circle.radius;
                }
                mainCollider.isTrigger = true;
            }
            isActive = true;
            transform.name = proj.transform.name;

            projectileSprite.SetSprite(proj.projectileSprite, proj.projectileSprite);
            projectileSprite.SetColor(proj.projectileSprite.Color);
            projectileSprite.transform.localScale = proj.projectileSprite.transform.localScale;
            projectileSprite.transform.localRotation = proj.projectileSprite.transform.localRotation;

            transform.localScale = proj.transform.localScale;

            rb.gravityScale = proj.gravityModifier;
            rb.drag = proj.drag;
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
                    Debug.LogWarning("Many Projectiles");
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
    #region Projectile Clearing
    public partial class Projectile
    {
        private bool isActive;
        public bool Active => isActive;
        public void ClearProjectile()
        {
            isActive = false;
            gameObject.SetActive(false);
            ProjectileQueue.Enqueue(this);
            StopAllCoroutines();
            activeProjectiles.Remove(this);
        }
    }
    #endregion
    #region Projectile Folder
    public partial class Projectile
    {
        static HashSet<Projectile> activeProjectiles = new();
        private static GameObject projectileFolder = null;
        [ContextMenu("Clear All Projectiles Globally")]
        public void ClearAllProjectiles()
        {
            projectileFolder.transform.position = Vector2.zero;
            if (projectileFolder != null)
            {
                Destroy(projectileFolder);
            }
            activeProjectiles.Clear();
        }
        public static Projectile[] ProjectilesWhere(System.Func<Projectile, bool> predicate) => activeProjectiles.Where(predicate).ToArray();
        public static void ClearProjectilesOfFaction(BremseFaction f)
        {
            foreach (var item in ProjectilesWhere(x => x.Faction == f))
            {
                item.ClearProjectile();
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
                activeProjectiles.Add(proj);
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
                if (t.TryGetComponent<IFaction>(out IFaction faction))
                {
                    if (faction.Faction == Faction)
                    {
                        return;
                    }
                }
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
    public partial class Projectile : MonoBehaviour
    {
        public ProjectileSO Data => projectile;
        ProjectileSO projectile;
        public Vector2 Position => transform.position;
        [SerializeField] Rigidbody2D rb;
        [SerializeField] Transform rotationAnchor;
        [SerializeField] float gravityModifier = 0f;
        [SerializeField] float drag = 0f;
        private void Awake()
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody2D>();
                rb.gravityScale = gravityModifier;
            }

            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

            if (GetComponent<Collider2D>() is not null and Collider2D c)
            {
                mainCollider = c;
                c.isTrigger = true;
            }
        }
    }
}
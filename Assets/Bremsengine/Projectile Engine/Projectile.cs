using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bremsengine
{
    #region Set Projectile
    public partial class Projectile
    {
        public Projectile SetTarget(Transform target)
        {
            this.target = target;
            return this;
        }
        public Projectile SetProjectile(Projectile proj)
        {
            isActive = true;

            mainCollider.size = proj.mainCollider.size;

            transform.name = proj.transform.name;

            projectileSprite.SetSprite(proj.projectileSprite, proj.projectileSprite);
            projectileSprite.SetColor(proj.projectileSprite.Color);
            projectileSprite.transform.localScale = proj.projectileSprite.transform.localScale;
            projectileSprite.transform.localRotation = proj.projectileSprite.transform.localRotation;

            transform.localScale = proj.transform.localScale;

            rb.gravityScale = proj.gravityModifier;
            rb.linearDamping = proj.drag;
            QueueRecalculateOffscreen();
            return this;
        }
        public Projectile SetIgnoreCollision(Collider2D c)
        {
            if (ignoredCollision != null)
            {
                Physics2D.IgnoreCollision(ignoredCollision, mainCollider, false);
            }
            if (c == null)
            {
                ignoredCollision = null;
                return this;
            }
            Physics2D.IgnoreCollision(mainCollider, c, true);
            ignoredCollision = c;
            return this;
        }
        public Projectile Post_AddVelocity(float v)
        {
            rb.linearVelocity += rb.linearVelocity.ScaleToMagnitude(v);
            return this;
        }
        public Projectile Post_AddRotation(float r)
        {
            rb.linearVelocity = rb.linearVelocity.Rotate2D(r);
            rb.rotation += r;

            return this;
        }
        Collider2D ignoredCollision;
    }
    #endregion
    #region Queue
    public partial class Projectile
    {
        public static Queue<Projectile> ProjectileQueue = new Queue<Projectile>();
        const int MaxQueueClearAttempts = 2500;
        public static Projectile OldCreateFromQueue(Projectile proj, Vector2 position, ProjectileDirection direction)
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
        public static Projectile NewCreateFromQueue(Projectile proj, Vector2 position, ProjectileNodeDirection direction)
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
            projectile.SetPosition(position);
            projectile.SetProjectile(proj);
            projectile.NewSetDirection(direction);

            return projectile;
        }
    }
    #endregion
    #region Direction
    public partial class Projectile
    {
        ProjectileDirection currentDirection;
        ProjectileNodeDirection currentNodeDirection;
        public Projectile SetPosition(Vector3 position)
        {
            transform.position = position;
            return this;
        }
        public Projectile SetDirection(ProjectileDirection direction)
        {
            currentDirection = direction.Clone();
            rb.linearVelocity = direction.Direction;
            transform.position += (Vector3)direction.DirectionalOffset;
            rotationAnchor.Lookat2D((Vector2)rotationAnchor.position + rb.linearVelocity);
            return this;
        }
        public Projectile NewSetDirection(ProjectileNodeDirection direction)
        {
            currentNodeDirection = direction.Clone();
            rb.linearVelocity = direction.Direction;
            transform.position += (Vector3)direction.DirectionalOffset;
            rotationAnchor.Lookat2D((Vector2)rotationAnchor.position + rb.linearVelocity);
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
        private Vector2 RotatedDirection => direction.Rotate2D(AngleOffset).Rotate2D(projectile.Spread);
        public Vector2 Direction => RotatedDirection.normalized * Speed * speedMod;
        public ProjectileDirection(ProjectileSO projectile, Vector2 direction)
        {
            this.projectile = projectile;
            this.SpeedRange = projectile.SpeedRange;
            this.direction = direction;
            this.AngleOffset = 0f;
            this.speedMod = 1f;
            this.directionalOffset = 0f;
        }
        public ProjectileDirection SetSpeedMod(float speedModifier)
        {
            speedMod = speedModifier;
            return this;
        }
        public ProjectileDirection SetDirectionalOffset(float offset)
        {
            directionalOffset = offset;
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
        float directionalOffset;
        [SerializeField] public float AngleOffset;
        public Vector2 DirectionalOffset => Direction.ScaleToMagnitude(directionalOffset);
    }
    #endregion
    #region Spawning
    public partial class Projectile
    {
        public delegate void SpawnCallback(Projectile p, Transform owner, Transform target);
        public static Projectile SpawnProjectile(ProjectileSO proj, Transform owner, Vector2 position, ProjectileDirection direction, SpawnCallback callback, Transform target)
        {
            if (proj.projectilePrefab == null)
            {
                return null;
            }
            CountProjectiles++;
            direction.SetDirectionalOffset(proj.DirectionalOffset);
            Projectile spawnedProjectile = OldCreateFromQueue(proj.projectilePrefab, position, direction);
            AddToFolder(spawnedProjectile);

            spawnedProjectile.projectile = proj;

            callback?.Invoke(spawnedProjectile, owner, target);
            return spawnedProjectile;

            //return CreateProjectile(proj.projectilePrefab, position, direction.AddAngle(proj.Spread));
        }
        public static void SpawnProjectileGraph(ProjectileGraphSO graph, ProjectileGraphInput input, SpawnCallback callback)
        {
            graph.SpawnGraph(input, callback);
        }
        public static void RegisterProjectile(Projectile spawned)
        {
            CountProjectiles++;
            AddToFolder(spawned);
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
        bool isOffscreenClearing;
        bool recalculateOffscreen;
        public void ClearProjectile()
        {
            if (gameObject == null)
            {
                activeProjectiles.Remove(this);
                return;
            }
            CountProjectiles--;
            isActive = false;
            gameObject.SetActive(false);
            ProjectileQueue.Enqueue(this);
            StopAllCoroutines();
            activeProjectiles.Remove(this);
        }
        public void QueueRecalculateOffscreen() => recalculateOffscreen = true;
        public void SetOffScreen(bool state)
        {
            recalculateOffscreen = false;
            if (state)
            {
                if (!isOffscreenClearing)
                {
                    Invoke(nameof(ClearProjectile), offScreenClearDelay);
                }
                isOffscreenClearing = state;
                return;
            }
            isOffscreenClearing = state;
            CancelInvoke(nameof(ClearProjectile));
        }
        public static void ClearProjectileTimelineFor(Transform t) => ProjectileEmitterTimelineHandler.ClearEmitQueue(t);
    }
    #endregion
    #region Projectile Folder
    public partial class Projectile
    {
        public static int CountProjectiles { get; private set; }
        static HashSet<Projectile> activeProjectiles = new();
        private static GameObject projectileFolder = null;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        [ContextMenu("Clear All Projectiles Globally")]
        public static void ClearAllProjectiles()
        {
            CountProjectiles = 0;
            if (projectileFolder != null)
            {
                Destroy(projectileFolder);
            }
            activeProjectiles.Clear();
        }
        public static Projectile[] ProjectilesWhere(System.Func<Projectile, bool> predicate) => activeProjectiles.Where(predicate).ToArray();
        public static void ClearProjectilesOfFaction(BremseFaction f)
        {
            foreach (var item in ProjectilesWhere(x => x != null && x.Faction == f))
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
            CollideWith(collision);
        }
        public void CollideWith(Collider2D collision)
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
        [SerializeField] ProjectileSprite projectileSprite;
        [SerializeField] CapsuleCollider2D mainCollider;
        [SerializeField] Transform target;
        public ProjectileSO Data => projectile;
        ProjectileSO projectile;
        public Vector2 Position => transform.position;
        public Vector2 Velocity => rb.linearVelocity;
        [SerializeField] Rigidbody2D rb;
        [SerializeField] Transform rotationAnchor;
        [SerializeField] float gravityModifier = 0f;
        [SerializeField] float drag = 0f;
        [SerializeField] float offScreenClearDelay = 5f;
        public Texture Texture => projectileSprite == null ? null : projectileSprite.Texture;
        public bool IsOffScreen => projectileSprite.IsOffScreen;
        private void Awake()
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody2D>();
                rb.gravityScale = gravityModifier;
            }

            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

            if (GetComponent<CapsuleCollider2D>() is not null and CapsuleCollider2D c)
            {
                mainCollider = c;
                c.isTrigger = true;
            }
        }
        private void LateUpdate()
        {
            if (recalculateOffscreen)
            {
                SetOffScreen(IsOffScreen);
            }
        }
        public bool Contains(Vector2 position) => mainCollider.bounds.Contains(position);
    }
}
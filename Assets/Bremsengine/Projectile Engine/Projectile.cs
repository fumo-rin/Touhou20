using Core.Extensions;
using Mono.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            mainCollider.enabled = proj.mainCollider.enabled;

            //transform.name = proj.transform.name;

            projectileSprite.SetSprite(proj.projectileSprite, proj.projectileSprite);
            projectileSprite.SetColor(proj.projectileSprite.Color);
            projectileSprite.transform.localScale = proj.projectileSprite.transform.localScale;
            projectileSprite.transform.localRotation = proj.projectileSprite.transform.localRotation;

            transform.localScale = proj.transform.localScale;

            rb.gravityScale = proj.gravityModifier;
            rb.linearDamping = proj.drag;
            ClearMods();
            return this;
        }
        public Projectile SetSpriteIndex(int index)
        {
            projectileSprite.SetSpriteSortingIndex(index);
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
        public Projectile Post_SetNewVelocity(Vector2 velocity)
        {
            rb.linearVelocity = velocity;
            return this;
        }
        public Projectile Post_AddVelocity(float v)
        {
            rb.linearVelocity += rb.linearVelocity.ScaleToMagnitude(v);
            return this;
        }
        public Projectile ApplyCurrentVelocity()
        {
            rb.linearVelocity = Velocity;
            LookAtVelocity();
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
    #region Sweeping
    public partial class Projectile
    {
        public delegate void BulletSweep(Vector2 position);
        public static BulletSweep OnSpawnSweep;
        private static Projectile SweepRunner;
        static List<Projectile> ToSweep = new();
        public static float SweepTimeEnd { get; private set; }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ReinitializeSweep()
        {
            SweepTimeEnd = 0f;
            ToSweep.Clear();
        }
        private static void RunSweep(Projectile p)
        {
            if (SweepRunner != null && SweepRunner.Active)
            {
                return;
            }
            SweepRunner = p;
            while (ToSweep.Count > 0)
            {
                if (ToSweep[0] == null || !ToSweep[0].Active)
                {
                    ToSweep.RemoveAt(0);
                    continue;
                }
                OnSpawnSweep?.Invoke(ToSweep[0].Position);
                ToSweep[0].ClearProjectile();
                ToSweep.RemoveAt(0);
                Debug.Log("T");
            }
        }
        public static void SetSpawnSweepTime(float duration)
        {
            SweepTimeEnd = Time.time + duration;
        }
        private static void SpawnSweep(Vector2 position)
        {
            OnSpawnSweep?.Invoke(position);
        }
    }
    #endregion
    #region Queue
    public partial class Projectile
    {
        public static int GetBulletID => GetBulletIDAndIncrementCount();
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ResetBulletID()
        {
            BulletIDCounter = 0;
        }
        private static int BulletIDCounter;
        private static int GetBulletIDAndIncrementCount()
        {
            return BulletIDCounter++;
        }
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
            projectile.projectileID = GetBulletID;
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
            projectile.projectileID = GetBulletID;
            if (Time.time < SweepTimeEnd)
            {
                ToSweep.Add(projectile);
            }
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
            rb.linearVelocity = direction.VelocityDirection;
            transform.position += (Vector3)direction.DirectionalOffset;
            rotationAnchor.Lookat2D((Vector2)rotationAnchor.position + rb.linearVelocity);
            BulletFlare.FlareAt(transform.position, direction.flareIndex);
            return this;
        }
        public Projectile LookAtVelocity()
        {
            rotationAnchor.Lookat2D((Vector2)rotationAnchor.position + rb.linearVelocity);
            return this;
        }
        public Projectile Flip()
        {
            NewSetDirection(currentNodeDirection.AddAngle(180f));
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
    [System.Serializable]
    public struct ClearProjectile
    {
        public float edgePaddingOutwards { get; private set; }
        public static void ForceClearCache()
        {
            if (BoundsCache == null)
                BoundsCache = new();
            BoundsCache.Clear();
        }
        static Dictionary<float, Bounds> BoundsCache;
        private void RecalculateBounds()
        {
            if (BoundsCache == null)
                BoundsCache = new Dictionary<float, Bounds>();

            if (!BoundsCache.ContainsKey(edgePaddingOutwards))
            {
                BoundsCache[edgePaddingOutwards] = DirectionSolver.GetPaddedBounds(-edgePaddingOutwards.Max(5f));
            }
        }
        public ClearProjectile(float edgePaddingOutwards)
        {
            this.edgePaddingOutwards = edgePaddingOutwards;
            RecalculateBounds();
        }
        public bool KeepProjectile(Vector2 position)
        {
            return BoundsCache[edgePaddingOutwards].Contains(position);
        }
    }
    public partial class Projectile
    {
        ClearProjectile offscreenClear;
        Coroutine offScreenCoroutine;
        static WaitForSeconds cachedStall;
        WaitForSeconds offScreenLoopStall => cachedStall ?? new(0.5f);
        public Projectile SetOffScreenClear(float edgePaddingOutwards)
        {
            offscreenClear = new(edgePaddingOutwards.Multiply(2f));
            if (offScreenCoroutine != null)
            {
                StopCoroutine(offScreenCoroutine);
            }
            offScreenCoroutine = StartCoroutine(CO_OffscreenLoop());
            return this;
        }
        IEnumerator CO_OffscreenLoop()
        {
            if (!Active)
            {
                offScreenCoroutine = null;
                yield break;
            }
            while (offscreenClear.KeepProjectile(this.Position))
            {
                if (cachedStall == null)
                    cachedStall = new(0.5f);

                yield return offScreenLoopStall;
                continue;
            }
            ClearProjectile();
        }
        private bool isActive;
        public bool Active => isActive;
        public delegate void ClearAction(Vector2 position);
        public ClearAction OnClear;
        public void ClearProjectile(ClearAction c = null)
        {
            if (gameObject == null)
            {
                activeProjectiles.Remove(this);
                return;
            }
            c?.Invoke(Position);
            CountProjectiles--;
            isActive = false;
            gameObject.SetActive(false);
            ProjectileQueue.Enqueue(this);
            StopAllCoroutines();
            activeProjectiles.Remove(this);
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
        public static void ClearProjectilesOfFaction(BremseFaction f, ClearAction onClear = null)
        {
            foreach (var item in ProjectilesWhere(x => x != null && x.Faction == f))
            {
                item.ClearProjectile(onClear);
            }
        }
        public static void ClearProjectilesNotOfFaction(BremseFaction f, ClearAction onClear = null)
        {
            foreach (var item in ProjectilesWhere(x => x != null && x.Faction != f))
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
        [SerializeField] float projectileDamage = 10f;
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
                    }
                    return;
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
    #region Player Bomb
    public partial class Projectile
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            lastBombTime = -1f;
            playerIframesInSeconds = 2f;
        }
        static float lastBombTime = -1f;
        static float playerIframesInSeconds;
        public static void PlayerTriggerBomb(float bombLength, float explosionDelay, AudioClipWrapper bombSound, AudioClipWrapper bombExplosion)
        {
            lastBombTime = Time.time;
            GeneralManager.Instance.StartCoroutine(CO_BombClear(explosionDelay, bombSound, bombExplosion));
        }
        static IEnumerator CO_BombClear(float delay, AudioClipWrapper bombSound, AudioClipWrapper bombExplosion)
        {
            bombSound.Play(DirectionSolver.GetPaddedBounds(0f).center);
            yield return new WaitForSeconds(delay);

            playerIframesInSeconds = 1.5f;
            bombExplosion.Play(DirectionSolver.GetPaddedBounds(0f).center);
            BombClearProjectiles(BremseFaction.Player);
        }
        private static void BombClearProjectiles(BremseFaction ownerFaction)
        {
            ClearProjectilesNotOfFaction(ownerFaction);
        }
        public static bool PlayerBombedRecently => lastBombTime > 0f && Time.time <= lastBombTime + playerIframesInSeconds;
    }
    #endregion
    #region Projectile Mods
    public partial class Projectile
    {
        Dictionary<ProjectileMod, float> modsDurations;
        HashSet<ProjectileMod> mods = new HashSet<ProjectileMod>();
        public void AddMod(ProjectileMod mod)
        {
            mods.Add(mod);
            modsDurations[mod] = mod.duration;
        }
        public void RemoveMod(ProjectileMod mod)
        {
            mods.Remove(mod);
            modsDurations.Remove(mod);
        }
        public void ClearMods()
        {
            if (mods != null && mods.Count > 0)
            {
                mods.Clear();
            }
            if (modsDurations == null)
            {
                modsDurations = new Dictionary<ProjectileMod, float>();
            }
            else
            {
                modsDurations.Clear();
            }
        }
        public void RunMods()
        {
            foreach (var item in mods)
            {
                if (!modsDurations.ContainsKey(item))
                {
                    Debug.Log("Bad");
                    continue;
                }
                if (modsDurations[item] <= 0f)
                {
                    continue;
                }
                item.RunMod(this, modsDurations[item], out float newDuration);
                modsDurations[item] = newDuration;
            }
        }
    }
    #endregion
    [RequireComponent(typeof(Rigidbody2D))]
    public partial class Projectile : MonoBehaviour
    {
        public int projectileID;
        [SerializeField] ProjectileSprite projectileSprite;
        [SerializeField] CapsuleCollider2D mainCollider;
        [SerializeField] Transform target;
        public Transform Target => target;
        public ProjectileSO Data => projectile;
        ProjectileSO projectile;
        public Vector2 Position => transform.position;
        public Vector2 Velocity => rb.linearVelocity;
        [SerializeField] Rigidbody2D rb;
        [SerializeField] Transform rotationAnchor;
        [SerializeField] float gravityModifier = 0f;
        [SerializeField] float drag = 0f;
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
        private void Update()
        {
            RunSweep(this);
            RunMods();
        }
        public bool Contains(Vector2 position) => mainCollider.bounds.Contains(position);
    }
}
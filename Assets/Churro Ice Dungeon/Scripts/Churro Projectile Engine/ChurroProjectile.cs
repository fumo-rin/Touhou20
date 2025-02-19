using Bremsengine;
using Core.Extensions;
using Mono.CSharp;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace ChurroIceDungeon
{
    #region Attack Types
    public partial class ChurroProjectile
    {
        public struct InputSettings
        {
            public InputSettings(Vector2 origin, Vector2 direction)
            {
                this.Origin = origin;
                this.Direction = direction;
                this.OnSpawn = null;
            }
            public Vector2 Origin { get; private set; }
            public Vector2 Direction { get; private set; }
            public ProjectileSpawnAction OnSpawn;
        }
        public struct SingleSettings
        {
            public SingleSettings(float addedAngle, float projectileSpeed)
            {
                this.AddedAngle = addedAngle;
                this.ProjectileSpeed = projectileSpeed;
            }
            public float AddedAngle;
            public float ProjectileSpeed;
        }
        public struct ArcSettings
        {
            public ArcSettings(float startingAngle, float arcAngle, float arcInterval, float projectileSpeed)
            {
                this.StartingAngle = startingAngle;
                this.ArcAngle = arcAngle;
                this.ArcInterval = arcInterval;
                this.ProjectileSpeed = projectileSpeed;
            }
            public float StartingAngle;
            public float ArcAngle;
            public float ArcInterval;
            public float ProjectileSpeed;
        }
        public static ChurroProjectile SpawnSingle(ChurroProjectile prefab, InputSettings input, SingleSettings settings)
        {
            return CreateBullet(prefab, input.Origin, input.Direction.normalized.Rotate2D(settings.AddedAngle), input.OnSpawn, settings.ProjectileSpeed);
        }
        public static List<ChurroProjectile> SpawnArc(ChurroProjectile prefab, InputSettings input, ArcSettings settings)
        {
            List<ChurroProjectile> output = new();
            foreach (var item in settings.ArcInterval.StepFromTo(settings.StartingAngle, settings.ArcAngle))
            {
                ChurroProjectile p = CreateBullet(prefab, input.Origin, input.Direction.Rotate2D(item), input.OnSpawn, settings.ProjectileSpeed);
                if (p == null)
                    continue;
                output.Add(p);
            }
            return output;
        }
    }
    #endregion
    #region Projectile Actions
    public partial class ChurroProjectile
    {
        public ChurroProjectile Action_MatchOther(ChurroProjectile other)
        {
            Action_SetSprite(other.projectileSprite.sprite);
            offScreenClearDistance = other.offScreenClearDistance;

            return this;
        }
        public ChurroProjectile Action_SetVelocity(Vector2 direction, float speed)
        {
            CurrentVelocity = direction.ScaleToMagnitude(speed);
            return this;
        }
        public ChurroProjectile Action_FacePosition(Vector2 worldPosition)
        {
            projectileSprite.transform.Lookat2D(worldPosition);
            return this;
        }
        public ChurroProjectile Action_SetSprite(Sprite s)
        {
            projectileSprite.sprite = s; return this;
        }
        public ChurroProjectile Action_SetSpriteLayerIndex(int index)
        {
            projectileSprite.sortingOrder = index; return this;
        }
        public ChurroProjectile Action_SetFaction(BremseFaction f)
        {
            this.Faction = f; return this;
        }
        public ChurroProjectile Action_SetDamage(float newDamage)
        {
            this.ContainedDamage = newDamage; return this;
        }
        public ChurroProjectile Action_SetOwnerReference(DungeonUnit owner)
        {
            this.Owner = owner; return this;
        }
    }
    #endregion
    #region Spawning
    public partial class ChurroProjectile
    {
        public delegate void ProjectileSpawnAction(ChurroProjectile p);
        private static ChurroProjectile CreateBullet(ChurroProjectile prefab, Vector2 position, Vector2 direction, ProjectileSpawnAction spawnAction, float speed)
        {
            ChurroProjectile p = Instantiate(prefab, position, Quaternion.identity);
            p.Action_FacePosition(position + direction);
            p.Action_SetVelocity(direction, speed);
            p.Action_MatchOther(prefab);
            spawnAction?.Invoke(p);
            return p;
        }
    }
    #endregion
    #region Offscreen Tracking
    [System.Serializable]
    public partial class ChurroProjectileOffscreen
    {
        public static ChurroProjectile offscreenRunner;
        public static List<ChurroProjectile> trackedOffscreenProjectiles;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Reinitialize()
        {
            offscreenRunner = null;
            trackedOffscreenProjectiles = new();
        }
        public static void SetRunnerIfNoneExists(ChurroProjectile p)
        {
            if (offscreenRunner == null || !offscreenRunner.gameObject.activeInHierarchy)
                offscreenRunner = p;
        }
        public static void RunOffscreen()
        {
            Bounds cam;
            ChurroProjectile iteration;
            for (int i = 0; i < trackedOffscreenProjectiles.Count; i++)
            {
                iteration = trackedOffscreenProjectiles[i];
                if (iteration == null)
                {
                    trackedOffscreenProjectiles.RemoveAt(i);
                    i--;
                    continue;
                }
                cam = CameraBoundsPadOutwards(iteration.offScreenClearDistance);

                if (cam.Contains(iteration.CurrentPosition))
                    continue;

                trackedOffscreenProjectiles.RemoveAt(i);
                iteration.ClearProjectile();
                i--;
                continue;
            }
        }
        public static Bounds CameraBoundsPadOutwards(float addedPadding = 0f)
        {
            static Bounds CameraBounds(float addedPadding)
            {
                static Vector2 CalculateSize()
                {
                    Vector2 size = new(10f, 10f);
                    if (Camera.main is not null and Camera c)
                    {
                        float height = (c.orthographicSize * 2f);
                        size = new(height * c.aspect, height);
                    }
                    else
                    {
                        Debug.Log("Failed to find camera main to calculate stage world size");
                    }
                    return size;
                }
                Bounds output = new Bounds(Vector2.zero, CalculateSize() + new Vector2(addedPadding * 2f, addedPadding * 2f));
                if (Camera.main is not null and Camera c)
                {
                    output.center = c.transform.position.Z(0f);
                }
                return output;
            }
            return CameraBounds(addedPadding);
        }
    }
    public partial class ChurroProjectile
    {
        private void OnBecameVisible()
        {
            ChurroProjectileOffscreen.trackedOffscreenProjectiles.Remove(this);
        }
        private void OnBecameInvisible()
        {
            ChurroProjectileOffscreen.trackedOffscreenProjectiles.AddIfDoesntExist(this);
        }
    }
    #endregion
    #region Pooling (for now no pooling lemao)
    public partial class ChurroProjectile
    {
        public void ClearProjectile()
        {
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
    }
    #endregion
    #region Collide
    public partial class ChurroProjectile
    {
        public enum CollisionResult
        {
            Error,
            Friends,
            DefaultObject,
            Hit
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            CollisionResult result = GetCollisionResult(collision);
            switch (result)
            {
                case CollisionResult.Error:
                    break;
                case CollisionResult.Friends:
                    break;
                case CollisionResult.DefaultObject:
                    ClearProjectile();
                    break;
                case CollisionResult.Hit:
                    if (collision.GetComponent<TargetBox>() is TargetBox box and not null)
                    {
                        box.SendDamageEvent(ContainedDamage, CurrentPosition);
                        if (Owner != null && box.transform.root.GetComponent<EnemyUnit>() is EnemyUnit enemy and not null)
                        {
                            enemy.Alert(Owner);
                        }
                        ClearProjectile();
                        return;
                    }
                    if (collision.GetComponent<IDamageable>() is IDamageable d and not null)
                    {
                        d.Hurt(ContainedDamage, CurrentPosition);
                        ClearProjectile();
                        return;
                    }
                    break;
                default:
                    break;
            }
        }
        public CollisionResult GetCollisionResult(Collider2D other)
        {
            if (other == null)
            {
                Debug.LogError("Bad");
                return CollisionResult.Error;
            }
            if (other.GetComponent<IFaction>() is IFaction hitListener and not null)
            {
                if (hitListener.IsFriendsWith(Faction))
                {
                    return CollisionResult.Friends;
                }
                else
                {
                    return CollisionResult.Hit;
                }
            }
            return CollisionResult.DefaultObject;
        }
    }
    #endregion
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    public partial class ChurroProjectile : MonoBehaviour
    {
        public Vector2 CurrentVelocity { get; private set; }
        public SpriteRenderer projectileSprite;
        [field: SerializeField] public float offScreenClearDistance;
        [SerializeField] Rigidbody2D projectileRB;
        public DungeonUnit Owner { get; private set; }
        public float ContainedDamage { get; private set; } = 1f;
        public Vector2 CurrentPosition => transform.position;
        public BremseFaction Faction { get; private set; } = BremseFaction.Enemy;
        public Rigidbody2D RB => projectileRB;
        public bool IsOffscreenRunner => ChurroProjectileOffscreen.offscreenRunner == this;
        private void Update()
        {
            ChurroProjectileOffscreen.SetRunnerIfNoneExists(this);
        }
        private void LateUpdate()
        {
            PerformVelocity(CurrentVelocity);
            if (IsOffscreenRunner)
            {
                ChurroProjectileOffscreen.RunOffscreen();
            }
        }
        public ChurroProjectile PerformVelocity(Vector2 velocity)
        {
            RB.linearVelocity = velocity;
            return this;
        }
    }
}

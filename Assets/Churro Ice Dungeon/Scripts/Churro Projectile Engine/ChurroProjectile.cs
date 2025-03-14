using Bremsengine;
using Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

namespace ChurroIceDungeon
{
    #region Projectile Events
    public partial class ChurroProjectile
    {
        List<ChurroProjectileEvent> knownEvents;
        public ChurroProjectile AddEvent(ChurroProjectileEvent e)
        {
            knownEvents.Add(e);
            return this;
        }
        public ChurroProjectile RemoveEvent(ChurroProjectileEvent e)
        {
            knownEvents.Remove(e);
            return this;
        }
        private void RunEvents(float deltaTime)
        {
            for (int i = 0; i < knownEvents.Count; i++)
            {
                if (knownEvents[i] == null)
                {
                    knownEvents.RemoveAt(i);
                    i--;
                }
                knownEvents[i].TickEvent(this, deltaTime);
            }
        }
    }
    #endregion
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
            public InputSettings SetOrigin(Vector2 position)
            {
                Origin = position;
                return this;
            }
            public InputSettings SetDirection(Vector2 direction)
            {
                Direction = direction;
                return this;
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
            public static ArcSettings operator *(ArcSettings settings, float multiplier)
            {
                return new ArcSettings(
                    settings.StartingAngle,
                    settings.EndingAngle,
                    settings.ArcInterval / multiplier,
                    settings.ProjectileSpeed
                );
            }
            public ArcSettings Widen(float multiplier)
            {
                return new ArcSettings(
                    this.StartingAngle * multiplier,
                    this.EndingAngle * multiplier,
                    this.ArcInterval * multiplier,
                    this.ProjectileSpeed
                    );
            }
            public ArcSettings Speed(float multiplier)
            {
                return new ArcSettings(
                   this.StartingAngle,
                   this.EndingAngle,
                   this.ArcInterval,
                   this.ProjectileSpeed * multiplier);
            }
            public ArcSettings(float startingAngle, float arcAngle, float arcInterval, float projectileSpeed)
            {
                this.StartingAngle = startingAngle;
                this.EndingAngle = arcAngle;
                this.ArcInterval = arcInterval;
                this.ProjectileSpeed = projectileSpeed;
            }
            public bool Spawn(ChurroProjectile.InputSettings input, ChurroProjectile prefab, out List<ChurroProjectile> output)
            {
                return SpawnArc(prefab, input, this, out output);
            }
            public float StartingAngle { get; private set; }
            public float EndingAngle { get; private set; }
            public float ArcInterval { get; private set; }
            public float ProjectileSpeed { get; private set; }
        }
        public static bool SpawnSingle(ChurroProjectile prefab, InputSettings input, SingleSettings settings, out ChurroProjectile p)
        {
            bool spawnedBullet = CreateBullet(prefab, input.Origin, input.Direction.normalized.Rotate2D(settings.AddedAngle), input.OnSpawn, settings.ProjectileSpeed, out p);
            if (p != null)
            {
                p.Action_AddPosition(p.CurrentVelocity.ScaleToMagnitude(0.25f));
            }
            return spawnedBullet;
        }
        public static bool SpawnArc(ChurroProjectile prefab, InputSettings input, ArcSettings settings, out List<ChurroProjectile> output)
        {
            output = new();
            foreach (var item in settings.ArcInterval.StepFromTo(settings.StartingAngle, settings.EndingAngle))
            {
                if (!CreateBullet(prefab, input.Origin, input.Direction.Rotate2D(item), input.OnSpawn, settings.ProjectileSpeed, out ChurroProjectile p))
                {
                    continue;
                }
                output.Add(p);
                p.Action_AddPosition(p.CurrentVelocity.ScaleToMagnitude(0.15f));
            }
            return output != null && output.Count > 0;
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
            ContainedDamage = other.ContainedDamage;

            return this;
        }
        public ChurroProjectile Action_MultiplyVelocity(float multiplier)
        {
            CurrentVelocity *= multiplier;
            return this;
        }
        public ChurroProjectile Action_SetVelocity(Vector2 direction, float speed)
        {
            CurrentVelocity = direction.ScaleToMagnitude(speed);
            if (direction != Vector2.zero)
            {
                projectileSprite.transform.Lookat2D(CurrentPosition + CurrentVelocity);
            }
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
            if (Owner != null)
            {
                newDamage = Owner.DamageScale(newDamage);
            }
            this.ContainedDamage = newDamage; return this;
        }
        public ChurroProjectile Action_SetOwnerReference(DungeonUnit owner)
        {
            this.Owner = owner; return this;
        }
        public ChurroProjectile Action_AddPosition(Vector2 direction)
        {
            transform.position = (Vector2)transform.position + direction;
            return this;
        }
        public ChurroProjectile Action_NewPosition(Vector2 position)
        {
            transform.position = position;
            return this;
        }
        public ChurroProjectile Action_AddRotation(float value)
        {
            return Action_SetVelocity(CurrentVelocity.Rotate2D(value), CurrentVelocity.magnitude);
        }
        public ChurroProjectile Action_SetBounceLives(int value)
        {
            TerrainBounceLives = value; return this;
        }
        public ChurroProjectile Action_ClearDistance(float distance)
        {
            offScreenClearDistance = distance; return this;
        }
        public ChurroProjectile Action_Split(float arcAngle, float arcRotation, int splitCount, bool destroy = true)
        {
            float iterationSplitRotation = 0f;
            for (int i = 0; i < splitCount; i++)
            {
                iterationSplitRotation = splitCount > 1 ? (-0.5f * arcAngle + ((arcAngle / splitCount) * i)) : 0f;
                Instantiate(this, CurrentPosition, transform.rotation).Action_SetVelocity(CurrentVelocity, CurrentVelocity.magnitude).Action_AddRotation(iterationSplitRotation).Action_MatchOther(this);
            }
            if (destroy)
            {
                ClearProjectile();
                return null;
            }
            return this;
        }
        public struct crawlerPacket
        {
            public float delay;
            public float aimAngle;
            public float repeatAngle;
            public int repeatCount;
            public float repeatTimeInterval;
            public Action<ChurroProjectile> OnSpawn;
            public crawlerPacket(float delay, float aimAngle, float repeatAngle, int repeatCount, float repeatTimeInterval)
            {
                this.delay = delay;
                this.aimAngle = aimAngle;
                this.repeatAngle = repeatAngle;
                this.repeatCount = repeatCount;
                this.repeatTimeInterval = repeatTimeInterval;
                OnSpawn = null;
            }
            public crawlerPacket AttachOnSpawnEvent(Action<ChurroProjectile> a)
            {
                OnSpawn += a;
                return this;
            }
        }
        public ChurroProjectile Action_AttachCrawlerEvent(ChurroProjectile crawlerPrefab, ArcSettings arc, crawlerPacket crawlerData)
        {
            ChurroEventCrawler crawlerEvent = new ChurroEventCrawler(new(1f, crawlerData.delay), new(crawlerPrefab, arc, crawlerData.aimAngle));
            crawlerEvent.AttachOnSpawnEvent(crawlerData.OnSpawn);
            crawlerEvent.SetRepeats(crawlerData.repeatCount, crawlerData.repeatTimeInterval, crawlerData.repeatAngle);
            this.AddEvent(crawlerEvent);
            return this;
        }
    }
    #endregion
    #region Spawning
    public partial class ChurroProjectile
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeSpawnID()
        {
            NextSpawnID = 0;
        }
        public int SpawnID { get; private set; }
        static int NextSpawnID;
        public delegate void ProjectileSpawnAction(ChurroProjectile p);
        private static bool CreateBullet(ChurroProjectile prefab, Vector2 position, Vector2 direction, ProjectileSpawnAction spawnAction, float speed, out ChurroProjectile spawnedBullet)
        {
            spawnedBullet = null;
            if (prefab == null)
                return spawnedBullet != null;
            if (IsSweeping && prefab.sweepable)
            {
                return false;
            }
            ChurroProjectile p = null;
            if (TryGetFromPool(prefab.poolID, out p) && p != null)
            {
                p.Action_NewPosition(position);
                p.gameObject.SetActive(true);
            }
            else
            {
                if (p == null)
                {
                    //something bad destroy the queues lemao
                    DestroyPool(prefab.poolID);
                }
                p = Instantiate(prefab, position, Quaternion.identity);
            }
            p.SpawnID = NextSpawnID++;
            p.knownEvents = new();
            p.Action_FacePosition(position + direction);
            p.Action_SetVelocity(direction, speed);
            p.Action_MatchOther(prefab);
            spawnAction?.Invoke(p);
            spawnedBullet = p;
            activeBullets.Add(p);
            p.OnScreenExit = null;
            return spawnedBullet != null;
        }
    }
    #endregion
    #region Sweeping
    public partial class ChurroProjectile
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void ReinitializeSweep()
        {
            SweepEndTime = 0;
            sweepList = new();
        }
        static float SweepEndTime;
        public static bool IsSweeping => Time.time < SweepEndTime;
        static byte sweepLootWeight;
        public static bool SweepLoot => sweepLootWeight > Helper.SeededRandomInt256;
        [field: SerializeField] public bool sweepable { get; private set; } = true;
        [QFSW.QC.Command("-sweep")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void CommandSweep()
        {
            SweepBullets(0.5f, 15);
        }
        static List<ChurroProjectile> sweepList;
        public static void SweepBullets(float sweepDuration, byte lootWeight)
        {
            sweepLootWeight = lootWeight;
            SweepEndTime = Time.time + sweepDuration;
            sweepList.Clear();
            foreach (var item in activeBullets)
            {
                if (item != null && item.sweepable)
                {
                    sweepList.Add(item);
                    if (SweepLoot)
                    {
                        WakaScoring.SpawnPickup(item.CurrentPosition);
                    }
                }
            }
            for (int i = 0; i < sweepList.Count; i++)
            {
                sweepList[i].ClearProjectile();
                sweepList.RemoveAt(i);
                i--;
                continue;
            }
        }
    }
    #endregion
    #region Pooling
    #region Editor
#if UNITY_EDITOR

    [CustomEditor(typeof(ChurroProjectile), true)]
    public class ChurroProjectileEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();  // Draw the default inspector for ChurroBaseAttack.

            // Add a button in the Inspector
            if (GUILayout.Button("Get New Pool ID"))
            {
                // Get the ChurroBaseAttack component
                ChurroProjectile pTarget = (ChurroProjectile)target;
                pTarget.poolID = System.Guid.NewGuid().ToString().GetHashCode();
                pTarget.Dirty();
                AssetDatabase.SaveAssets();

                // Log to confirm the ID was added
                Debug.Log($"Added PoolingID to {pTarget.gameObject.name} with ID: {pTarget.poolID}");
            }
        }
    }
#endif
    #endregion
    public partial class ChurroProjectile
    {
        [SerializeField] bool pooling = true;
        static Dictionary<int, Queue<ChurroProjectile>> pools;
        public int poolID;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ReinitializeProjectilePool()
        {
            pools = new();
            activeBullets = new();
            GeneralManager.SetStageLoadAction("Initialize Projectile Pool", ForceResetProjectileSystem);
        }
        private static bool TryGetQueueFor(int poolID, out Queue<ChurroProjectile> queue)
        {
            queue = null;
            if (!pools.ContainsKey(poolID))
            {
                pools[poolID] = new();
            }
            if (pools.ContainsKey(poolID) && pools[poolID] != null)
            {
                queue = pools[poolID];
            }
            return queue != null;
        }
        static void DestroyPool(int poolID)
        {
            if (TryGetQueueFor(poolID, out Queue<ChurroProjectile> queue))
            {
                foreach (var item in queue)
                {
                    if (item != null && item.gameObject != null)
                    {
                        Destroy(item.gameObject);
                    }
                }
                queue.Clear();
            }
        }
        static bool TryGetFromPool(int poolID, out ChurroProjectile p)
        {
            p = null;
            if (pools.ContainsKey(poolID) && pools[poolID] != null && pools[poolID].Count > 0)
            {
                p = pools[poolID].Dequeue();
                return true;
            }
            return false;
        }
        public bool ClearProjectile(int bounceCost = 1)
        {
            void Local_Destroy()
            {
                Destroy(gameObject);
                activeBullets.Remove(this);
            }
            void TryPool()
            {
                if (TryGetQueueFor(poolID, out Queue<ChurroProjectile> queue))
                {
                    queue.Enqueue(this);
                    gameObject.SetActive(false);
                }
                else
                {
                    Local_Destroy();
                }
                activeBullets.Remove(this);
            }
            TerrainBounceLives -= bounceCost.Abs();
            GrazeBox.Unregister(SpawnID);
            if (TerrainBounceLives <= 0)
            {
                if (!pooling)
                {
                    Local_Destroy();
                    return true;
                }
                TryPool();
                return true;
            }
            return false;
        }
    }
    #endregion
    #region Offscreen
    public partial class ChurroProjectile
    {
        static TagHandle stageboxTag => TagHandle.GetExistingTag("Projectile Stagebox");
        public delegate void ScreenExitAction(ChurroProjectile p);
        private ScreenExitAction OnScreenExit;
        public void AddOnScreenExitEvent(ScreenExitAction action)
        {
            OnScreenExit += action;
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag(stageboxTag))
            {
                OnScreenExit?.Invoke(this);
                OnScreenExit = null;
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
                    if (!collision.CompareTag(stageboxTag))
                    {
                        ClearProjectile();
                    }
                    break;
                case CollisionResult.Hit:
                    if (collision.GetComponent<TargetBox>() is TargetBox box and not null)
                    {
                        box.SendDamageEvent(ContainedDamage, CurrentPosition);
                        if (Owner != null && box.transform.root.GetComponent<EnemyUnit>() is EnemyUnit enemy and not null)
                        {
                            enemy.Alert(Owner);
                            ClearProjectile();
                            return;
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
                if (hitListener.Faction != BremseFaction.None && hitListener.IsFriendsWith(Faction))
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
        static HashSet<ChurroProjectile> activeBullets;
        public static int BulletCount => activeBullets == null ? 0 : activeBullets.Count;
        public static void ForceResetProjectileSystem()
        {
            ReinitializeProjectilePool();
            activeBullets = new();
        }
        float nextEventTickTime;
        float lastTickTime;
        float tickTimeLength = 0.05f;
        [field: SerializeField] public Collider2D ProjectileCollider { get; private set; }
        public int TerrainBounceLives { get; private set; } = 0;
        public Vector2 CurrentVelocity { get; private set; }
        public SpriteRenderer projectileSprite;
        [field: SerializeField] public float offScreenClearDistance;
        [SerializeField] Rigidbody2D projectileRB;
        public DungeonUnit Owner { get; private set; }
        public float ContainedDamage { get; private set; } = 1f;
        public Vector2 CurrentPosition => transform.position;
        public BremseFaction Faction { get; private set; } = BremseFaction.Enemy;
        public Rigidbody2D RB => projectileRB;
        public static void RunActiveBullets()
        {
            foreach (var item in activeBullets)
            {
                item.RunProjectile();
            }
        }
        public static void LateRunActiveBullets(float velocityScale)
        {
            foreach (var item in activeBullets)
            {
                item.PerformVelocity(velocityScale);
            }
        }
        public void RunProjectile()
        {
            if (Time.time > nextEventTickTime)
            {
                float tickDuration = Time.time - lastTickTime;
                lastTickTime = Time.time;
                nextEventTickTime = Time.time + tickTimeLength;
                RunEvents(tickDuration);
            }
        }
        public void PerformVelocity(float velocityScale)
        {
            PerformVelocity(CurrentVelocity * velocityScale);
        }
        public ChurroProjectile PerformVelocity(Vector2 velocity)
        {
            if (RB == null)
            {
                activeBullets.Remove(this);
                if (gameObject != null)
                {
                    Destroy(gameObject);
                }
            }
            RB.linearVelocity = velocity;
            return this;
        }
    }
}

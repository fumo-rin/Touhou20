using Core.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace ChurroIceDungeon
{
    #region Demo Damageable
    public interface IDemoDamageable
    {
        public void Hurt(float damage, Vector2 damagePosition, out GameObject hitObject);
    }
    public class Barrel : MonoBehaviour
    {
        [SerializeField] List<Hitbox> hitboxes;
        private void Start()
        {
            foreach (var hitbox in hitboxes)
            {
                hitbox.BindListener(DamageListen);
            }
        }
        private void OnDestroy()
        {
            foreach(var hitbox in hitboxes)
            {
                hitbox.ReleaseListener(DamageListen);
            }
        }
        float health = 100f;
        public bool Alive => health > 0f && gameObject.activeInHierarchy;
        private void DamageListen(float damage, Vector2 damagePosition, out GameObject hitObject)
        {
            if (!Alive)
                return;
            hitObject = gameObject;
        }
    }
    public class Hitbox : MonoBehaviour, IDemoDamageable
    {
        public delegate void HurtEvent(float damage, Vector2 damagePosition);
        public event HurtEvent OnHurt;
        public void Hurt(float damage, Vector2 damagePosition, out GameObject hitObject)
        {
            OnHurt?.Invoke(damage, damagePosition, out hitObject);
        }
        public void BindListener(HurtEvent hurtAction)
        {
            OnHurt += hurtAction;
        }
        public void ReleaseListener(HurtEvent hurtAction)
        {
            OnHurt -= hurtAction;
        }
    }
    public struct DamagePacket
    {
        public float Damage;
        public Vector2 DamagePosition;
        public DungeonUnit Sender;
        public DungeonUnit Receiver;
        public long CreditCardNumber;
        public int ThreeDigitsOnTheBack;
        public System.DateTime ExpirationMonthAndYear;
    }
    public class Player : MonoBehaviour, IDemoDamageable
    {
        int lives = 3;
        public void Hurt(float damage)
        {
            void Die()
            {
                gameObject.SetActive(false);
            }
            if (damage > 0 && lives <= 0)
            {
                Die();
            }
            lives -= 1;
        }
    }
    public class Projectile : MonoBehaviour
    {
        float damage = 69;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform != null && collision.transform.TryGetComponent(out IDemoDamageable d))
            {
                d.Hurt(damage);
            }
        }
    }
    #endregion


    public class MoveProjectile : MonoBehaviour
    {
        public static void SpawnProjectile(MoveProjectile prefab, Vector2 target, GameObject owner, float speed)
        {
            MoveProjectile p = Instantiate(prefab, owner.transform.position, Quaternion.identity);
            p.owner = owner;
            p.CurrentVelocity = target - (Vector2)owner.transform.position;
            p.speed = speed;
            p.Action_FaceDirection();

        }
        public MoveProjectile Action_SetDamage(float d)
        {
            damage = d;
            return this;
        }
        public MoveProjectile Action_FaceDirection()
        {
            if (CurrentVelocity.sqrMagnitude > 0.01f)
            {
                transform.Lookat2D(CurrentVelocity);
            }
            return this;
        }
        public MoveProjectile Action_AddRotation(float r)
        {
            CurrentVelocity = CurrentVelocity.Rotate2D(r);
            Action_FaceDirection();
            return this;
        }
        public Vector2 CurrentVelocity { get; private set; }
        [SerializeField] float speed;
        [SerializeField] float damage;
        public GameObject owner;
        void Update()
        {
            transform.Translate(CurrentVelocity.ScaleToMagnitude(1f) * Time.deltaTime * speed);
            if (transform.position.SquareDistanceToGreaterThan(owner.transform.position, 100f))
            {
                Destroy(gameObject);
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                //collision.gameObject.GetComponent<EnemyController>().TakeDamage(damage);
            }
            if (!collision.CompareTag("Building") || !collision.CompareTag("Player"))
            {
                Destroy(gameObject);
            }
        }

    }
    public class Conveyor : MonoBehaviour
    {
        public struct ConveyorSettings
        {
            public float Force;
            public float Acceleration;
        }
        [SerializeField] float force = 1.5f;
        [SerializeField] float acceleration = 10f;
        public ConveyorSettings Settings => new()
        {
            Force = force,
            Acceleration = acceleration
        };
        public Vector2 DirectionWithForce => transform.forward * force;
        HashSet<IConveyorable> trackedObjects;
        Dictionary<string, int> currentResources = new();
        public int CheckResourceValue(string resource)
        {
            if (!currentResources.ContainsKey(resource))
                return 0;
            return currentResources[resource];
        }
        public void AddResource(string resource, int value)
        {
            int resourceValue = 0;
            if (!currentResources.ContainsKey(resource))
            {
                currentResources[resource] = 0;
            }
            if (currentResources.TryGetValue(resource, out resourceValue))
            {
                resourceValue += value;
                currentResources[resource] = resourceValue;
            }
        }
        private void Update()
        {
            foreach (var item in trackedObjects)
            {
                PushOther(item);
            }
            DungeonUnit churro = DungeonUnit.Player;


        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform != null && collision.transform.TryGetComponent(out IConveyorable c))
            {
                trackedObjects.Add(c);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform != null && collision.transform.TryGetComponent(out IConveyorable c))
            {
                trackedObjects.Remove(c);
            }
        }
        public bool PushOther(IConveyorable c)
        {
            if (c == null)
            {
                return false;
            }
            c.Push(this);
            return true;
        }

    }
    public interface IConveyorable
    {
        public void Push(Conveyor c);
    }
    public class DemoStat : ScriptableObject
    {
        [field: SerializeField] public string statName { get; private set; } = "Headhunter, Leather Belt";
    }
    public class ResourceItem : MonoBehaviour
    {
        DemoStat addStat;
        public DemoStat GetStatType() => addStat;
        public string statName => addStat.statName;
    }
}

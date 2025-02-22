using Bremsengine;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ChurroIceDungeon
{
    #region Damageable
    public partial class DestructionItem : IDamageable, IFaction
    {
        IDamageable Damageable => (IDamageable)this;
        float IDamageable.CurrentHealth { get; set; }
        public BremseFaction Faction => ((IFaction)this).Faction;
        BremseFaction IFaction.Faction { get; set; }
        void IDamageable.Hurt(float damage, Vector2 damagePosition)
        {
            Damageable.CurrentHealth -= damage;
            if (Damageable.CurrentHealth <= 0f && !isDestroyed)
            {
                DestroyItem();
            }
        }
    }
    #endregion
    public partial class DestructionItem : MonoBehaviour
    {
        public float CollectValue() => destructionValue;
        [SerializeField] float destructionValue = 5f;
        [SerializeField] float destructionVisualDelay = 0f;
        [SerializeField] float startingHealth = 100f;
        [Range(0,500)]
        [SerializeField] int strengthOnDestroy = 1;
        [SerializeField] BremseFaction faction = BremseFaction.None;
        [SerializeField] UnityEvent destroyEvent;
        [SerializeField] bool spawnExplosion = false;
        [Range(1,15)]
        [SerializeField] float explosionScale;
        [SerializeField] DebrisPacket debrisOnDestroy;
        [SerializeField] bool TestkillItem;
        public bool isDestroyed { get; private set; }
        private void Awake()
        {
            Damageable.CurrentHealth = startingHealth;
            ((IFaction)this).SetFaction(faction);
        }
        public void DestroyItem()
        {
            IEnumerator CO_DestroyAfter(float delay)
            {
                yield return new WaitForSeconds(delay);
                if (TestkillItem)
                {
                    gameObject.SetActive(false);
                }

            }
            if (gameObject.activeInHierarchy && !isDestroyed)
            {
                destroyEvent?.Invoke();
                isDestroyed = true;
                if (destructionVisualDelay > 0f)
                {
                    StartCoroutine(CO_DestroyAfter(destructionVisualDelay));
                }
                else
                {
                    gameObject.SetActive(false);
                }
                ChurroManager.AddDestruction(destructionValue);
                if (spawnExplosion)
                {
                    GeneralManager.FunnyExplosion(transform.position, explosionScale);
                }
                ChurroManager.SpawnDebris(transform.position, debrisOnDestroy);
                ChurroManager.ChangeStrength(strengthOnDestroy);
            }
        }
    }
}

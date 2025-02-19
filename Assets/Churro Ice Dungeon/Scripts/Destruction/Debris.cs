using Core.Extensions;
using UnityEngine;

namespace ChurroIceDungeon
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class Debris : MonoBehaviour
    {
        [SerializeField] SpriteRenderer sr;
        [SerializeField] Rigidbody2D rb;
        public void SetDebris(DebrisPacket p)
        {
            sr.sprite = p.sprite;
            sr.color = p.debrisColor;
            sr.transform.localScale = new(p.size, p.size, 1f);
            MoveDebris(p.force);
        }
        public void MoveDebris(float force)
        {
            rb.linearVelocity = (force.RandomPositiveNegativeRange() * Random.insideUnitCircle);
        }
    }
    [System.Serializable]
    public struct DebrisPacket
    {
        [SerializeField] Sprite[] spriteList;
        public Sprite sprite => spriteList[Random.Range(0, spriteList.Length)];
        public Color32 debrisColor;
        [Range(0.1f, 3f)]
        public float size;
        [Range(0, 20)]
        public int amount;
        [Range(0f, 10f)]
        public float force;
    }
}
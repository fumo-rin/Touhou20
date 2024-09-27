using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bremsengine
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ProjectileSprite : MonoBehaviour
    {
        public static implicit operator SpriteRenderer(ProjectileSprite p) => p.sprite;
        public static implicit operator Sprite(ProjectileSprite p) => p.sprite.sprite;
        [SerializeField] Projectile assignedProjectile;
        [SerializeField] SpriteRenderer sprite;
        public Texture Texture => sprite.sprite.texture;
        public Color32 Color => sprite.color;
        public void SetColor(Color32 c) => sprite.color = c;
        public bool IsOffScreen => !sprite.IsVisible(Camera.main);
        public void SetSprite(Sprite s, SpriteRenderer other)
        {
            sprite.sprite = s;
            sprite.sortingOrder = other.sortingOrder;
            sprite.sharedMaterial = other.sharedMaterial;
            sprite.sortingLayerID = other.sortingLayerID;
        }
        private void OnBecameInvisible()
        {
            if (assignedProjectile.Active)
            {
                assignedProjectile.QueueRecalculateOffscreen();
            }
        }
        private void OnBecameVisible()
        {
            if (assignedProjectile.Active)
            {
                assignedProjectile.SetOffScreen(false);
            }
        }
    }
}

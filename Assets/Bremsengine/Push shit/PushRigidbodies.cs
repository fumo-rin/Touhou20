using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Extensions;

namespace Bremsengine
{
    public class PushRigidbodies : MonoBehaviour
    {
        [SerializeField] Transform pushAnchor;
        [SerializeField] LayerMask mask;
        [SerializeField] float radius = 1.5f;
        [SerializeField] Vector2 force = new(15f, 25f);
        [SerializeField] float rotationForce = 1f;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            DamageNumbersHelper.Spawn(1000f.Spread(5f), pushAnchor.position);
            RigidbodyExplosion.Explode(pushAnchor.position, 2f, radius, mask, force.RandomBetweenXY(), RigidbodyExplosion.ProximityMode.Distance, rotationForce);
        }
    }
}

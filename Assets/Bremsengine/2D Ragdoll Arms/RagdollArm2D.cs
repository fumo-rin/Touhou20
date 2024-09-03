using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Extensions;

namespace Bremsengine
{
    public class RagdollArm2D : MonoBehaviour
    {
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + idleAnchor);
            Gizmos.DrawLine(transform.position, (Vector2)transform.position+ fallingAnchor);
        }
        Vector2 Velocity;
        Vector3 LastPosition;
        [SerializeField] float maxFallSpeed = 15f;
        [SerializeField] Vector2 idleAnchor;
        [SerializeField] Vector2 fallingAnchor;
        [SerializeField] float xOffset; 
        float lerp;
        [SerializeField] float gravity= 10f;
        private void Update()
        {
            Velocity = (transform.position - LastPosition) * (1f / Time.deltaTime);
            LastPosition = transform.position;
        }
        private void LateUpdate()
        {
            float fallDot = maxFallSpeed - (maxFallSpeed - Velocity.y);
            fallDot /= maxFallSpeed.Absolute();

            lerp += -fallDot * Time.deltaTime * gravity;
            if (lerp > 0f && Velocity.y > -2f)
            {
                lerp -= Time.deltaTime * gravity;
            }
            lerp = lerp.Clamp(0f, 1f);

            Vector2 lookPosition = Vector2.Lerp((Vector2)transform.position + idleAnchor, (Vector2)transform.position + fallingAnchor, lerp);
            transform.Lookat2D(lookPosition, TransformExtensions.LookDirection2D.Down);
        }
    }
}

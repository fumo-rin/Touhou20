using Bremsengine;
using Core.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    [DefaultExecutionOrder(-1)]
    public class GrazeBox : MonoBehaviour
    {
        [SerializeField] BaseUnit owner;
        public float radius = 1f;
        [SerializeField] LayerMask grazeLayer;
        Collider2D[] grazeIteration;
        static HashSet<int> grazedBulletIDs = new HashSet<int>();
        public static int GrazeCount;
        [SerializeField] AudioClipWrapper grazeAudio;
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green.Opacity(80);
            Gizmos.DrawSphere(transform.position, radius);
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void ResetGrazeCount()
        {
            GrazeCount = 0;
            grazedBulletIDs.Clear();
        }
        private void Update()
        {
            grazeIteration = Physics2D.OverlapCircleAll(transform.position, radius, grazeLayer);
            if (grazeIteration == null || grazeIteration.Length <= 0)
                return;

            foreach (Collider2D col in grazeIteration)
            {
                if (col == null || col.transform == null)
                    continue;

                if (col.transform.GetComponent<Projectile>() is not null and Projectile p)
                {
                    if (grazedBulletIDs.Contains(p.projectileID))
                        continue;
                    if (owner.FactionInterface.IsFriendsWith(p.Faction))
                        continue;

                    grazedBulletIDs.Add(p.projectileID);
                    GrazeCount++;
                    OnGraze?.Invoke(GrazeCount);
                    grazeAudio.Play(p.Position);
                }
            }
        }
        public delegate void GrazeAction(int newGrazeCount);
        public static GrazeAction OnGraze;
    }
}

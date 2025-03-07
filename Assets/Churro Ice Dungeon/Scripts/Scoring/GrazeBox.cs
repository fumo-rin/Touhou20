using Core.Extensions;
using Mono.CSharp;
using System.Collections.Generic;
using UnityEngine;

namespace ChurroIceDungeon
{
    [DefaultExecutionOrder(-1)]
    public class GrazeBox : MonoBehaviour
    {
        [SerializeField] DungeonUnit owner;
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
        public static void Unregister(int id)
        {
            if (grazedBulletIDs != null)
            {
                grazedBulletIDs.Remove(id);
            }
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

                if (col.transform.GetComponent<ChurroProjectile>() is not null and ChurroProjectile p)
                {
                    if (grazedBulletIDs.Contains(p.SpawnID))
                        continue;
                    if (owner.FactionInterface.IsFriendsWith(p.Faction))
                        continue;

                    grazedBulletIDs.Add(p.SpawnID);
                    GrazeCount++;
                    OnGraze?.Invoke(GrazeCount);
                    grazeAudio.Play(p.CurrentPosition);
                }
            }
        }
        public delegate void GrazeAction(int newGrazeCount);
        public static GrazeAction OnGraze;
    }
}

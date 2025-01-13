using Bremsengine;
using NUnit.Framework;
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
        HashSet<int> grazedBulletIDs = new HashSet<int>();
        public static int GrazeCount;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void ResetGrazeCount()
        {
            GrazeCount = 0; 
        }
        private void Update()
        {
            grazeIteration = Physics2D.OverlapCircleAll(transform.position, radius, grazeLayer);
            if (grazeIteration != null && grazeIteration.Length > 0)
            {
                foreach (Collider2D col in grazeIteration)
                {
                    if (col == null || col.transform == null)
                        continue;

                    if (col.transform.GetComponent<Projectile>() is not null and Projectile p)
                    {
                        if (grazedBulletIDs.Contains(p.projectileID))
                            continue;
                        if (owner.FactionInterface.IsFriendsWith(p.Faction))
                        {
                            continue;
                        }
                        grazedBulletIDs.Add(p.projectileID);
                        GrazeCount++;
                        OnGraze?.Invoke(GrazeCount);
                        Debug.Log("Graze : " + GrazeCount.ToString());
                    }
                }
            }
        }
        public delegate void GrazeAction(int newGrazeCount);
        public static GrazeAction OnGraze;
    }
}

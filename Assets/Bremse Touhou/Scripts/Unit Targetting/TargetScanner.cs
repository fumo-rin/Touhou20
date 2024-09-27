using Core;
using System.Collections.Generic;
using UnityEngine;

namespace BremseTouhou
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class TargetScanner : MonoBehaviour
    {
        #region Scanners List
        [SerializeField]
        List<Vector2> scanners = new()
        {
            new(0.5f,0.5f),
            new(0f,0f),
            new(-0.5f,-0.5f),
            new(0.5f,-0.5f),
            new(-0.5f,0.5f)
        };
        #endregion
        HashSet<BaseUnit> trackedUnits = new();
        [SerializeField] LayerMask blockingLayer;
        [SerializeField] float maxScanRange = 25f;

        [SerializeField] BaseUnit owner;
        BoxCollider2D scannerHitbox;
        [SerializeField] float loseTargetTime = 3f;
        private void OnValidate()
        {
            scannerHitbox = GetComponent<BoxCollider2D>();
            scannerHitbox.isTrigger = true;
        }
        private void Awake()
        {
            scannerHitbox= GetComponent<BoxCollider2D>();
            scannerHitbox.isTrigger = true;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.GetComponent<BaseUnit>() is BaseUnit unit and not null && !unit.FactionInterface.IsFriendsWith(owner.FactionInterface.Faction))
            {
                trackedUnits.Add(unit);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform.GetComponent<BaseUnit>() is BaseUnit unit and not null)
            {
                if (owner.IsTarget(unit))
                {
                    owner.LoseTarget(loseTargetTime);
                }
                trackedUnits.Remove(unit);
            }
        }
        RaycastHit2D iterationHit;
        public bool TryScan(out BaseUnit found)
        {
            found = null;
            foreach (BaseUnit unit in trackedUnits)
            {
                foreach (var item in scanners)
                {
                    if (RaycastHelper.Cast(owner.Center + item,unit.Center,blockingLayer, out iterationHit, maxScanRange))
                    {
                        if (iterationHit.transform.TryGetComponent(out BaseUnit hitUnit))
                        {
                            found = hitUnit;
                            break;
                        }
                    }
                }
            }
            return found != null;
        }
    }
}

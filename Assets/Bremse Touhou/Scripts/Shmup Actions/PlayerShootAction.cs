using UnityEngine;
using Core.Input;
using Bremsengine;
using System.Collections.Generic;

namespace BremseTouhou
{
    public class PlayerShootAction : MonoBehaviour
    {
        [SerializeField] BremseInputEventBus eventBus;
        [SerializeField] List<PlayerAttackHandler> handlers = new();
        bool isHeld;
        private void Start()
        {
            /*eventBus.BindAction(BremseInputPhase.Performed, PressDown);
            eventBus.BindAction(BremseInputPhase.Cancelled, PressUp);*/
        }
        private void OnDestroy()
        {
            /*eventBus.ReleaseAction(BremseInputPhase.Performed, PressDown);
            eventBus.ReleaseAction(BremseInputPhase.Cancelled, PressUp);*/
        }
        private void PressDown()
        {
            isHeld = true;
            foreach (var item in handlers)
            {
                item.SetAttackPressed(isHeld);
            }
        }
        private void PressUp()
        {
            isHeld = false;
            foreach (var item in handlers)
            {
                item.SetAttackPressed(isHeld);
            }
        }
    }
}

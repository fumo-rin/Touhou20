using UnityEngine;
using Core.Input;
using Bremsengine;

namespace BremseTouhou
{
    public class PlayerShootAction : MonoBehaviour
    {
        [SerializeField] BremseInputEventBus eventBus;
        [SerializeField] PlayerAttackHandler attackHandler;
        bool isHeld;
        private void Start()
        {
            eventBus.BindAction(BremseInputPhase.Performed, PressDown);
            eventBus.BindAction(BremseInputPhase.Cancelled, PressUp);
        }
        private void OnDestroy()
        {
            eventBus.ReleaseAction(BremseInputPhase.Performed, PressDown);
            eventBus.ReleaseAction(BremseInputPhase.Cancelled, PressUp);
        }
        private void Update()
        {
            attackHandler.SetAttackPressed(isHeld);
        }
        private void PressDown()
        {
            isHeld = true;
        }
        private void PressUp()
        {
            isHeld = false;
        }
    }
}

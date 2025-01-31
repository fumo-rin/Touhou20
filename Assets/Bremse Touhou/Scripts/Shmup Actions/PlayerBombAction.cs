using Bremsengine;
using Core.Extensions;
using Core.Input;
using UnityEngine;

namespace BremseTouhou
{
    public class PlayerBombAction : MonoBehaviour
    {
        [SerializeField] BremseInputEventBus eventBus;
        [SerializeField] FloatSO bombLength;
        public static bool CanBomb = true;
        private void Start()
        {
            eventBus.BindAction(BremseInputPhase.JustPressed, TriggerBomb);
        }
        private void OnDestroy()
        {
            eventBus.ReleaseAction(BremseInputPhase.JustPressed, TriggerBomb);
        }
        private void TriggerBomb()
        {
            Projectile.PlayerTriggerBomb(bombLength);
        }
    }
}

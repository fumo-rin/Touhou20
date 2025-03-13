using Core.Extensions;
using Core.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ChurroIceDungeon
{
    public class PlayerBomb : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void ReinitializeBomb()
        {
            NextBombTime = 0f;
        }
        [SerializeField] DungeonUnit owner;
        static float NextBombTime;
        public int BombCount => (currentBombValue / BombUseCost).ToInt();
        public float BombUseCost = 800f;
        public float currentBombValue = 0f;
        [SerializeField] float bombDuration = 4f;
        public float BarUIValue => currentBombValue % BombUseCost;
        public bool CanBomb => BombCount > 0 && Time.time >= NextBombTime;
        bool BombInputHeld = false;
        public void AddBombValue(float amount)
        {
            currentBombValue += amount;
        }
        public void SetBombCost(float cost)
        {
            BombUseCost = cost;
        }
        public void TryTriggerBomb()
        {
            if (owner.IsAlive() && BombInputHeld && CanBomb)
            {
                TriggerBomb(bombDuration);
            }
        }
        private void TriggerBomb(float duration)
        {
            NextBombTime = Time.time + duration;
            ChurroProjectile.SweepBullets(0.5f, 0);
            WakaUnit.TriggerIFrames(bombDuration);
        }
        private void PressBombInput(InputAction.CallbackContext c)
        {
            BombInputHeld = true;
        }
        private void ReleaseBombInput(InputAction.CallbackContext c)
        {
            BombInputHeld = false;
        }
        private void Update()
        {
            TryTriggerBomb();
        }
        private void OnEnable()
        {
            PlayerInputController.actions.Shmup.Bomb.performed += PressBombInput;
            PlayerInputController.actions.Shmup.Bomb.canceled += ReleaseBombInput;
        }
        private void OnDisable()
        {
            PlayerInputController.actions.Shmup.Bomb.performed -= PressBombInput;
            PlayerInputController.actions.Shmup.Bomb.canceled -= ReleaseBombInput;
        }
    }
}

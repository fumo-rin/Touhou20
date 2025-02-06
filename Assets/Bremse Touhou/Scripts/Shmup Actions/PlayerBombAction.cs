using Bremsengine;
using Core.Extensions;
using Core.Input;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BremseTouhou
{
    public class PlayerBombAction : MonoBehaviour
    {
        [SerializeField] BremseInputEventBus eventBus;
        [SerializeField] FloatSO bombLength;
        [SerializeField] AudioClipWrapper bombSound;
        [SerializeField] AudioClipWrapper bombSoundExplosion;
        [SerializeField] ParticleSystem explosionCharge;
        [SerializeField] ParticleSystem explosionFart;
        public static int Cost => bombCost;
        static int bombCost;
        static int maximumBombValue;
        static int currentBombValue = 0;
        public static float BombIframesTime { get; private set; }
        public static bool CanBomb => PlayerUnit.Player.Alive && currentBombValue >= bombCost;
        public static int Full => maximumBombValue;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Reinitialize()
        {
            bombCost = 950;
            maximumBombValue = 950;
            currentBombValue = maximumBombValue;
            BombIframesTime = 0f;
        }
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
            if (this == null) { return; }
            if (CanBomb)
            {
                Projectile.PlayerTriggerBomb(bombLength, 2f,bombSound, bombSoundExplosion);
                PlayerUnit.SetIFrames(bombLength + 2f, true);
                StartCoroutine(BombEffects(2f));
                SetBombValue(currentBombValue - bombCost);
                BombIframesTime = Time.time + bombLength;
            }
        }
        private IEnumerator BombEffects(float explosionDelay)
        {
            explosionCharge.Play();
            yield return new WaitForSeconds(explosionDelay);
            explosionFart.Play();
        }
        public static void AddBombValue(int newValue)
        {
            SetBombValue(currentBombValue + newValue);
        }
        public delegate void BombUIPacket(int current, int maximum);
        public static BombUIPacket OnRefresh;
        private static void SendBombValues(int current, int maximum)
        {
            OnRefresh?.Invoke(current, maximum);
        }
        public static void RequestRefresh()
        {
            SendBombValues(currentBombValue, maximumBombValue);
        }
        public static void SetBombValue(int newValue)
        {
            currentBombValue = newValue.Clamp(0, maximumBombValue);
            SendBombValues(currentBombValue, maximumBombValue);
        }
    }
}

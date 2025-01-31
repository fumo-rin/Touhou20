using Bremsengine;
using Core.Extensions;
using Core.Input;
using System.Collections;
using UnityEngine;

namespace BremseTouhou
{
    public class PlayerBombAction : MonoBehaviour
    {
        [SerializeField] BremseInputEventBus eventBus;
        [SerializeField] FloatSO bombLength;
        [SerializeField] AudioClipWrapper bombSound;
        [SerializeField] AudioClipWrapper bombSoundExplosion;
        public static int Cost => bombCost;
        static int bombCost;
        static int maximumBombValue;
        static int currentBombValue = 0;
        public static float BombIframesTime { get; private set; }
        public static bool CanBomb => PlayerUnit.Player.Alive && currentBombValue >= bombCost;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Reinitialize()
        {
            bombCost = 600;
            maximumBombValue = 600;
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
            if (CanBomb)
            {
                Projectile.PlayerTriggerBomb(bombLength, bombSound,bombSoundExplosion);
                PlayerUnit.SetIFrames(bombLength, true);
                SetBombValue(currentBombValue - bombCost);
                BombIframesTime = Time.time + bombLength;
            }
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

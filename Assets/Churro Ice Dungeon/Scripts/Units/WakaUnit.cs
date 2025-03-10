using Bremsengine;
using Core.Extensions;
using Core.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ChurroIceDungeon
{
    #region Damage Taken
    public partial class WakaUnit
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void ReinitializePlayerLives()
        {
            int newLives = 9;
            GeneralManager.StoreGameValue(GeneralManager.Keys.PlayerLives, newLives);
        }
        [SerializeField] SpriteFlashMaterial onHurtFlashMaterial;
        float iFramesEndTime;
        [SerializeField] float iFramesLength = 2f;
        public delegate void DamagedEvent(int newLives, int maxLives);
        public static event DamagedEvent OnLivesChanged;
        public static void RequestLivesRefresh()
        {
            if (GeneralManager.TryFetchGameValue(GeneralManager.Keys.PlayerLives, out int lives))
            {
                SendLivesChanged(lives, maxLives);
            }
            else
            {
                Debug.LogWarning("Failed to find lives");
            }
        }
        private static int maxLives = 9;
        public override bool IsAlive()
        {
            return gameObject.activeInHierarchy && Health > 0f;
        }
        protected override void OnHurt(float damage, Vector2 damagePosition)
        {
            if (Time.time < iFramesEndTime)
            {
                return;
            }
            GeneralManager.TryFetchGameValue(GeneralManager.Keys.PlayerLives, out int lives);
            bool respawn = lives > 0;
            if (respawn)
            {
                onHurtFlashMaterial.TriggerFlashMaterial(iFramesLength);
                iFramesEndTime = Time.time + iFramesLength;
                lives -= 1;
            }
            else if (gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);
            }
            WakaScoring.PlayerDeathRecalculateScoreValue(0.85f);
            GeneralManager.StoreGameValue(GeneralManager.Keys.PlayerLives, lives);
            SendLivesChanged(lives, maxLives);
        }
        private static void SendLivesChanged(int newLives, int maxLives)
        {
            OnLivesChanged?.Invoke(newLives, maxLives);
        }
    }
    #endregion
    #region Power Scaler
    public partial class WakaUnit : PowerScaler
    {
        [SerializeField] float TestPower;
        float CurrentPower => TestPower;

        public override float DamageScale(float damage)
        {
            return ScaleDamage(damage);
        }

        public float ScaleDamage(float damage)
        {
            float scale = (CurrentPower / 100f).Max(1f) * (1f + ((ChurroManager.Strength - 100) / 250f)).Max(1f);
            return damage * scale;
        }
    }
    #endregion
    #region Attack Handler
    public partial class WakaUnit
    {
        public AttackHandler attackHandler;
        Vector2 cursorPosition => RenderTextureCursorHandler.CursorPosition;
        bool IsHoveringGame => RenderTextureCursorHandler.IsHovering;
        bool attackPressed;
        public void Attack(Vector2 worldPosition)
        {
            if (attackHandler.TryAttack(worldPosition))
            {

            }
        }
        private void AttackLoop()
        {
            if (IsHoveringGame && attackPressed)
            {
                Attack(cursorPosition);
            }
        }
    }
    #endregion
    #region Motor Focus
    public partial class WakaUnit
    {
        [SerializeField] DungeonMotor focusMotor;
        bool focusMovement;
        private void PressFocus(InputAction.CallbackContext c)
        {
            focusMovement = true;
        }
        private void ReleaseFocus(InputAction.CallbackContext c)
        {
            focusMovement = false;
        }
        public override DungeonMotor CollapseMotor()
        {
            if (focusMovement)
            {
                return focusMotor;
            }
            return base.CollapseMotor();
        }
    }
    #endregion
    public partial class WakaUnit : DungeonUnit
    {
        [SerializeField] Animator movementAnimator;
        Vector2 moveInput;
        private void ReadInput(ref Vector2 input)
        {
            input = Vector2.zero;
            if (PlayerInputController.actions == null)
                return;
            input = PlayerInputController.actions.Player.Move.ReadValue<Vector2>().QuantizeToStepSize(45f);
        }
        private void Update()
        {
            ReadInput(ref moveInput);
            MoveMotor(moveInput, out DungeonMotor.MotorOutput result);
            if (result.Failed == false && movementAnimator != null)
            {
                float speed = RB.linearVelocity.magnitude;
                movementAnimator.SetFloat("VELOCITY", speed);
                if (moveInput != Vector2.zero && speed > 0.5f)
                {
                    DanceController.CancelDance(true);
                }
                else
                {
                    DanceController.CancelDance(false);
                }
            }
            AttackLoop();
        }
        protected override void WhenAwake()
        {

        }
        protected override void WhenDestroy()
        {
            PlayerInputController.actions.Player.Focus.performed -= PressFocus;
            PlayerInputController.actions.Player.Focus.canceled -= ReleaseFocus;
        }

        [SerializeField] LayerMask testUnitLayer;
        protected override void WhenStart()
        {
            PlayerInputController.actions.Player.Focus.performed += PressFocus;
            PlayerInputController.actions.Player.Focus.canceled += ReleaseFocus;
            if (GeneralManager.TryFetchGameValue(GeneralManager.Keys.PlayerLives, out int lives))
            {
                SendLivesChanged(lives, maxLives);
            }
        }
    }
}

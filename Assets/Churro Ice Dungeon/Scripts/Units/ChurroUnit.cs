using Bremsengine;
using Core.Extensions;
using Core.Input;
using UnityEngine;

namespace ChurroIceDungeon
{
    #region Click Action
    public partial class ChurroUnit
    {
        private void OnWorldClick(Vector2 position)
        {
            Debug.DrawLine(position, UnitPosition, ColorHelper.PastelPurple, 1f);
            if (attackHandler != null)
            {
                ClickAttack(position);
            }
        }
    }
    #endregion
    #region Power Scaler
    public partial class ChurroUnit : PowerScaler
    {
        [SerializeField] float TestPower;
        float CurrentPower => TestPower;

        public override float DamageScale(float damage)
        {
            return ((PowerScaler)this).ScaleDamage(damage);
        }

        float PowerScaler.ScaleDamage(float damage)
        {
            return damage * (CurrentPower / 100f).Max(1f);
        }
    }
    #endregion
    #region Attack Handler
    public partial class ChurroUnit
    {
        public AttackHandler attackHandler;
        public void ClickAttack(Vector2 worldPosition)
        {
            attackHandler.TryAttack(worldPosition);
        }
    }
    #endregion
    public partial class ChurroUnit : DungeonUnit
    {
        public static ChurroUnit PlayerChurro => (ChurroUnit)Player;
        public static bool DoesPlayerExist => PlayerChurro != null;
        Vector2 moveInput;
        private void ReadInput(ref Vector2 input)
        {
            input = Vector2.zero;
            if (PlayerInputController.actions == null)
                return;
            input = PlayerInputController.actions.Player.Move.ReadValue<Vector2>();
        }
        private void Update()
        {
            ReadInput(ref moveInput);
            MoveMotor(moveInput, out DungeonMotor.MotorOutput result);
        }

        protected override void WhenAwake()
        {

        }

        protected override void WhenDestroy()
        {
            RenderTextureCursorHandler.StaticRectWorldPositionClick -= OnWorldClick;
        }

        protected override void WhenStart()
        {
            RenderTextureCursorHandler.StaticRectWorldPositionClick += OnWorldClick;
        }
    }
}
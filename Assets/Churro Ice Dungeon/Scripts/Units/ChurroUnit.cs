using Bremsengine;
using Core.Extensions;
using Core.Input;
using UnityEngine;

namespace ChurroIceDungeon
{
    #region Damageable
    public partial class ChurroUnit
    {
        public override bool IsAlive()
        {
            return gameObject.activeInHierarchy && Health > 0f;
        }

        protected override void OnHurt(float damage, Vector2 damagePosition)
        {
            if (ChurroManager.CanRespawn)
            {
                ChurroManager.ChangeBraincells(ChurroManager.RespawnCost);
            }
        }
    }
    #endregion
    #region Click Action
    public partial class ChurroUnit
    {
        RaycastHit2D[] clickHit = new RaycastHit2D[25];
        private void UpdateClickCast()
        {
            RaycastHit2D hit;
            RenderTextureHoverTooltip tooltip = null;
            clickHit = Physics2D.CircleCastAll(RenderTextureCursorHandler.CursorPosition, 1f, Vector2.up, 0f);
            for (int i = 0; i < clickHit.Length; i++)
            {
                hit = clickHit[i];
                if (hit.collider.isTrigger)
                    continue;
                if (hit.transform == null)
                    continue;

                if (hit.transform.GetComponent<RenderTextureHoverTooltip>() is RenderTextureHoverTooltip t and not null)
                {
                    tooltip = t;
                    t.Hover();
                    break;
                }
            }
            if (tooltip == null)
            {
                RenderTextureHoverTooltipUI.ClearTooltipText();
            }
        }
        private void OnWorldRelease(Vector2 position, PointerButton pressType)
        {
            switch (pressType)
            {
                case PointerButton.Left:
                    attackPressed = false;
                    break;
                case PointerButton.Right:
                    break;
                case PointerButton.Middle:
                    break;
                default:
                    break;
            }
        }
        private void OnWorldClick(Vector2 position, PointerButton pressType)
        {
            clickHit = Physics2D.CircleCastAll(position, 1f, Vector2.up, 0f);
            bool TryAction<T>(out T output) where T : Object
            {
                output = null;
                if (position.SquareDistanceToGreaterThan(CurrentPosition, 3f))
                {
                    return false;
                }
                for (int i = 0; i < clickHit.Length; i++)
                {
                    if (clickHit[i].collider.isTrigger)
                        continue;
                    if (clickHit[i].transform != null && clickHit[i].transform.GetComponent<T>() is T t and not null)
                    {
                        output = t;
                        break;
                    }
                }
                return output != null;
            }
            bool TryTalk(out Dialogue dialogue)
            {
                return TryAction(out dialogue);
            }
            bool TryRenderTextureClick(out RenderTextureClickAction clickAction)
            {
                return TryAction(out clickAction);
            }
            Debug.DrawLine(position, CurrentPosition, ColorHelper.PastelPurple, 1f);
            switch (pressType)
            {
                case PointerButton.Left:
                    if (TryTalk(out Dialogue d))
                    {
                        d.StartDialogue(0);
                        return;
                    }
                    if (TryRenderTextureClick(out RenderTextureClickAction clickAction))
                    {
                        clickAction.ClickPayload();
                        return;
                    }
                    if (attackHandler != null)
                    {
                        attackPressed = true;
                        return;
                    }
                    break;
                case PointerButton.Right:
                    break;
                case PointerButton.Middle:
                    break;
                default:
                    break;
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
        Vector2 cursorPosition => RenderTextureCursorHandler.CursorPosition;
        bool IsHoveringGame => RenderTextureCursorHandler.IsHovering;
        bool attackPressed;
        public void Attack(Vector2 worldPosition)
        {
            attackHandler.TryAttack(worldPosition);
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
    public partial class ChurroUnit : DungeonUnit
    {
        public static ChurroUnit PlayerChurro => (ChurroUnit)Player;
        public static bool DoesPlayerExist => PlayerChurro != null;
        [SerializeField] Animator movementAnimator;
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
            UpdateClickCast();
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
            RenderTextureCursorHandler.ClickDown -= OnWorldClick;
            RenderTextureCursorHandler.ClickUp -= OnWorldRelease;
        }

        protected override void WhenStart()
        {
            RenderTextureCursorHandler.ClickDown += OnWorldClick;
            RenderTextureCursorHandler.ClickUp += OnWorldRelease;
            RenderTextureCursorHandler.SetControllerReference(transform);
        }
    }
}
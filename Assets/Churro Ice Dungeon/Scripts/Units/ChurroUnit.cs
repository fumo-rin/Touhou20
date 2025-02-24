using Bremsengine;
using Core.Extensions;
using Core.Input;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ChurroIceDungeon
{
    #region Damageable
    public partial class ChurroUnit
    {
        [SerializeField] SpriteFlashMaterial onHurtFlashMaterial;
        float iFramesEndTime;
        [SerializeField] float iFramesLength = 2f;
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
            bool respawn = ChurroManager.CanRespawn;
            if (respawn)
            {
                ChurroManager.ChangeBraincells(ChurroManager.RespawnCost);
                onHurtFlashMaterial.TriggerFlashMaterial(iFramesLength);
                iFramesEndTime = Time.time + iFramesLength;
                ChurroManager.KillChangeStrength(0.75f);
            }
            else if (gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);
                ChurroManager.LoseGame();
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
    #region Inventory
    public partial class ChurroUnit
    {
        static Inventory assignedInventory;
        static Dictionary<int, Inventory.itemSlotSnapshot> InventorySnapShot;
        public static void ClearInventorySnapshot()
        {
            assignedInventory.ClearInventory(ref InventorySnapShot);
        }
        public static bool TryGetFromSnapshot(int id, out Inventory.itemSlotSnapshot item)
        {
            item = new();
            if (InventorySnapShot == null)
            {
                return false;
            }
            if (InventorySnapShot.ContainsKey(id))
            {
                item = InventorySnapShot[id];
                return true;
            }
            return false;
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void ReinitializeInventorySnapshot()
        {
            assignedInventory = null;
            InventorySnapShot = null;
        }
        public static void SetInventory(Inventory i)
        {
            assignedInventory = i;
        }
        public static void SnapshotInventory()
        {
            Debug.Log("Try Set Player Snapshot");
            if (Player != null && assignedInventory == null)
            {
                Debug.LogError("Bad");
            }
            if (assignedInventory.SnapshotCurrent(out Dictionary<int, Inventory.itemSlotSnapshot> snapShot))
            {
                Debug.Log("Success, Snapshotted " + snapShot.Count + " items");
                InventorySnapShot = snapShot;
            }
        }
        public static bool TryGetInventory(out Inventory i)
        {
            i = assignedInventory;
            return i != null;
        }
    }
    #endregion
    #region Motor Focus
    public partial class ChurroUnit
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
    public partial class ChurroUnit : DungeonUnit
    {
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
            GeneralManager.OnStageExitPreLoadingScreen -= SnapshotInventory;

            PlayerInputController.actions.Player.Focus.performed -= PressFocus;
            PlayerInputController.actions.Player.Focus.canceled -= ReleaseFocus;
        }

        protected override void WhenStart()
        {
            RenderTextureCursorHandler.ClickDown += OnWorldClick;
            RenderTextureCursorHandler.ClickUp += OnWorldRelease;

            RenderTextureCursorHandler.SetControllerReference(transform);
            GeneralManager.OnStageExitPreLoadingScreen += SnapshotInventory;

            PlayerInputController.actions.Player.Focus.performed += PressFocus;
            PlayerInputController.actions.Player.Focus.canceled += ReleaseFocus;
        }
    }
}
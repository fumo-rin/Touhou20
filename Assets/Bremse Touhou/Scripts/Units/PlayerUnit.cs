using Bremsengine;
using Core;
using Core.Extensions;
using Core.Input;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
namespace BremseTouhou
{
    #region Projectile Hit
    public partial class PlayerUnit
    {
        public static float IFrameEndTime = 0f;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Reinitialize()
        {
            IFrameEndTime = 0f;
        }
        [SerializeField] AudioClipWrapper hitSound;
        [SerializeField] TMP_Text hitCounterText;
        [SerializeField] SpriteFlashMaterial flashMaterial;
        int hitCounter = 0;
        protected override bool ProjectileHit(Projectile p)
        {
            if (FactionInterface.IsFriendsWith(p.Faction))
            {
                return false;
            }
            if (Time.time < IFrameEndTime)
                return true;
            SetIFrames(3f, false);
            SpellCardUI.FailSpell();
            hitSound.Play(Center);
            hitCounter++;
            hitCounterText.text = hitCounter.ToString();
            StartCoroutine(CO_PlayerHit());
            return true;
        }
        float DeathBombTime;
        bool bombPressed;
        private bool DeathBombPressed;
        [SerializeField] BremseInputEventBus bombEvent;
        private void SetDeathBombInput()
        {
            DeathBombPressed = true;
        }
        private void ReleaseDeathBombInput()
        {
            DeathBombPressed = false;
        }
        public static void SetIFrames(float duration, bool flashSprite)
        {
            IFrameEndTime = Time.time + duration;
            if (flashSprite)
            {
                ((PlayerUnit)Player).flashMaterial.TriggerFlashMaterial(duration);
            }
        }
        IEnumerator CO_PlayerHit()
        {
            Time.timeScale = 0.03f;
            DeathBombTime = Time.unscaledTime + 0.35f;
            bool deathBombed = false;
            while (Time.unscaledTime <= DeathBombTime && !deathBombed)
            {
                yield return null;
                if ((DeathBombPressed && PlayerBombAction.CanBomb) || PlayerBombAction.BombIframesTime >= Time.time)
                {
                    Time.timeScale = 1f;
                    SetIFrames(4f, true);
                    deathBombed = true;
                    yield break;
                }
            }
            Time.timeScale = 1f;
            SetIFrames(4f, true);
        }
    }
    #endregion
    #region Shoot Projectile
    public partial class PlayerUnit
    {
        [SerializeField] AimAssist testAimProfile;
        [SerializeField] UnitAttack TestAttack;
        [SerializeField] UnitAttack TestFocusAttack;
        bool attackHeld;
        float nextAttackTime;
        [SerializeField] float fireRate = 10f;
        private void BindInput()
        {
            PlayerInputController.actions.Player.Fire.started += PressAttackDown;
            PlayerInputController.actions.Player.Fire.canceled += PressAttackUp;
        }
        private void ClearInput()
        {
            PlayerInputController.actions.Player.Fire.started -= PressAttackDown;
            PlayerInputController.actions.Player.Fire.canceled -= PressAttackUp;
        }
        private void PressAttackUp(InputAction.CallbackContext c)
        {
            attackHeld = false;
        }
        private void PressAttackDown(InputAction.CallbackContext c)
        {
            attackHeld = true;
        }
        private void AttackUpdate()
        {
            if (!attackHeld)
            {
                return;
            }
            if (Time.time < nextAttackTime)
            {
                return;
            }
            nextAttackTime = Time.time + (1f / fireRate).Max(0.05f);
            BaseUnit aimTarget = null;
            if (testAimProfile && testAimProfile.FetchTarget(this, out BaseUnit t))
            {
                aimTarget = t;
                SetTarget(aimTarget);
            }
            else
            {
                SetTarget(null);
            }
            Vector2 target = aimTarget ? aimTarget.Center : Up;
            if (Focused && TestFocusAttack != null)
            {
                TestFocusAttack.AttackTarget(this, Center, target);
            }
            else
            {
                if (TestAttack == null)
                {
                    return;
                }
                TestAttack.AttackTarget(this, Center, target);
            }
        }
    }
    #endregion
    #region Movement
    public partial class PlayerUnit : BaseUnit
    {
        bool focusHeld;
        public bool Focused => focusHeld;
        [SerializeField] UnitMotor sneakMotor;
        public override UnitMotor ActiveMotor => Focused ? sneakMotor : standardMotor;
        private void ReadInput(ref Vector2 input)
        {
            /*Vector2 positionDelta = RenderTextureCursorHandler.CursorPosition - Center;
            if (positionDelta.magnitude < 0.02f || !RenderTextureCursorHandler.IsHovering)
            {
                input = Vector2.zero;
                return;
            }
            input = positionDelta.normalized;
            return;
            */
            input = PlayerInputController.actions == null ? Vector2.zero : PlayerInputController.actions.Player.Move.ReadValue<Vector2>();
        }
        private void ApplyInput(Vector2 input)
        {
            if (ActiveMotor == null)
                return;
            Move(ActiveMotor, input);
        }
    }
    #endregion
    public partial class PlayerUnit : BaseUnit
    {
        Vector2 input = new();
        private void OnDestroy()
        {
            ClearInput();
        }
        protected override void WhenAwake()
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody2D>();
            }
            BindInput();
            Player = this;
        }
        protected override void WhenStart()
        {
            DirectionSolver.SetKnownTarget(transform);
            bombEvent.BindAction(BremseInputPhase.Performed, SetDeathBombInput);
            bombEvent.BindAction(BremseInputPhase.Cancelled, ReleaseDeathBombInput);
        }
        protected override void WhenDestroy()
        {
            bombEvent.ReleaseAction(BremseInputPhase.Performed, SetDeathBombInput);
            bombEvent.ReleaseAction(BremseInputPhase.Cancelled, ReleaseDeathBombInput);
        }
        private void Update()
        {
            ReadInput(ref input);
            ApplyInput(input);
            focusHeld = PlayerInputController.actions.Player.Focus.ReadValue<float>() > 0.5f || PlayerInputController.actions.Shmup.Focus.ReadValue<float>() > 0.5f;
            ProjectileScanOverlap();
        }
        private void ProjectileScanOverlap()
        {
            foreach (var item in Physics2D.BoxCastAll(Center, Vector2.one * 0.02f, 0f, Vector2.zero, 0f, 1 << 7))
            {
                if (item.transform.GetComponent<Projectile>() is Projectile p and not null && !FactionInterface.IsFriendsWith(p.Faction))
                {
                    if (p.Contains(Center))
                    {
                        p.CollideWith(Collider);
                        //p.ClearProjectile();
                    }
                }
            }
        }
        private void LateUpdate()
        {
            AttackUpdate();
        }
        private void OnApplicationQuit()
        {
            Debug.Log("Projectile Count at Game Close : "+ Projectile.CountProjectiles);
        }
    }
}

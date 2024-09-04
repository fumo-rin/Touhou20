using Bremsengine;
using Core.Extensions;
using Core.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace BremseTouhou
{
    #region Projectile Hit
    public partial class PlayerUnit
    {
        protected override bool ProjectileHit(Projectile p)
        {
            if (FactionInterface.IsFriendsWith(p.Faction))
            {
                return false;
            }
            Debug.Log("Player Hit : " + p);
            return true;
        }
    }
    #endregion
    #region Shoot Projectile
    public partial class PlayerUnit
    {
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
            ShootProjectile(testProjectile, UnitCenter + Vector2.up.Shift(5f));
        }
    }
    #endregion
    public partial class PlayerUnit : BaseUnit
    {
        [SerializeField] ProjectileSO testProjectile;
        private void Awake()
        {
            BindInput();
        }
        private void OnDestroy()
        {
            ClearInput();
        }
        private void Update()
        {
            AttackUpdate();
        }
    }
}

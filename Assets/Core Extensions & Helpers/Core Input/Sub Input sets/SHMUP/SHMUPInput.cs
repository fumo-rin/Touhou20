using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Core.Input
{
    public class SHMUPInput : MonoBehaviour
    {
        [SerializeField] BremseInputEventBus Focus;
        [SerializeField] BremseInputEventBus Shoot;
        [SerializeField] BremseInputEventBus AutoFire;
        [SerializeField] BremseInputEventBus Berserk;
        [SerializeField] BremseInputEventBus Bomb;
        [SerializeField] BremseInputEventBus WeaponNext;
        [SerializeField] BremseInputEventBus WeaponPrevious;
        private void Awake()
        {
            PlayerInputController.actions.Shmup.Focus.performed += Focus.TriggerPerformed;
            PlayerInputController.actions.Shmup.Focus.started += Focus.TriggerJustPressed;
            PlayerInputController.actions.Shmup.Focus.canceled += Focus.TriggerCancelled;

            PlayerInputController.actions.Shmup.Fire.performed += Shoot.TriggerPerformed;
            PlayerInputController.actions.Shmup.Fire.started += Shoot.TriggerJustPressed;
            PlayerInputController.actions.Shmup.Fire.canceled += Shoot.TriggerCancelled;

            PlayerInputController.actions.Shmup.Autofire.performed += AutoFire.TriggerPerformed;
            PlayerInputController.actions.Shmup.Autofire.started += AutoFire.TriggerJustPressed;
            PlayerInputController.actions.Shmup.Autofire.canceled += AutoFire.TriggerCancelled;

            PlayerInputController.actions.Shmup.Ability1.performed += Berserk.TriggerPerformed;
            PlayerInputController.actions.Shmup.Ability1.started += Berserk.TriggerJustPressed;
            PlayerInputController.actions.Shmup.Ability1.canceled += Berserk.TriggerCancelled;

            PlayerInputController.actions.Shmup.Bomb.performed += Bomb.TriggerPerformed;
            PlayerInputController.actions.Shmup.Bomb.started += Bomb.TriggerJustPressed;
            PlayerInputController.actions.Shmup.Bomb.canceled += Bomb.TriggerCancelled;

            PlayerInputController.actions.Shmup.WeaponNext.performed += WeaponNext.TriggerPerformed;
            PlayerInputController.actions.Shmup.WeaponNext.started += WeaponNext.TriggerJustPressed;
            PlayerInputController.actions.Shmup.WeaponNext.canceled += WeaponNext.TriggerCancelled;

            PlayerInputController.actions.Shmup.WeaponPrevious.performed += WeaponPrevious.TriggerPerformed;
            PlayerInputController.actions.Shmup.WeaponPrevious.started += WeaponPrevious.TriggerJustPressed;
            PlayerInputController.actions.Shmup.WeaponPrevious.canceled += WeaponPrevious.TriggerCancelled;
        }
        private void OnDestroy()
        {
            PlayerInputController.actions.Shmup.Focus.performed -= Focus.TriggerPerformed;
            PlayerInputController.actions.Shmup.Focus.started -= Focus.TriggerJustPressed;
            PlayerInputController.actions.Shmup.Focus.canceled -= Focus.TriggerCancelled;

            PlayerInputController.actions.Shmup.Fire.performed -= Shoot.TriggerPerformed;
            PlayerInputController.actions.Shmup.Fire.started -= Shoot.TriggerJustPressed;
            PlayerInputController.actions.Shmup.Fire.canceled -= Shoot.TriggerCancelled;

            PlayerInputController.actions.Shmup.Autofire.performed -= AutoFire.TriggerPerformed;
            PlayerInputController.actions.Shmup.Autofire.started -= AutoFire.TriggerJustPressed;
            PlayerInputController.actions.Shmup.Autofire.canceled -= AutoFire.TriggerCancelled;

            PlayerInputController.actions.Shmup.Ability1.performed -= Berserk.TriggerPerformed;
            PlayerInputController.actions.Shmup.Ability1.started -= Berserk.TriggerJustPressed;
            PlayerInputController.actions.Shmup.Ability1.canceled -= Berserk.TriggerCancelled;

            PlayerInputController.actions.Shmup.Bomb.performed -= Bomb.TriggerPerformed;
            PlayerInputController.actions.Shmup.Bomb.started -= Bomb.TriggerJustPressed;
            PlayerInputController.actions.Shmup.Bomb.canceled -= Bomb.TriggerCancelled;

            PlayerInputController.actions.Shmup.WeaponNext.performed -= WeaponNext.TriggerPerformed;
            PlayerInputController.actions.Shmup.WeaponNext.started -= WeaponNext.TriggerJustPressed;
            PlayerInputController.actions.Shmup.WeaponNext.canceled -= WeaponNext.TriggerCancelled;

            PlayerInputController.actions.Shmup.WeaponPrevious.performed -= WeaponPrevious.TriggerPerformed;
            PlayerInputController.actions.Shmup.WeaponPrevious.started -= WeaponPrevious.TriggerJustPressed;
            PlayerInputController.actions.Shmup.WeaponPrevious.canceled -= WeaponPrevious.TriggerCancelled;
        }
    }
}

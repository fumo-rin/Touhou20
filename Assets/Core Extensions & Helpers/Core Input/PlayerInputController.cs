using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Input
{
    public enum ControlScheme
    {
        None,
        PC,
        Gamepad
    }
    [DefaultExecutionOrder(-50)]
    public class PlayerInputController : MonoBehaviour
    {
        public static Vector2 RightStickDirection => ReadStickDirection();
        private static Vector2 ReadStickDirection()
        {
            Vector2 direction = actions.Player.RightStick.ReadValue<Vector2>();
            if (direction != Vector2.zero)
            {
                storedRightStickDirectionInput = direction;
            }
            return direction;
        }
        public static Vector2 LastRightStickDirectionInput => ReadStickDirection() == Vector2.zero ? storedRightStickDirectionInput : RightStickDirection;
        private static Vector2 storedRightStickDirectionInput;
        public static GameActions actions { get; private set; }
        public static ControlScheme ControlScheme { get; private set; } = ControlScheme.None;
        [SerializeField] UnityEngine.InputSystem.PlayerInput playerInput;
        public delegate void ControlSchemeEvent(ControlScheme scheme);
        public static ControlSchemeEvent OnControlSchemeChanged;
        private void Awake()
        {
            playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
            actions ??= new();
            actions?.Enable();
        }
        private void Start()
        {
            SetControlScheme(playerInput);
        }
        public static void SetControlScheme(UnityEngine.InputSystem.PlayerInput input)
        {
            if (input.currentControlScheme.Equals("Gamepad"))
            {
                ControlScheme = ControlScheme.Gamepad;
                OnControlSchemeChanged?.Invoke(ControlScheme);
            }
            else
            {
                ControlScheme = ControlScheme.PC;
                OnControlSchemeChanged?.Invoke(ControlScheme);
            }
        }
        private void OnEnable()
        {
            actions ??= new();
            actions?.Enable();
            playerInput.onControlsChanged += SetControlScheme;
        }
        private void OnDisable()
        {
            actions ??= new();
            actions?.Disable();
            playerInput.onControlsChanged -= SetControlScheme;
        }
    }
}
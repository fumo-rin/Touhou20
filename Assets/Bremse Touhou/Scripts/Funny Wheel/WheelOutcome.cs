using Bremsengine;
using Core.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace BremseTouhou
{
    public abstract class WheelOutcome : MonoBehaviour
    {
        [SerializeField] AudioClipWrapper SelectionSound;
        [SerializeField] Transform UIObject;
        [SerializeField] Image UIIcon;
        [SerializeField] Sprite iconSprite;
        [SerializeField] string tooltip = "Default Tooltip";
        public string Tooltip => tooltip;
        private void Start()
        {
            UIIcon.sprite = iconSprite;
        }
        public void PlaySound()
        {
            SelectionSound.Play(DirectionSolver.GetPaddedBounds(0f).center);
        }
        public abstract float GetDuration();
        public abstract void ApplyEffect(BaseUnit unit);
        public abstract void RemoveEffect(BaseUnit unit);
        public abstract void GameReset(BaseUnit unit);
    }
}

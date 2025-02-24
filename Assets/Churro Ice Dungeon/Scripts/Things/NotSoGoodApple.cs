using UnityEngine;

namespace ChurroIceDungeon
{
    public class NotSoGoodApple : GroundItem
    {
        [SerializeField] SpriteRenderer modeDisplay;
        protected override bool OnTouch(Collider2D other, object playerData)
        {
            SetHardMode(!modeDisplay.enabled);
            return true;
        }
        private void Start()
        {
            SetMode(ChurroManager.HardMode);
        }
        [QFSW.QC.Command("Hardmode")]
        public static void SetMode(bool state)
        {
            ChurroManager.HardMode = state;
            ChurroManager.RequestStatsRefresh();
        }
        public void SetHardMode(bool mode)
        {
            modeDisplay.enabled = mode;
            SetMode(mode);
        }
    }
}
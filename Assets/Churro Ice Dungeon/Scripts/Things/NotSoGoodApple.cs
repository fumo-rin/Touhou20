using UnityEngine;

namespace ChurroIceDungeon
{
    public class NotSoGoodApple : GroundItem
    {
        [SerializeField] SpriteRenderer modeDisplay;
        protected override bool OnTouch(Collider2D other, object playerData)
        {
            SetMode(!modeDisplay.enabled);
            return true;
        }
        private void Start()
        {
            SetMode(ChurroManager.HardMode);
        }
        [QFSW.QC.Command("Hardmode")]
        public void SetMode(bool state)
        {
            modeDisplay.enabled = state;
            ChurroManager.HardMode = state;
            ChurroManager.RequestStatsRefresh();
        }
    }
}
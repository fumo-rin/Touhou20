using UnityEngine;

namespace ChurroIceDungeon
{
    public class NotSoGoodApple : GroundItem
    {
        [SerializeField] SpriteRenderer modeDisplay;
        [SerializeField] GameObject[] hardmodeDisplayOn;
        [SerializeField] GameObject[] hardmodeDisplayOff;
        protected override bool OnTouch(Collider2D other, object playerData)
        {
            SetHardMode(!modeDisplay.enabled);
            return true;
        }
        protected override void WhenStart()
        {
            SetHardMode(ChurroManager.HardMode);
        }
        public static void SetMode(bool state)
        {
            ChurroManager.HardMode = state;
            ChurroManager.RequestStatsRefresh();
        }
        public void SetHardMode(bool mode)
        {
            modeDisplay.enabled = mode;
            SetMode(mode);
            foreach (GameObject go in hardmodeDisplayOn)
            {
                go.SetActive(mode == true);
            }
            foreach (GameObject go in hardmodeDisplayOff)
            {
                go.SetActive(mode == false);
            }
        }
    }
}
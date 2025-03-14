using Bremsengine;
using UnityEngine;
using TMPro;

namespace ChurroIceDungeon
{
    public class ChurroProjectileCountDisplay : MonoBehaviour
    {
        [SerializeField] TMP_Text bulletCountText;
        private void OnEnable()
        {
            TickManager.MainTickLightweight += RefreshUI;
        }
        private void OnDisable()
        {
            TickManager.MainTickLightweight -= RefreshUI;
        }
        public void RefreshUI()
        {
            if (bulletCountText == null)
                return;
            bulletCountText.text = string.Intern("Bullet Count: ") + ChurroProjectile.BulletCount.ToString();
        }
    }
}

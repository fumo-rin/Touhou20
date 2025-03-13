using UnityEngine;

namespace ChurroIceDungeon
{
    public class ChurroProjectileUpdater : MonoBehaviour
    {
        private void Update()
        {
            ChurroProjectile.RunActiveBullets();
        }
        private void LateUpdate()
        {
            ChurroProjectile.LateRunActiveBullets(1f);
        }
    }
}

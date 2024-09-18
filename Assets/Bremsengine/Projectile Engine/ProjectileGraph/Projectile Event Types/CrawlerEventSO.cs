using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bremsengine
{
    #region Editor
#if UNITY_EDITOR
    public partial class CrawlerEventSO
    {
        public override string GetGraphComponentName()
        {
            return "Projectile Crawler Event";
        }

        public override void OnDrag(Vector2 delta)
        {

        }

        protected override Rect GetRect(Vector2 mousePosition)
        {
            return new(mousePosition.x, mousePosition.y, 200f, 300f);
        }

        protected override void OnDraw(GUIStyle style)
        {

        }
        protected override void OnInitialize(Vector2 mousePosition, ProjectileGraphSO graph, ProjectileTypeSO type)
        {

        }
    }
#endif
    #endregion
    public partial class CrawlerEventSO : ProjectileEventSO
    {
        protected override void TriggerEvent(Projectile p, TriggeredEvent e)
        {
            Debug.Log("Trigger Crawler Event");
        }
    }
}

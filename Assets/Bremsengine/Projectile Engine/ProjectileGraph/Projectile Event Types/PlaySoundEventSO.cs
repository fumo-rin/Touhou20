using Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Bremsengine
{
    #region Editor
#if UNITY_EDITOR
    public partial class PlaySoundEventSO
    {
        public override string GetEventName()
        {
            return $"(Delay:{EventDelay}){(acw == null ? "None":acw.name)}";
        }
        protected override Rect GetRect(Vector2 mousePosition)
        {
            return new Rect(mousePosition.x, mousePosition.y, 200f, 50f);
        }

        protected override void OnDraw(GUIStyle style)
        {
            EditorGUI.BeginChangeCheck();

            AudioClipWrapper old = acw;
            acw = (AudioClipWrapper)EditorGUI.ObjectField(new(10f, 10f, 150f, 20f), acw, typeof(AudioClipWrapper), false);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssetIfDirty(this);
            }
            if (acw != old)
            {
                Reinitialize();
            }
        }
        protected override void OnInitialize(Vector2 mousePosition, ProjectileGraphSO graph)
        {

        }
    }
#endif
    #endregion
    public partial class PlaySoundEventSO : ProjectileEventSO
    {
        public AudioClipWrapper acw;
        protected override void TriggerEvent(Projectile p, TriggeredEvent t)
        {
            if (t.HasPlayed(acw))
                return;
            t.PlaySound(p.Position, acw);
        }
    }
}

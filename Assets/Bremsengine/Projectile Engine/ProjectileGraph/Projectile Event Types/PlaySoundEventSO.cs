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
        public override string GetGraphComponentName()
        {
            return $"(Delay:{EventDelay}){(acw == null ? "None" : acw.name)}";
        }

        public override void OnDrag(Vector2 delta)
        {

        }

        protected override Rect GetRect(Vector2 mousePosition)
        {
            return new Rect(mousePosition.x, mousePosition.y, 350f, 100f);
        }

        protected override void OnDraw(GUIStyle style)
        {
            EditorGUI.BeginChangeCheck();
            AddSpace(4);
            base.OnDraw(style);
            AudioClipWrapper old = acw;
            acw = (AudioClipWrapper)EditorGUILayout.ObjectField("Sound Effect", acw, typeof(AudioClipWrapper), false);

            repeatCooldown = EditorGUILayout.Slider(repeatCooldown, 0f, 1f);

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
        protected override void OnInitialize(Vector2 mousePosition, ProjectileGraphSO graph, ProjectileTypeSO type)
        {
            rect = GetRect(mousePosition);
        }
    }
#endif
    #endregion
    public partial class PlaySoundEventSO : ProjectileEventSO
    {
        public AudioClipWrapper acw;
        public float repeatCooldown = 0.03f;
        const string LastPlayedSoundKey = "LastPlayedSoundTime";
        protected override void TriggerEvent(Projectile p, TriggeredEvent t)
        {
            if (t.HasPlayed(acw))
                return;


            if (t.keyfloats.ContainsKey(LastPlayedSoundKey) && t.keyfloats[LastPlayedSoundKey] + repeatCooldown > Time.time)
            {
                return;
            }

            t.keyfloats[LastPlayedSoundKey] = Time.time;
            t.PlaySound(p.Position, acw);
        }
    }
}

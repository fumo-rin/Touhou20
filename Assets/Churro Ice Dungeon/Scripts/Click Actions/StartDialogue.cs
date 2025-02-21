using Bremsengine;
using UnityEngine;

namespace ChurroIceDungeon
{
    public class StartDialogue : RenderTextureClickAction
    {
        [SerializeField] Dialogue d;
        public override void ClickPayload()
        {
            d.StartDialogue();
        }
    }
}

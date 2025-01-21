using UnityEngine;
using UnityEngine.Playables;

namespace Bremsengine
{
    public class AttackTimelineClip : PlayableAsset
    {
        [Header("Attack Override")]
        [SerializeField] ProjectileGraphSO containedAttack;
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<AttackTimelineBehaviour>.Create(graph);

            AttackTimelineBehaviour attack = playable.GetBehaviour();
            attack.attack = containedAttack;
            return playable;
        }
    }
}

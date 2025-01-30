using UnityEngine;
using UnityEngine.Timeline;
using System.Collections.Generic;
using Bremsengine;
using UnityEngine.Playables;
namespace BremseTouhou
{
    [System.Serializable]
    public struct PhaseEntry
    {
        public PlayableDirector timeline;
        public ProjectileGraphSO mainAttack;
        public float MaxHealth;
        public float phaseBonus;
        public float phaseBonusDecay;
        public bool MoveToCenter;
        public float GraceTimer;
        //ADD A GRACE TIMER AND RESET THE ATTACK TIME
        //MOVE THE BOSS TO CENTER
    }
    public interface DeathPhase
    {
        public bool ShouldDie();
        public void SetNextPhase();
        public PhaseEntry GetPhase();
        public bool IsLastPhase();
    }
    public class UnitPhase : MonoBehaviour, DeathPhase
    {
        [SerializeField] ProjectileAttack attack;
        [SerializeField] BaseUnit owner;
        PlayableDirector activeTimeline;
        public int phaseIndex;
        [SerializeField] List<PhaseEntry> phaseList = new();
        private void Start()
        {
            owner.SetPhaseHandler((DeathPhase)this);
            if (phaseList.Count == 0) { return; }
            PhaseEntry p = phaseList[0];
            if (p.mainAttack == null)
                return;
            SetOwnerPhase(p);
            foreach (var item in phaseList)
            {
                item.timeline.playOnAwake = false;
            }
        }
        private void OnDestroy()
        {

        }
        private void SendToSpellcardInfo(ProjectileGraphSO g)
        {
            string spellcardName = g.projectileGraphName;
        }
        private void SetOwnerPhase(PhaseEntry phase)
        {
            attack.SetAttackGraph(phase.mainAttack);
            if (!IsLastPhase())
            {
                owner.SetNewHealth(phase.MaxHealth, phase.MaxHealth);
            }
            if (activeTimeline != null)
            {
                activeTimeline.Stop();
            }
            activeTimeline = phase.timeline;
            phase.timeline.time = 0f;
            phase.timeline.Play();
            SpellCardUI.SetPhase(phase);
        }
        public bool IsLastPhase()
        {
            return !(phaseIndex < phaseList.Count);
        }
        public PhaseEntry GetPhase()
        {
            return phaseList[phaseIndex];
        }
        public void SetNextPhase()
        {
            phaseIndex++;
            PhaseEntry e = phaseList[phaseIndex % phaseList.Count];
            SetOwnerPhase(e);
        }
        public bool ShouldDie()
        {
            return IsLastPhase();
        }
    }
}

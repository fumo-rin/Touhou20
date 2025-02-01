using UnityEngine;
using UnityEngine.Timeline;
using System.Collections.Generic;
using Bremsengine;
using UnityEngine.Playables;
using Core.Extensions;
using System.Collections;
namespace BremseTouhou
{
    [System.Serializable]
    public class PhaseEntry
    {
        public UnitPhase Phase => phase;
        public PlayableDirector timeline;
        UnitPhase phase;
        public ProjectileGraphSO mainAttack;
        public float MaxHealth;
        public float phaseBonus;
        public float phaseBonusIncrease;
        public float phaseBonusIncreaseDelay;
        public float phaseLength;
        public bool MoveToCenter;
        public float GraceTimer;
        public bool IsSpellCard;
        public float verticalCenterOffset = 4f;
        bool isLastPhase;
        [field: SerializeField] public MusicWrapper phaseMusic { get; private set; }
        public void SetLastPhase(bool state)
        {
            isLastPhase = state;
        }
        public bool IsLastPhase => isLastPhase;
        //Lock the movement if under grace timer.
        public void SetPhase(UnitPhase p)
        {
            phase = p;
        }
        public IEnumerator CO_CenterBoss(Transform boss, float duration, float speed, float verticalCenterOffset)
        {
            if (boss == null)
                yield break;

            float lerp = 0f;
            Vector2 startPosition = boss.position;
            Vector2 centerTarget = (Vector2)DirectionSolver.GetPaddedBounds(0f).center + new Vector2(0f, verticalCenterOffset);
            while (lerp <= 1f)
            {
                lerp += Time.deltaTime * speed;
                if (boss is Transform t and not null)
                {
                    t.position = Vector2.Lerp(startPosition, centerTarget, (Mathf.Sqrt(lerp).Clamp(0f, 1f)));
                    if (t.GetComponent<Rigidbody2D>() is Rigidbody2D rb and not null)
                    {
                        rb.linearVelocity *= 0f;
                    }
                }
                yield return null;
            }
        }
        public void ForceNextPhase()
        {
            if (phase is DeathPhase p)
            {
                if (!p.IsLastPhase())
                {
                    phase.Unit.ForceKill();
                }
            }
        }
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
        public BaseUnit Unit => owner;
        public ProjectileAttack Attack => attack;
        [SerializeField] ProjectileAttack attack;
        [SerializeField] BaseUnit owner;
        PlayableDirector activeTimeline;
        public int phaseIndex = 0;
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
                item.SetPhase(this);
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
            if (IsLastPhase())
            {
                phase.SetLastPhase(true);
            }
            if (phase.phaseMusic)
            {
                phase.phaseMusic.Play();
            }
            attack.SetAttackGraph(phase.mainAttack);
            if (!IsLastPhase())
            {
                owner.SetNewHealth(phase.MaxHealth, phase.MaxHealth);
            }
            else
            {
                return;
            }
            if (activeTimeline != null)
            {
                activeTimeline.Stop();
            }
            attack.Handler.ForceSetNewAttackDelay(phase.GraceTimer);
            if (phase.MoveToCenter)
            {
                StartCoroutine(phase.CO_CenterBoss(owner.transform, 1.5f.Min(phase.GraceTimer), 1.5f, phase.verticalCenterOffset));
            }
            activeTimeline = phase.timeline;
            phase.timeline.time = 0f;
            phase.timeline.Play();
            SpellCardUI.SetPhase(phase);
            if (!phase.IsSpellCard && SpellCardUI.activeRunner != null)
            {
                SpellCardUI.activeRunner.RecalculatePhaseVisibility();
            }
            if (PhaseSliderValues(out int total, out int phasesLeft))
            {
                SpellCardUI.SetPhaseSlider(total, phasesLeft);
            }
        }
        public bool IsLastPhase()
        {
            return !(phaseIndex < phaseList.Count);
        }
        public PhaseEntry GetPhase()
        {
            return phaseList[phaseIndex];
        }
        private bool PhaseSliderValues(out int total, out int phasesLeft)
        {
            total = phaseList.Count;
            phasesLeft = phaseList.Count - phaseIndex;
            return true;
        }
        public void SetNextPhase()
        {
            phaseIndex++;
            PhaseEntry e = phaseList[phaseIndex % phaseList.Count];
            if (IsLastPhase())
            {
                SpellCardUI.HideUI();
                return;
            }
            SetOwnerPhase(e);
        }
        public bool ShouldDie()
        {
            return IsLastPhase();
        }
    }
}

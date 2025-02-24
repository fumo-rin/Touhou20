using Bremsengine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ChurroIceDungeon
{
    public class UltraFrog : MonoBehaviour
    {
        [System.Serializable]
        public struct Phase
        {
            public string anyname;
            public DungeonMotor motor;
            public ChurroBaseAttack[] activeObjects;
            public StrafeProfile profile;
            public float phaseDuration;
            public float stallTime;
        }
        [SerializeField] List<Phase> phases = new();
        public int phaseIndex;
        [SerializeField] EnemyUnit owner;
        [SerializeField] StrafeProfile originalStrafeProfile;
        [SerializeField] AttackHandler handler;
        [SerializeField] string mainMenuString = "Tewis Pipebomb Room";
        public void EnablePhase(Phase p)
        {
            if (phaseIndex >= phases.Count)
            {
                Kill();
                GeneralManager.LoadSceneAfterDelay(mainMenuString, 3f);
            }
            IEnumerator CO_Phase(Phase p)
            {
                void DisableAllObjects()
                {
                    foreach (var item in phases)
                    {
                        foreach (var obj in item.activeObjects)
                        {
                            obj.gameObject.SetActive(false);
                        }
                    }
                }
                DisableAllObjects();
                if (owner is EnemyUnit enemy)
                {
                    if (p.profile)
                    {
                        enemy.SetStrafeProfile(p.profile);
                    }
                    else
                    {
                        enemy.SetStrafeProfile(originalStrafeProfile);
                        enemy.OverrideMotor = null;
                    }
                    if (p.motor != null)
                    {
                        enemy.OverrideMotor = p.motor;
                    }
                    else
                    {
                        enemy.OverrideMotor = null;
                    }
                }
                handler.settings.SetNewAttackTime(2f);
                yield return new WaitForSeconds(1f);
                handler.settings.SetNewAttackTime(p.stallTime);
                foreach (var item in p.activeObjects)
                {
                    item.SetHandler(handler);
                    item.gameObject.SetActive(true);
                }
                phaseIndex++;
                BossTimer.SetTimer(p.phaseDuration);
                yield return new WaitForSeconds(p.phaseDuration);
                EnablePhase(phaseIndex);
            }
            StartCoroutine(CO_Phase(p));
        }
        private void EnablePhase(int i)
        {
            if (i >= phases.Count)
            {
                Kill();
                GeneralManager.LoadSceneAfterDelay(mainMenuString, 3f);
                return;
            }
            EnablePhase(phases[i]);
        }
        private void Kill()
        {
            owner.ExternalKill();
        }
        private void Start()
        {
            EnablePhase(phaseIndex);
            NotSoGoodApple.SetMode(true);
        }
    }
}

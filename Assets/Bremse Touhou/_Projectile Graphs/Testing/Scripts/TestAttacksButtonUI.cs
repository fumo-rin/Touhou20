using Bremsengine;
using System.Collections.Generic;
using UnityEngine;
using Core.Extensions;
using TMPro;

namespace BremseTouhou
{
    public class TestAttacksButtonUI : MonoBehaviour
    {
        HashSet<ProjectileAttack> attacks = new();
        ProjectileGraphSO lastSelectedAttack;
        [SerializeField] TestAttackButton buttonPrefab;
        [SerializeField] List<ProjectileGraphSO> knownAttacks;
        [SerializeField] Transform buttonsAnchor;
        [SerializeField] Transform toggleBox;
        [SerializeField] GameObject skeletron;
        [SerializeField] TargetDummyDPS targetDummy;
        [SerializeField] TMP_InputField dummyEntriesCountField;
        [SerializeField] HashSet<GameObject> skeletrons = new();
        TargetDummyDPS activeTargetDummy;
        public void ToggleUI()
        {
            toggleBox.gameObject.SetActive(!toggleBox.gameObject.activeInHierarchy);
        }
        public void CloseUI() { toggleBox.gameObject.SetActive(false); }
        public void BuildButtons()
        {
            for (int i = 0; i < buttonsAnchor.childCount; i++)
            {
                Destroy(buttonsAnchor.GetChild(i).gameObject);
            }
            foreach (var i in knownAttacks)
            {
                TestAttackButton spawnedButton = Instantiate(buttonPrefab, buttonsAnchor);
                spawnedButton.Bind(i, this);
            }
        }
        public void SpawnSkeletron()
        {
            if (activeTargetDummy != null)
            {
                Destroy(activeTargetDummy.gameObject);
                activeTargetDummy = null;
            }
            GameObject s = Instantiate(skeletron, StageWorldCenter.Center + new Vector2(0f, 2f), Quaternion.identity);
            skeletrons.Add(s);
            if (s.TryGetComponent(out ProjectileAttack p))
            {
                attacks.Add(p);
            }
            else
            {
                if (TryGetComponent(out ProjectileAttack pp))
                {
                    attacks.Add(pp);
                }
            }
            SetAttack(lastSelectedAttack);
        }
        public void SpawnTargetDummy()
        {
            if (activeTargetDummy != null)
                return;
            foreach (var i in skeletrons)
            {
                if (i.TryGetComponent(out ProjectileAttack p))
                {
                    attacks.Remove(p);
                }
                else
                {
                    if (TryGetComponent(out ProjectileAttack pp))
                    {
                        attacks.Remove(pp);
                    }
                }
                Destroy(i);
            }
            skeletrons.Clear();
            activeTargetDummy = Instantiate(targetDummy, StageWorldCenter.Center + new Vector2(0f, 2f), Quaternion.identity);
        }
        public void SetTargetDummy()
        {
            if (activeTargetDummy)
            {
                activeTargetDummy.SetMaxDummyEntries(int.Parse(dummyEntriesCountField.text).Clamp(5, 500));
            }
            dummyEntriesCountField.text = "";
        }
        private void Awake()
        {
            CloseUI();
        }
        private void Start()
        {
            BuildButtons();
            SpawnSkeletron();
        }
        public void SetAttack(ProjectileGraphSO graph)
        {
            lastSelectedAttack = graph;
            if (graph != null)
            {
                Projectile.ClearProjectilesOfFaction(BremseFaction.Enemy);
                foreach (var item in attacks)
                {
                    item.SetAttackGraph(graph);
                }
            }
        }
    }
}

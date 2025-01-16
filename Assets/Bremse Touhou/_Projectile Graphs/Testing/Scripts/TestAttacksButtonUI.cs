using Bremsengine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BremseTouhou
{
    public class TestAttacksButtonUI : MonoBehaviour
    {
        public ProjectileAttack attack;
        [SerializeField] TestAttackButton buttonPrefab;
        [SerializeField] List<ProjectileGraphSO> knownAttacks;
        [SerializeField] Transform buttonsAnchor;
        [SerializeField] Transform toggleBox;
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
        private void Awake()
        {
            CloseUI();
        }
        private void Start()
        {
            BuildButtons();
        }
        public void SetAttack(ProjectileGraphSO graph)
        {
            if (graph != null)
            {
                Projectile.ClearProjectilesOfFaction(BremseFaction.Enemy);
                attack.SetAttackGraph(graph);
            }
        }
    }
}

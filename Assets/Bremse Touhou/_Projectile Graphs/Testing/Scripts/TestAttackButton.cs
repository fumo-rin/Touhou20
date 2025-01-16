using Bremsengine;
using TMPro;
using UnityEngine;

namespace BremseTouhou
{
    public class TestAttackButton : MonoBehaviour
    {
        public ProjectileGraphSO attack;
        [SerializeField] TMP_Text buttonText;
        TestAttacksButtonUI boundUI;
        public void Bind(ProjectileGraphSO graph, TestAttacksButtonUI boundUI)
        {
            this.boundUI = boundUI;
            attack = (ProjectileGraphSO)graph;
            buttonText.text = attack.projectileGraphName;
        }
        public void Apply()
        {
            boundUI.SetAttack(this.attack);
        }
    }
}

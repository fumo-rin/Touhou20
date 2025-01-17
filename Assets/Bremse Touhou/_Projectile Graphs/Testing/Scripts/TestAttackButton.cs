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
            string builtText = attack.projectileGraphName;
            if (string.IsNullOrEmpty(builtText))
            {
                builtText = graph.name;
            }
            buttonText.text = builtText;
        }
        public void Apply()
        {
            boundUI.SetAttack(this.attack);
        }
    }
}

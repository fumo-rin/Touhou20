using TMPro;
using UnityEngine;

namespace BremseTouhou
{
    public class GrazeUI : MonoBehaviour
    {
        [SerializeField] TMP_Text grazeText;
        static GrazeUI instance;
        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {
            RebuildGraze(GrazeBox.GrazeCount);
            GrazeBox.OnGraze += RebuildGraze;
        }
        private void OnDestroy()
        {
            GrazeBox.OnGraze -= RebuildGraze;   
        }
        public static void RebuildGraze(int grazeCount)
        {
            if (instance == null || instance.grazeText == null)
                return;
            instance.grazeText.text = grazeCount.ToString();
        }
    }
}

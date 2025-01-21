using TMPro;
using UnityEngine;

namespace Bremsengine
{
    public class CreditsLoader : MonoBehaviour
    {
        [SerializeField] GameCreditsSO gameCredits;
        [SerializeField] TMP_Text creditsText;
        private void Start()
        {
            creditsText.text = gameCredits.CompileCredits();
        }
    }
}

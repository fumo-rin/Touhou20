using TMPro;
using UnityEngine;

namespace Bremsengine
{
    public class CreditsLoader : MonoBehaviour
    {
        [SerializeField] GameCreditsSO gameCredits;
        [SerializeField] TMP_Text creditsText;
        [SerializeField] Rigidbody2D rb;
        [SerializeField] float upwardsForce;
        [SerializeField] GameObject container;
        private void Start()
        {
            creditsText.text = gameCredits.CompileCredits();
        }
        public void StartCredits()
        {
            container.SetActive(true);
            rb.position = transform.position;
            rb.linearVelocity = new(0f, upwardsForce);
        }
        public void EndCredits()
        {
            container.SetActive(false);
        }
    }
}

using Core.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Bremsengine
{
    [RequireComponent(typeof(Button))]
    public class QuitButton : MonoBehaviour
    {
        Button b;
        private void Awake()
        {
             b = GetComponent<Button>();
        }
        private void Start()
        {
            b.AddClickAction(() => Application.Quit());
        }
        private void OnDestroy()
        {
            b.RemoveClickAction(() => Application.Quit());
        }
    }
}

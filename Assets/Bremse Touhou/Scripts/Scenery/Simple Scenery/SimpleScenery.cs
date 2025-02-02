using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace BremseTouhou
{
    [System.Serializable]
    public struct SimpleSceneryEntry
    {
        public List<GameObject> items;
    }
    public class SimpleScenery : MonoBehaviour
    {
        public static SimpleScenery Instance;
        [SerializeField] List<SimpleSceneryEntry> sceneryItems = new();
        private void Awake()
        {
            Instance = this;
            SetScenery(0);
        }
        [QFSW.QC.Command("-set-scenery")]
        public static void SetScenery(int index)
        {
            foreach (var entries in Instance.sceneryItems)
            {
                foreach (var entry in entries.items)
                {
                    entry.SetActive(false);
                }
            }
            foreach(var item in Instance.sceneryItems[index].items)
            {
                item.SetActive(true);
            }
        }
    }
}

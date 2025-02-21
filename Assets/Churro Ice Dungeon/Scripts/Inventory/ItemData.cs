using Core;
using Core.Extensions;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ChurroIceDungeon
{
    #region Item Event Keys
    public enum ItemActionKey
    {
        None = -1,
        Airhorn = 1,
        Pipebomb = 2
    }
    #endregion
    #region Item Events
    public partial class ItemData
    {
        [SerializeField] ItemEvent UseEvent;
        static Dictionary<int, Action> UseLookup;
        [SerializeField] bool clearOnUse;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ReinitializeEvents()
        {
            UseLookup = new Dictionary<int, Action>();
        }
        public static void BindEvent(Action a, ItemActionKey key)
        {
            if (a == null)
            {
                return;
            }
            if (!UseLookup.ContainsKey((int)key))
            {
                UseLookup[(int)key] = null;
            }
            UseLookup[(int)key] += a;
        }
        public static void ReleaseEvent(Action a, ItemActionKey key)
        {
            if (a != null)
            {
                UseLookup[(int)key] -= a;
            }
        }
        [System.Serializable]
        public struct ItemEvent
        {
            public ItemActionKey EventKey;
            public void TriggerEvent()
            {
                Debug.Log(EventKey);
                if (UseLookup.ContainsKey((int)EventKey))
                {
                    UseLookup[(int)EventKey]?.Invoke();
                }
            }
        }
        public enum UseResult
        {
            NoUse,
            UseUnlimited,
            UseLimited
        }
        public bool UseItem(out UseResult result)
        {
            result = UseResult.NoUse;
            if (UseEvent.EventKey == ItemActionKey.None)
                return result != UseResult.NoUse;
            if (clearOnUse)
            {
                result = UseResult.UseLimited;
            }
            else
            {
                result = UseResult.UseUnlimited;
            }
            UseEvent.TriggerEvent();
            return result != UseResult.NoUse;
        }
    }
    #endregion
    #region Item Cache
    public partial class ItemData
    {
        static Dictionary<string, ItemData> cachedItems;
        static Dictionary<int, ItemData> indexCacche;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LoadItemCache()
        {
            cachedItems = new();
            indexCacche = new Dictionary<int, ItemData>();
            AddressablesTools.LoadKeys<ItemData>("All Items", SetCache);
        }
        private static void SetCache(IList<ItemData> items)
        {
            int i = 1;
            foreach (var item in items)
            {
                if (item == null)
                    continue;
                cachedItems[item.itemID] = item;
                indexCacche[i] = item;
                i++;
            }
        }
        public static bool TryGetItemByID(string guid, out ItemData output)
        {
            return cachedItems.TryGetValue(guid, out output);
        }
        public static bool TryGetItemByIndex(int index, out ItemData output)
        {
            return indexCacche.TryGetValue(index, out output);
        }
    }
    #endregion
    #region Editor
#if UNITY_EDITOR

    [CustomEditor(typeof(ItemData))]
    public partial class ItemDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("New ID") && target is ItemData d and not null)
            {
                d.GetNewID();
            }
        }
    }
#endif
    #endregion
    [CreateAssetMenu(fileName = "New Item Data", menuName = "Churro/Item")]
    public partial class ItemData : ScriptableObject
    {
        [SerializeField] ItemSlot.SlotType slotType;
        [SerializeField] string itemID;
        public string ID => itemID;
        public void GetNewID()
        {
            itemID = Guid.NewGuid().ToString();
            this.Dirty();
        }
        [field: SerializeField] public Sprite itemSprite { get; private set; }

        public bool IsOfType(ItemSlot.SlotType type)
        {
            if (slotType == type)
                return true;
            return false;
        }
    }
}

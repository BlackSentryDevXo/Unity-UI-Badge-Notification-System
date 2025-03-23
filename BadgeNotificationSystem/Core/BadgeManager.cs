using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System;

namespace SentryToolkit
{
    public class BadgeManager : MonoBehaviour
    {
        public static BadgeManager Instance;

        private Dictionary<BadgeID, BadgeState> badgeStates; // Stores current badge states
        private Dictionary<BadgeID, List<BadgeID>> childMap; // Maps parent badges to their child badges

        public event Action<BadgeID> OnBadgeStateChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                LoadBadgeStates();
                childMap = new Dictionary<BadgeID, List<BadgeID>>();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void InitializeChildMap(List<BadgeChildMapping> mappings)
        {
            foreach (var mapping in mappings)
            {
                if (!childMap.ContainsKey(mapping.Parent))
                {
                    childMap[mapping.Parent] = new List<BadgeID>();
                }
                childMap[mapping.Parent].AddRange(mapping.Children);
            }
        }

        public void InitializeBadgesWithPropagations()
        {
            // Create a temporary list of keys to iterate over
            List<BadgeID> badgeKeys = new List<BadgeID>(badgeStates.Keys);

            // Iterate over the temporary list
            foreach (var key in badgeKeys)
            {
                if (badgeStates.TryGetValue(key, out var state))
                {
                    SetBadge(key, state);
                }
            }
        }

        public void ClearChildMap()
        {
            childMap.Clear();
        }

        public void SetBadge(BadgeID id, bool saveState = true)
        {
            SetBadge(id, new BadgeState { Type = BadgeType.Symbol }, saveState);
        }

        public void SetBadge(BadgeID id, BadgeState state, bool saveState = true)
        {
            badgeStates[id] = state;
            if (saveState) { SaveBadgeStates(); }
            OnBadgeStateChanged?.Invoke(id);
            PropagateBadgeState(id, saveState);
        }

        public BadgeState GetBadge(BadgeID id)
        {
            if (badgeStates.ContainsKey(id))
            {
                return badgeStates[id];
            }
            return new BadgeState { Type = BadgeType.None };
        }

        public void ClearBadge(BadgeID id)
        {
            if (badgeStates.ContainsKey(id))
            {
                badgeStates.Remove(id);
                SaveBadgeStates();
                OnBadgeStateChanged?.Invoke(id);
                PropagateBadgeState(id, true);
            }
        }

        private void SaveBadgeStates()
        {
            string json = JsonConvert.SerializeObject(badgeStates, Formatting.Indented);
            File.WriteAllText(Application.persistentDataPath + "/badgeStates.json", json);
        }

        private void LoadBadgeStates()
        {
            string path = Application.persistentDataPath + "/badgeStates.json";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                badgeStates = JsonConvert.DeserializeObject<Dictionary<BadgeID, BadgeState>>(json);
            }
            else
            {
                badgeStates = new Dictionary<BadgeID, BadgeState>();
            }
        }

        private void PropagateBadgeState(BadgeID id, bool saveState)
        {
            // Propagate state to children
            if (childMap.TryGetValue(id, out List<BadgeID> childIds))
            {
                foreach (var childId in childIds)
                {
                    PropagateBadgeState(childId, saveState);
                }
            }

            // Check if the current badge should propagate an alert to its parent
            foreach (var kvp in childMap)
            {
                if (kvp.Value.Contains(id))
                {
                    BadgeID parentId = kvp.Key;
                    bool parentShouldHaveBadge = false;

                    // Check if any child badge of the parent has an active badge state
                    foreach (var childId in kvp.Value)
                    {
                        if (badgeStates.ContainsKey(childId) && badgeStates[childId].Type != BadgeType.None)
                        {
                            parentShouldHaveBadge = true;
                            break;
                        }
                    }

                    BadgeState parentState = GetBadge(parentId);
                    if (parentShouldHaveBadge && parentState.Type == BadgeType.None)
                    {
                        parentState.Type = BadgeType.Symbol;
                        SetBadge(parentId, parentState, saveState);
                    }
                    else if (!parentShouldHaveBadge && parentState.Type != BadgeType.None)
                    {
                        ClearBadge(parentId);
                    }
                }
            }
        }
    }
}
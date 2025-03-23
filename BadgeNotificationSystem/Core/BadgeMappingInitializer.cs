using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

namespace SentryToolkit
{
    public class BadgeMappingInitializer : MonoBehaviour
    {
        [SerializeField]
        private List<BadgeChildMapping> childMappings; // Configure in the Inspector

        [Header("Debug")]
        public BadgeID testId;

        [Button]
        public void TriggerBadge()
        {
            BadgeManager.Instance.SetBadge(testId);
        }

        private void Start()
        {
            // Initialize the BadgeManager with mappings when the scene starts
            if (BadgeManager.Instance != null)
            {
                BadgeManager.Instance.InitializeChildMap(childMappings);
                BadgeManager.Instance.InitializeBadgesWithPropagations();
            }
        }

        private void OnDisable()
        {
            // Clear the BadgeManager mappings when the scene is unloaded
            if (BadgeManager.Instance != null)
            {
                BadgeManager.Instance.ClearChildMap();
            }
        }
    }
}
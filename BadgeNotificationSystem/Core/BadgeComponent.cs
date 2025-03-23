using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace SentryToolkit
{
    public class BadgeComponent : MonoBehaviour
    {
        public BadgeID badgeId; // Unique ID for the badge
        public Image badgeSymbolImage; // Image component to display the badge symbol

        public bool autoGetButton = true;
        [DisableIf("autoGetButton")]
        public Button badgeClickButton;
        BadgeManager badgeManager;

        private void Start()
        {
            InitializeBadgeManager();
            if (autoGetButton)
                GetComponent<Button>().onClick.AddListener(OnBadgeClicked);
            else
                badgeClickButton.onClick.AddListener(OnBadgeClicked);
        }

        void InitializeBadgeManager()
        {
            if (!badgeManager)
            {
                badgeManager = BadgeManager.Instance;
            }
        }

        private void OnEnable()
        {
            InitializeBadgeManager();
            StartCoroutine(HandleManagerSubscription());
        }

        private IEnumerator HandleManagerSubscription()
        {
            yield return new WaitForEndOfFrame();
            
            if (badgeSymbolImage != null) badgeSymbolImage.gameObject.SetActive(false);

            if (badgeManager != null) {
                badgeManager.OnBadgeStateChanged += HandleBadgeStateChanged;
            }
            else {
                Debug.LogError("Badge Manager is null");
            }
            UpdateBadge();
        }

        private void OnDisable()
        {
            if (badgeManager != null)
            {
                badgeManager.OnBadgeStateChanged -= HandleBadgeStateChanged;
            }
        }

        public void OnBadgeClicked()
        {
            // Debug.LogError(gameObject.name);
            if (badgeManager != null)
            {
                badgeManager.ClearBadge(badgeId);
            }
            else
            {
                Debug.LogError("Can't find badge manager");
            }
        }

        private void HandleBadgeStateChanged(BadgeID id)
        {
            // Debug.LogError("state has changed");
            UpdateBadge();
        }

        public void UpdateBadge()
        {
            if (badgeManager == null)
            {
                Debug.LogError("Badge Manager is null");
            }

            BadgeState state = badgeManager.GetBadge(badgeId);
            // Debug.LogError($"{gameObject.name} badge state is {state.Type.ToString()} & the active status is {state.Type != BadgeType.None}");
            if (badgeSymbolImage == null) { Debug.LogError($"{gameObject.name} doesn't have a badge symbol image"); return; }
            // Activate or deactivate the symbol image based on badge state
            badgeSymbolImage.gameObject.SetActive(state.Type != BadgeType.None);
        }
    }
}
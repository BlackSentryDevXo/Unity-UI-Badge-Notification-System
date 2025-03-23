using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace SentryToolkit
{
    public class BadgeMappingTrigger : MonoBehaviour
    {
        [SerializeField] private List<BadgeChildMapping> childMappings; // Configure in Inspector
        [SerializeField] private TMP_Text triggerText; // Text to display triggered badge
        [SerializeField] private TMP_Text countdownText; // Countdown display

        private void Start()
        {
            StartCoroutine(TriggerBadgesSequentially());
        }

        private IEnumerator TriggerBadgesSequentially()
        {
            foreach (var mapping in childMappings)
            {
                // Trigger the parent badge
                yield return StartCoroutine(TriggerBadgeWithCountdown(mapping.Parent));

                // Trigger each child badge
                foreach (var child in mapping.Children)
                {
                    yield return StartCoroutine(TriggerBadgeWithCountdown(child));
                }
            }

            // Reset UI text when done
            triggerText.text = "Done!";
            countdownText.text = "";
        }

        private IEnumerator TriggerBadgeWithCountdown(BadgeID badgeId)
        {
            triggerText.text = $"Triggering: {badgeId}";

            for (int i = 5; i > 0; i--)
            {
                countdownText.text = $"In: {i}s";
                yield return new WaitForSeconds(1f);
            }

            yield return new WaitForSeconds(0.5f);
            BadgeManager.Instance.SetBadge(badgeId);
            countdownText.text = "Triggering next...";
            yield return new WaitForSeconds(1f);
        }
    }
}

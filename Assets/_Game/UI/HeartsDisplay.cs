using UnityEngine;
using WhereAreMyKeys.Combat;

namespace WhereAreMyKeys.UI
{
    /// <summary>
    /// Hearts are invisible by default, fading in on damage/heal and while
    /// enemies are engaged, fading out after combat (GDD section 8). Kept
    /// as hearts, not pure vignette, so a child can check how close to
    /// death they are.
    /// </summary>
    public class HeartsDisplay : MonoBehaviour
    {
        [SerializeField] private Health playerHealth;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private GameObject[] heartIcons;
        [SerializeField] private float fadeInSeconds = 0.15f;
        [SerializeField] private float holdAfterCombatSeconds = 2f;
        [SerializeField] private float fadeOutSeconds = 0.6f;
        [SerializeField] private CanvasGroup screenEdgeRedness;

        private float _visibleUntil;

        private void OnEnable()
        {
            if (playerHealth == null) return;
            playerHealth.OnDamaged += HandleDamagedOrHealed;
            playerHealth.OnHealed += HandleDamagedOrHealed;
        }

        private void OnDisable()
        {
            if (playerHealth == null) return;
            playerHealth.OnDamaged -= HandleDamagedOrHealed;
            playerHealth.OnHealed -= HandleDamagedOrHealed;
        }

        private void HandleDamagedOrHealed(int _) => Reveal();

        /// Call from a combat tracker (or MonsterAI entering Chase/Attack
        /// against the player) to keep hearts up while a fight is on.
        public void NotifyEnemyEngaged() => Reveal();

        private void Reveal() => _visibleUntil = Time.time + holdAfterCombatSeconds;

        private void Update()
        {
            RefreshIcons();

            float target = Time.time < _visibleUntil ? 1f : 0f;
            if (group != null)
            {
                float seconds = target > group.alpha ? fadeInSeconds : fadeOutSeconds;
                float speed = seconds <= 0f ? 999f : 1f / seconds;
                group.alpha = Mathf.MoveTowards(group.alpha, target, speed * Time.deltaTime);
            }

            if (screenEdgeRedness != null && playerHealth != null)
                screenEdgeRedness.alpha = 1f - (float)playerHealth.CurrentHearts / playerHealth.MaxHearts;
        }

        private void RefreshIcons()
        {
            if (playerHealth == null || heartIcons == null) return;
            for (int i = 0; i < heartIcons.Length; i++)
                heartIcons[i].SetActive(i < playerHealth.CurrentHearts);
        }
    }
}

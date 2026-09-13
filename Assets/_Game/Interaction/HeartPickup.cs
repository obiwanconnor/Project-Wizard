using UnityEngine;
using WhereAreMyKeys.Combat;

namespace WhereAreMyKeys.Interaction
{
    /// <summary>
    /// Restores one heart, then removes itself (GDD section 7).
    /// </summary>
    public class HeartPickup : Interactable
    {
        [SerializeField] private int amount = 1;

        protected override void OnInteract(GameObject interactor)
        {
            var health = interactor.GetComponentInParent<Health>();
            if (health == null || !health.CanHeal) return;
            health.Heal(amount);
            gameObject.SetActive(false);
        }
    }
}

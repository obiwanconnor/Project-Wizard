using UnityEngine;

namespace WhereAreMyKeys.Interaction
{
    /// <summary>
    /// Tracks nearby <see cref="Interactable"/>s, keeps the closest one
    /// focused, and fires it on demand. <c>PlayerRig</c> calls
    /// <see cref="TryInteract"/> when the Input System's Interact action
    /// performs — this class knows nothing about Cainos or input bindings.
    ///
    /// ASSUMPTION: Physics2D (see README).
    /// </summary>
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private float range = 1.25f;
        [SerializeField] private LayerMask interactableMask = ~0;
        [SerializeField] private InteractionPromptView promptView;

        private readonly Collider2D[] _hits = new Collider2D[8];
        private Interactable _focused;

        private void Update()
        {
            Interactable nearest = FindNearest();
            if (nearest != _focused)
            {
                _focused = nearest;
                RefreshPrompt();
            }
            else if (_focused != null)
            {
                RefreshPrompt();
            }
        }

        private void RefreshPrompt()
        {
            if (promptView == null) return;
            if (_focused != null) promptView.Show(_focused.PromptPosition);
            else promptView.Hide();
        }

        private Interactable FindNearest()
        {
            int count = Physics2D.OverlapCircleNonAlloc(transform.position, range, _hits, interactableMask);
            Interactable best = null;
            float bestDist = float.MaxValue;
            for (int i = 0; i < count; i++)
            {
                var candidate = _hits[i].GetComponentInParent<Interactable>();
                if (candidate == null || !candidate.CanInteract) continue;
                float d = (candidate.transform.position - transform.position).sqrMagnitude;
                if (d < bestDist) { bestDist = d; best = candidate; }
            }
            return best;
        }

        /// Called by PlayerRig on the Interact input action.
        public void TryInteract() => _focused?.Interact(gameObject);
    }
}

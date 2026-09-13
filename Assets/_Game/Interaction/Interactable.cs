using UnityEngine;

namespace WhereAreMyKeys.Interaction
{
    /// <summary>
    /// Anything the wizard can search (GDD section 3: "everything can be
    /// searched"). No trigger or range logic lives here on purpose —
    /// <see cref="PlayerInteractor"/> owns detection and picks the nearest
    /// one, so ten overlapping chests don't run ten overlap checks.
    /// </summary>
    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField] private Vector3 promptOffset = new Vector3(0f, 1.1f, 0f);
        [SerializeField] private bool oneShot;

        /// True once this has fired and <see cref="oneShot"/> is set.
        public bool Consumed { get; private set; }

        public Vector3 PromptPosition => transform.position + promptOffset;

        public bool CanInteract => !(Consumed && oneShot);

        /// Called by <see cref="PlayerInteractor"/>, or directly by a
        /// spell bolt (see Projectile.cs) for things like an unlit candle.
        public void Interact(GameObject interactor)
        {
            if (!CanInteract) return;
            OnInteract(interactor);
            if (oneShot) Consumed = true;
        }

        protected abstract void OnInteract(GameObject interactor);
    }
}

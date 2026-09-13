using UnityEngine;
using WhereAreMyKeys.Combat;
using WhereAreMyKeys.Data;
using WhereAreMyKeys.GameState;
using WhereAreMyKeys.UI;

namespace WhereAreMyKeys.Interaction
{
    /// <summary>
    /// A searchable chest, barrel, crate or bone pile — same behaviour,
    /// different art (GDD section 7). Optionally holds a key, a heart, or
    /// is secretly the Mimic (GDD section 6).
    /// </summary>
    public class Chest : Interactable
    {
        public enum Contents { Nothing, Key, Heart }

        [Header("Contents")]
        [SerializeField] private Contents contents = Contents.Nothing;
        [SerializeField] private FlavourTable flavourTable;

        [Header("Mimic")]
        [Tooltip("If set, this 'chest' is the Mimic — searching it reveals the monster instead of opening.")]
        [SerializeField] private bool isMimic;
        [SerializeField] private GameObject mimicMonster;

        [Header("Presentation")]
        [SerializeField] private Animator animator;

        /// <summary>
        /// Fired when a real (non-Mimic) chest opens. <see cref="Rigs.ChestRig"/>
        /// listens for this to drive the sibling Cainos Chest component's
        /// visuals — kept as an event rather than a direct reference so this
        /// assembly still can't see Cainos types (see WhereAreMyKeys.asmdef).
        /// </summary>
        public event System.Action OnOpened;

        protected override void OnInteract(GameObject interactor)
        {
            if (isMimic)
            {
                RevealMimic();
                return;
            }

            animator?.SetTrigger("Open");
            OnOpened?.Invoke();

            if (flavourTable != null)
                MemoryLog.Instance?.ShowText(flavourTable.GetRandomLine());

            switch (contents)
            {
                case Contents.Key:
                    KeyRing.Instance?.AddKey();
                    break;
                case Contents.Heart:
                    interactor.GetComponentInParent<Health>()?.Heal(1);
                    break;
            }
        }

        private void RevealMimic()
        {
            animator?.SetTrigger("Reveal");
            if (mimicMonster != null) mimicMonster.SetActive(true);
            // The pack's Pixel Monster.Is Hiding flag is what makes the
            // disguise possible in the first place — see MonsterRig's
            // `startsHidden` for the Cainos-side wiring, since that field
            // is a Cainos type this assembly can't reference.
        }
    }
}

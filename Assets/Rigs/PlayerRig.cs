using UnityEngine;
using UnityEngine.InputSystem;
using WhereAreMyKeys.Combat;
using WhereAreMyKeys.Interaction;

namespace WhereAreMyKeys.Rigs
{
    /// <summary>
    /// The one file allowed to know both the Cainos character controller
    /// AND our game logic. Everything here is a guess against the vendor
    /// DOCS (field/event *names*, not real C# signatures) — fix the
    /// <c>// TODO</c>s against the real <c>PixelCharacterController.cs</c>
    /// once the character pack is imported, and nowhere else in the
    /// project needs to change. See README for the asmdef trick that
    /// makes this the only file that has to.
    ///
    /// ASSUMPTIONS: Physics2D, New Input System with Cast/Interact actions
    /// on a PlayerInput component — see README.
    /// </summary>
    public class PlayerRig : MonoBehaviour
    {
        [Header("Cainos references — fix after import")]
        // TODO: replace with the real controller type, e.g.
        //   [SerializeField] private PixelCharacterController controller;
        [SerializeField] private MonoBehaviour controllerPlaceholder;

        [Header("Our game logic")]
        [SerializeField] private SpellCaster spellCaster;
        [SerializeField] private Health health;
        [SerializeField] private PlayerInteractor interactor;
        [SerializeField] private PlayerInput playerInput;

        [Header("Presentation")]
        [SerializeField] private Renderer staffCrystal;
        [SerializeField] private Color crystalReadyColor = Color.cyan;
        [SerializeField] private Color crystalDimColor = new Color(0.2f, 0.4f, 0.4f);

        private InputAction _castAction;
        private InputAction _interactAction;

        private void Awake()
        {
            if (playerInput == null) return;
            // TODO: verify these action names match the Input Actions
            // asset. The character pack's own Cast/Melee actions may
            // already be wired for you — if so, this can read those
            // directly instead of owning separate ones.
            _castAction = playerInput.actions.FindAction("Cast");
            _interactAction = playerInput.actions.FindAction("Interact");
        }

        private void OnEnable()
        {
            if (_castAction != null) _castAction.performed += OnCastPerformed;
            if (_interactAction != null) _interactAction.performed += OnInteractPerformed;
            if (spellCaster != null)
            {
                spellCaster.OnCastPerformed += HandleSpellCastPerformed;
                spellCaster.OnReadyAgain += HandleSpellReady;
            }
        }

        private void OnDisable()
        {
            if (_castAction != null) _castAction.performed -= OnCastPerformed;
            if (_interactAction != null) _interactAction.performed -= OnInteractPerformed;
            if (spellCaster != null)
            {
                spellCaster.OnCastPerformed -= HandleSpellCastPerformed;
                spellCaster.OnReadyAgain -= HandleSpellReady;
            }
        }

        private void OnCastPerformed(InputAction.CallbackContext ctx)
        {
            // The cooldown gate lives in our SpellCaster, not the Cainos
            // controller. If it's not ready we simply never forward the
            // press — the controller's melee action stays available as
            // the natural fallback (GDD section 6).
            spellCaster?.TryCast();

            // TODO: once `controller` is real —
            //   controller.ProjectileSpeed = spellCaster.Spell.projectileSpeed;
            //   controller.ProjectilePrefab = <the underlying prefab
            //     matching whatever type Cast expects>;
            //   then either let the controller's own Cast action fire the
            //   shot, or call whatever public method triggers Cast
            //   directly if the action can't just be allowed through.
        }

        private void OnInteractPerformed(InputAction.CallbackContext ctx) => interactor?.TryInteract();

        private void HandleSpellCastPerformed()
        {
            if (staffCrystal != null) staffCrystal.material.color = crystalDimColor;
            // TODO: play spellCaster.Spell.castSound
        }

        private void HandleSpellReady()
        {
            if (staffCrystal != null) staffCrystal.material.color = crystalReadyColor;
        }

        // TODO: subscribe to the controller's documented `On Attack Hit`
        // UnityEvent (melee damage) here, e.g.:
        //
        //   private void HandleAttackHit(Collider2D hitTarget)
        //   {
        //       hitTarget.GetComponentInParent<Health>()?.TakeDamage(meleeDamage);
        //   }
        //
        // Wire it in the Inspector or in Awake once the event's real
        // signature is known — the docs only confirm the event exists,
        // not its parameters.

        // TODO: subscribe to the controller's footstep event
        // (`On Footstep`, per the docs) to drive footstep audio.
    }
}

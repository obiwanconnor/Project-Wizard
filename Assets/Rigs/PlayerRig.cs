using UnityEngine;
using UnityEngine.InputSystem;
using Cainos.CustomizablePixelCharacter;
using WhereAreMyKeys.Combat;
using WhereAreMyKeys.Interaction;

namespace WhereAreMyKeys.Rigs
{
    /// <summary>
    /// The one file allowed to know both the Cainos character controller
    /// AND our game logic. See README for the asmdef trick that makes this
    /// the only file that has to.
    ///
    /// ASSUMPTIONS: Physics2D, New Input System with Cast/Interact actions
    /// on a PlayerInput component (WhereAreMyKeys.inputactions) — see README.
    ///
    /// Movement/jump/dodge/melee stay driven by Cainos's own
    /// <see cref="PixelCharacterInputMouseAndKeyboard"/> — this file only
    /// takes over the primary attack ("Attack Action", set to Cast in the
    /// Inspector), so it can gate it on <see cref="SpellCaster"/>'s cooldown
    /// instead of firing on every held click. Because that input script
    /// also feeds <c>inputAttack</c> every single frame, its own Attack Key
    /// must be set to None in the Inspector so it always feeds false — this
    /// file then pulses <c>inputAttack</c> true for exactly one frame in
    /// <see cref="Update"/> whenever a cast is accepted.
    ///
    /// <see cref="PixelCharacterController"/> reads <c>inputAttack</c> from
    /// its OWN Update (AttackUpdate), not LateUpdate, so the pulse has to
    /// land inside the Update phase too, strictly after the Cainos input
    /// script's Update (which stomps <c>inputAttack</c> back to false every
    /// frame) and strictly before the controller's. Neither vendor script
    /// declares an execution order, so that ordering isn't guaranteed by
    /// default — hence the explicit <see cref="DefaultExecutionOrder"/>
    /// here plus the matching override on
    /// PixelCharacterInputMouseAndKeyboard's script asset (set to -100 in
    /// its .meta). Get this wrong and Cast silently does nothing: the
    /// pulse gets overwritten before the controller ever sees it.
    /// </summary>
    [DefaultExecutionOrder(-50)]
    public class PlayerRig : MonoBehaviour
    {
        [Header("Cainos references")]
        [SerializeField] private PixelCharacterController controller;
        [SerializeField] private PixelCharacter character;

        [Header("Our game logic")]
        [SerializeField] private SpellCaster spellCaster;
        [SerializeField] private Health health;
        [SerializeField] private PlayerInteractor interactor;
        [SerializeField] private PlayerInput playerInput;

        [Header("Melee hit detection — On Attack Hit names no target of its own")]
        [SerializeField] private LayerMask meleeHitMask = ~0;
        [SerializeField] private float meleeHitRadius = 0.6f;
        [SerializeField] private int meleeDamage = 1;

        [Header("Presentation")]
        [SerializeField] private Renderer staffCrystal;
        [SerializeField] private Color crystalReadyColor = Color.cyan;
        [SerializeField] private Color crystalDimColor = new Color(0.2f, 0.4f, 0.4f);

        private InputAction _castAction;
        private InputAction _interactAction;
        private bool _pulseAttackThisFrame;

        private void Awake()
        {
            if (playerInput == null) return;
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
            if (controller != null) controller.onAttackHit.AddListener(HandleMeleeHit);
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
            if (controller != null) controller.onAttackHit.RemoveListener(HandleMeleeHit);
        }

        private void OnCastPerformed(InputAction.CallbackContext ctx)
        {
            // The cooldown gate lives in our SpellCaster, not the Cainos
            // controller. Melee stays available as the natural fallback
            // (GDD section 6) because it's a separate key/action entirely
            // and isn't touched here — only the primary attack is.
            if (spellCaster != null && spellCaster.TryCast())
                _pulseAttackThisFrame = true;
        }

        private void Update()
        {
            if (controller == null) return;

            // Fires the Cast attack action for exactly the frame a cast was
            // accepted — see the class doc for why this has to land here,
            // ordered between the two vendor scripts, rather than in
            // LateUpdate.
            controller.inputAttack = _pulseAttackThisFrame;
            _pulseAttackThisFrame = false;

            // Keeps the controller's own death state in sync with ours.
            // Idempotent on the controller's side (a same-value set is a
            // no-op) and self-corrects on respawn without needing a
            // separate "revived" event from Health.
            if (health != null) controller.IsDead = health.IsDead;
        }

        private void OnInteractPerformed(InputAction.CallbackContext ctx) => interactor?.TryInteract();

        private void HandleSpellCastPerformed()
        {
            if (staffCrystal != null) staffCrystal.material.color = crystalDimColor;
            if (spellCaster?.Spell?.castSound != null)
                AudioSource.PlayClipAtPoint(spellCaster.Spell.castSound, transform.position);
        }

        private void HandleSpellReady()
        {
            if (staffCrystal != null) staffCrystal.material.color = crystalReadyColor;
        }

        /// <summary>
        /// The controller's On Attack Hit event fires on the animation's hit
        /// frame but carries no target of its own, so we do our own overlap
        /// check at the weapon's tip to find what it hit (GDD section 6: "we
        /// do not write attack timing" — only this).
        ///
        /// Skips its own hierarchy explicitly rather than relying solely on
        /// <see cref="meleeHitMask"/> — no Enemy layer exists to scope it to
        /// yet (see docs/project-setup-checklist.md), and a default
        /// "everything" mask would otherwise let the wizard hit itself.
        /// </summary>
        private void HandleMeleeHit()
        {
            if (character == null || character.Weapon == null) return;

            Vector3 tip = character.Weapon.TipPosition;
            var hits = Physics2D.OverlapCircleAll(tip, meleeHitRadius, meleeHitMask);
            foreach (var hit in hits)
            {
                if (hit.transform.IsChildOf(transform)) continue;
                hit.GetComponentInParent<Health>()?.TakeDamage(meleeDamage);
            }
        }

        // TODO (E8, audio): subscribe to controller.onFootstep to drive
        // footstep audio. Not part of getting the wizard into the scene.
    }
}

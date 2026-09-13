using UnityEngine;
using WhereAreMyKeys.AI;
using WhereAreMyKeys.Combat;

namespace WhereAreMyKeys.Rigs
{
    /// <summary>
    /// Same boundary as <see cref="PlayerRig"/>, for the monster side.
    /// Reads <see cref="MonsterAI"/>'s computed <see cref="MonsterIntent"/>
    /// every frame and pushes it into the real Cainos MonsterController —
    /// this is the only file that needs editing per enemy archetype once
    /// the pack is imported.
    /// </summary>
    public class MonsterRig : MonoBehaviour
    {
        [Header("Cainos references — fix after import")]
        // TODO: replace with the real type, e.g.
        //   [SerializeField] private MonsterController controller;
        [SerializeField] private MonoBehaviour controllerPlaceholder;

        [Header("Our game logic")]
        [SerializeField] private MonsterAI ai;
        [SerializeField] private Health health;

        [Header("Mimic only")]
        [Tooltip("Only meaningful on the Mimic — see Pixel Monster.Is Hiding in the docs.")]
        [SerializeField] private bool startsHidden;

        private void Start()
        {
            if (startsHidden)
            {
                // TODO: controller-equivalent of `Pixel Monster.Is Hiding = true`
                // — the docs confirm the field only does anything for the Mimic.
            }
        }

        private void Update()
        {
            if (ai == null) return;
            MonsterIntent intent = ai.CurrentIntent;

            // TODO: once `controller` is real, replace with the documented
            // fields — names are confirmed by the docs, exact C# types are
            // not:
            //   controller.InputMove = intent.moveDirection;
            //   controller.InputMoveModifier = ai.State == MonsterState.Chase ? 1f : 0.5f;
            //   controller.InputAttack = intent.wantsAttack;
            //   // Facing: some Cainos controllers infer facing from
            //   // InputMove.x's sign rather than a separate flag — check
            //   // the real API before adding one here.
        }

        private void OnEnable()
        {
            if (health != null) health.OnDied += HandleDied;
        }

        private void OnDisable()
        {
            if (health != null) health.OnDied -= HandleDied;
        }

        private void HandleDied()
        {
            // TODO: trigger the monster's Die animation via the real
            // controller rather than disabling immediately, so the clip plays.
            enabled = false;
        }

        // TODO: subscribe to the monster's own documented `On Attack`
        // animation event (this is what melee damage should key off — see
        // PlayerRig's equivalent TODO) to apply damage to the player's
        // Health on a successful swing.
    }
}

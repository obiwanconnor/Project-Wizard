using UnityEngine;
using Cainos.CustomizablePixelCharacter;

namespace WhereAreMyKeys.Rigs
{
    /// <summary>
    /// Same boundary as <see cref="ChestRig"/>, for the wizard's spell.
    /// Cainos's <see cref="Projectile"/> (fired directly by
    /// <see cref="PixelCharacterController"/>'s own Cast attack action) is
    /// purely visual/physical — it has no idea what our
    /// <see cref="Combat.Health"/> or <see cref="Interaction.Candle"/> are —
    /// so this bridges the two, mirroring what our own
    /// <c>WhereAreMyKeys.Combat.Projectile</c> does for damage and
    /// candle-lighting (GDD section 6: "the bolt can light an unlit candle
    /// from range").
    ///
    /// Lives on the projectile PREFAB itself — a duplicate of one of
    /// Cainos's "PF Projectile - Magic Missile" prefabs kept under our own
    /// _Game/Prefabs, never the vendor original — not on the wizard.
    ///
    /// Note: no <c>using WhereAreMyKeys.Combat;</c>/<c>Interaction;</c>
    /// here — both would bring in our own <c>Projectile</c> class, which
    /// collides with Cainos's <see cref="Projectile"/> above. Qualifying
    /// <c>Combat.Health</c>/<c>Interaction.Candle</c> below resolves via the
    /// enclosing <c>WhereAreMyKeys</c> namespace instead (same trick
    /// <see cref="ChestRig"/> uses for <c>Interaction.Chest</c>).
    ///
    /// KNOWN SIMPLIFICATION: damage/canLightCandles are plain fields here
    /// rather than read from <c>SpellDefinition</c>, because the Cainos
    /// controller instantiates <c>projectilePrefab</c> itself with no way
    /// for us to pass data in at spawn time. Keep this in sync with the
    /// SpellDefinition asset by hand until a real need to unify them shows up.
    /// </summary>
    [RequireComponent(typeof(Projectile))]
    public class SpellProjectileRig : MonoBehaviour
    {
        [SerializeField] private int damage = 1;
        [SerializeField] private bool canLightCandles = true;

        private Projectile _projectile;

        private void Awake() => _projectile = GetComponent<Projectile>();

        private void OnEnable() => _projectile.onHit.AddListener(HandleHit);

        private void OnDisable() => _projectile.onHit.RemoveListener(HandleHit);

        private void HandleHit(Collision2D collision)
        {
            var other = collision.collider;

            var health = other.GetComponentInParent<Combat.Health>();
            health?.TakeDamage(damage);

            if (canLightCandles)
            {
                var candle = other.GetComponentInParent<Interaction.Candle>();
                if (candle != null && !candle.IsLit)
                    candle.Interact(gameObject);
            }
        }
    }
}

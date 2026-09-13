using UnityEngine;
using WhereAreMyKeys.Data;
using WhereAreMyKeys.GameState;
using WhereAreMyKeys.UI;

namespace WhereAreMyKeys.Interaction
{
    /// <summary>
    /// Lights, becomes a light source, fires a memory, and sets the
    /// checkpoint — one interaction, three payoffs (GDD section 4).
    ///
    /// This class doesn't care how it was lit: <see cref="Interact"/> is
    /// called either by <see cref="PlayerInteractor"/> on direct search,
    /// or by a magic bolt on hit (see Combat/Projectile.cs) — "lighting
    /// them is the same spell he attacks with."
    ///
    /// ASSUMPTION: <see cref="UnityEngine.Light"/> (a 3D light on a 2D
    /// sprite), matching the locked "URP 3D lighting" decision — not a
    /// URP 2D Light2D. See README.
    /// </summary>
    public class Candle : Interactable
    {
        [SerializeField] private CandleMemory memory;
        [SerializeField] private Light lightSource;
        [SerializeField] private ParticleSystem lightFlare;
        [SerializeField] private float litIntensity = 1.2f;
        [SerializeField] private float litRange = 4f;

        public bool IsLit { get; private set; }

        protected override void OnInteract(GameObject interactor)
        {
            if (IsLit) return;
            IsLit = true;

            if (lightSource != null)
            {
                lightSource.enabled = true;
                lightSource.intensity = litIntensity;
                lightSource.range = litRange;
            }
            lightFlare?.Play();

            if (memory != null)
                MemoryLog.Instance?.ShowMemory(memory);

            CheckpointManager.Instance?.SetCheckpoint(transform.position);
        }
    }
}

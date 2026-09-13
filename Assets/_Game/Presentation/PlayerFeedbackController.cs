using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using WhereAreMyKeys.Combat;

namespace WhereAreMyKeys.Presentation
{
    /// <summary>
    /// Health and magic, shown as light instead of a hearts/mana HUD (GDD's
    /// "as little on-screen as possible" call). Reads the public state
    /// <see cref="Health"/> and <see cref="SpellCaster"/> already expose —
    /// no new events needed on either.
    ///
    /// Damage darkens the edges of the screen (Vignette). Casting cools and
    /// drains the screen (ColorAdjustments) and dims the wizard's own light,
    /// both recovering as the spell comes off cooldown.
    ///
    /// Own Volume + profile, not Cainos's Post Processing Profile — this
    /// blends over it via priority rather than editing the vendor asset.
    /// </summary>
    public class PlayerFeedbackController : MonoBehaviour
    {
        [Header("Sources")]
        [SerializeField] private Health playerHealth;
        [SerializeField] private SpellCaster playerSpellCaster;

        [Header("Our feedback volume — assign the Volume holding PlayerFeedbackProfile")]
        [SerializeField] private Volume feedbackVolume;

        [Header("Damage → vignette")]
        [SerializeField] private float minVignetteIntensity = 0f;
        [SerializeField] private float maxVignetteIntensity = 0.55f;

        [Header("Casting → desaturate")]
        [SerializeField] private float minSaturation = 0f;
        [SerializeField] private float maxSaturationDrop = -70f;

        [Header("Casting → cool colour filter")]
        [SerializeField] private Color readyColorFilter = Color.white;
        [SerializeField] private Color depletedColorFilter = new Color(0.65f, 0.75f, 1f);

        [Header("Casting → wizard's own light (assign once the character prefab exists)")]
        [SerializeField] private Light wizardLight;
        [SerializeField] private float wizardLightMaxIntensity = 1f;
        [SerializeField] private float wizardLightMinIntensity = 0.35f;

        [Header("Smoothing")]
        [SerializeField] private float lerpSpeed = 4f;

        private Vignette _vignette;
        private ColorAdjustments _colorAdjustments;

        private void Awake()
        {
            if (feedbackVolume == null || feedbackVolume.profile == null)
            {
                Debug.LogError($"{name}: PlayerFeedbackController needs a Volume with PlayerFeedbackProfile assigned.", this);
                enabled = false;
                return;
            }

            feedbackVolume.profile.TryGet(out _vignette);
            feedbackVolume.profile.TryGet(out _colorAdjustments);
        }

        private void Update()
        {
            float healthT = HealthDepletedFraction();
            float magicT = playerSpellCaster != null ? playerSpellCaster.NormalizedRemaining : 0f;
            float dt = Time.deltaTime * lerpSpeed;

            if (_vignette != null)
            {
                float target = Mathf.Lerp(minVignetteIntensity, maxVignetteIntensity, healthT);
                _vignette.intensity.value = Mathf.Lerp(_vignette.intensity.value, target, dt);
            }

            if (_colorAdjustments != null)
            {
                float satTarget = Mathf.Lerp(minSaturation, maxSaturationDrop, magicT);
                _colorAdjustments.saturation.value = Mathf.Lerp(_colorAdjustments.saturation.value, satTarget, dt);

                Color colorTarget = Color.Lerp(readyColorFilter, depletedColorFilter, magicT);
                _colorAdjustments.colorFilter.value = Color.Lerp(_colorAdjustments.colorFilter.value, colorTarget, dt);
            }

            if (wizardLight != null)
            {
                float lightTarget = Mathf.Lerp(wizardLightMaxIntensity, wizardLightMinIntensity, magicT);
                wizardLight.intensity = Mathf.Lerp(wizardLight.intensity, lightTarget, dt);
            }
        }

        private float HealthDepletedFraction()
        {
            if (playerHealth == null || playerHealth.MaxHearts <= 0) return 0f;
            return 1f - (playerHealth.CurrentHearts / (float)playerHealth.MaxHearts);
        }
    }
}

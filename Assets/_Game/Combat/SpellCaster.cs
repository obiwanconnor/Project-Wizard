using System;
using UnityEngine;
using WhereAreMyKeys.Data;

namespace WhereAreMyKeys.Combat
{
    /// <summary>
    /// Pure cooldown/gating logic — no MonoBehaviour, no Unity lifecycle —
    /// so it's covered by EditMode tests without a scene.
    /// <see cref="SpellCaster"/> below just ticks this and wires it into
    /// the Input System.
    /// </summary>
    public class SpellCooldownState
    {
        private readonly SpellDefinition _spell;
        private float _remaining;

        public SpellCooldownState(SpellDefinition spell) => _spell = spell;

        public bool IsReady => _remaining <= 0f;
        public float RemainingSeconds => Mathf.Max(0f, _remaining);

        public float NormalizedRemaining =>
            _spell == null || _spell.cooldownSeconds <= 0f ? 0f : Mathf.Clamp01(_remaining / _spell.cooldownSeconds);

        public void Tick(float deltaTime)
        {
            if (_remaining > 0f) _remaining -= deltaTime;
        }

        /// Returns true and starts the cooldown if the spell was ready.
        public bool TryCast()
        {
            if (!IsReady) return false;
            _remaining = _spell.cooldownSeconds;
            return true;
        }
    }

    /// <summary>
    /// Ticks the cooldown every frame and exposes the crystal-glow signal
    /// the GDD calls for instead of a mana bar (section 6). <c>PlayerRig</c>
    /// calls <see cref="TryCast"/> when the Input System's Cast action
    /// performs — this class knows nothing about the Cainos controller.
    /// </summary>
    public class SpellCaster : MonoBehaviour
    {
        [SerializeField] private SpellDefinition spell;

        public SpellDefinition Spell => spell;
        public bool IsReady => _state != null && _state.IsReady;
        public float NormalizedRemaining => _state?.NormalizedRemaining ?? 0f;

        /// Fired the instant a cast is accepted — glow dims, sound plays.
        public event Action OnCastPerformed;
        /// Fired the frame the cooldown clears — crystal glows back up.
        public event Action OnReadyAgain;

        private SpellCooldownState _state;
        private bool _wasReadyLastFrame;

        private void Awake()
        {
            if (spell == null)
                Debug.LogError($"{name}: SpellCaster has no SpellDefinition assigned.", this);
            _state = new SpellCooldownState(spell);
            _wasReadyLastFrame = true;
        }

        private void Update()
        {
            _state.Tick(Time.deltaTime);
            if (_state.IsReady && !_wasReadyLastFrame)
                OnReadyAgain?.Invoke();
            _wasReadyLastFrame = _state.IsReady;
        }

        /// Called by PlayerRig on the Cast input action.
        public bool TryCast()
        {
            bool cast = _state.TryCast();
            if (cast) OnCastPerformed?.Invoke();
            return cast;
        }
    }
}

using System;
using UnityEngine;

namespace WhereAreMyKeys.Combat
{
    /// <summary>
    /// Pure damage/heal/invulnerability/death logic — no MonoBehaviour, no
    /// Unity lifecycle — so it's covered by EditMode tests without a
    /// scene. <see cref="Health"/> below just wraps this and fires the
    /// Unity-side events off its transitions.
    /// </summary>
    public class HealthState
    {
        public int MaxHearts { get; }
        public int CurrentHearts { get; private set; }
        public bool CanHeal => CurrentHearts < MaxHearts;
        public bool IsDead { get; private set; }

        private readonly float _invulnerabilitySeconds;
        private float _invulnerableUntil;

        public HealthState(int maxHearts, float invulnerabilitySeconds)
        {
            MaxHearts = maxHearts;
            _invulnerabilitySeconds = invulnerabilitySeconds;
            CurrentHearts = maxHearts;
        }

        /// Returns true if the damage was applied (false if blocked by
        /// death, a non-positive amount, or the invulnerability window).
        public bool TakeDamage(int amount, float currentTime)
        {
            if (IsDead || amount <= 0 || currentTime < _invulnerableUntil) return false;

            CurrentHearts = Mathf.Max(0, CurrentHearts - amount);
            _invulnerableUntil = currentTime + _invulnerabilitySeconds;
            if (CurrentHearts == 0) IsDead = true;
            return true;
        }

        /// Returns true if the heal was applied (false if blocked by
        /// death, a non-positive amount, or already at full hearts).
        public bool Heal(int amount)
        {
            if (IsDead || amount <= 0 || !CanHeal) return false;
            CurrentHearts = Mathf.Min(MaxHearts, CurrentHearts + amount);
            return true;
        }

        public void ResetToFull()
        {
            IsDead = false;
            CurrentHearts = MaxHearts;
        }
    }

    /// <summary>
    /// Generic damageable — used by the wizard and every monster. The
    /// hearts UI reads <see cref="CurrentHearts"/>/<see cref="MaxHearts"/>;
    /// this class owns no presentation of its own.
    /// </summary>
    public class Health : MonoBehaviour
    {
        [SerializeField] private int maxHearts = 3;
        [SerializeField] private float invulnerabilitySeconds = 0.6f;

        public int MaxHearts => maxHearts;
        public int CurrentHearts => _state?.CurrentHearts ?? 0;
        public bool CanHeal => CurrentHearts < maxHearts;
        public bool IsDead => _state != null && _state.IsDead;

        public event Action<int> OnDamaged;
        public event Action<int> OnHealed;
        public event Action OnDied;

        private HealthState _state;

        private void Awake() => _state = new HealthState(maxHearts, invulnerabilitySeconds);

        public void TakeDamage(int amount)
        {
            if (!_state.TakeDamage(amount, Time.time)) return;
            OnDamaged?.Invoke(amount);
            if (_state.IsDead) OnDied?.Invoke();
        }

        public void Heal(int amount)
        {
            if (_state.Heal(amount)) OnHealed?.Invoke(amount);
        }

        /// Used by GameFlowController on respawn — full heal, clears death.
        public void ResetToFull() => _state.ResetToFull();
    }
}

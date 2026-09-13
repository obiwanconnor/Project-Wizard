using System;
using UnityEngine;

namespace WhereAreMyKeys.Combat
{
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
        public int CurrentHearts { get; private set; }
        public bool CanHeal => CurrentHearts < maxHearts;
        public bool IsDead { get; private set; }

        public event Action<int> OnDamaged;
        public event Action<int> OnHealed;
        public event Action OnDied;

        private float _invulnerableUntil;

        private void Awake() => CurrentHearts = maxHearts;

        public void TakeDamage(int amount)
        {
            if (IsDead || amount <= 0) return;
            if (Time.time < _invulnerableUntil) return;

            CurrentHearts = Mathf.Max(0, CurrentHearts - amount);
            _invulnerableUntil = Time.time + invulnerabilitySeconds;
            OnDamaged?.Invoke(amount);

            if (CurrentHearts == 0)
            {
                IsDead = true;
                OnDied?.Invoke();
            }
        }

        public void Heal(int amount)
        {
            if (IsDead || amount <= 0 || !CanHeal) return;
            CurrentHearts = Mathf.Min(maxHearts, CurrentHearts + amount);
            OnHealed?.Invoke(amount);
        }

        /// Used by GameFlowController on respawn — full heal, clears death.
        public void ResetToFull()
        {
            IsDead = false;
            CurrentHearts = maxHearts;
        }
    }
}

using System;
using UnityEngine;

namespace WhereAreMyKeys.GameState
{
    /// <summary>
    /// Tracks the three keys (GDD section 5). Plain state class first,
    /// MonoBehaviour singleton wrapper second, so the counting logic is
    /// EditMode-testable without a scene.
    /// </summary>
    public class KeyRingState
    {
        public int TotalKeys { get; }
        public int KeysCollected { get; private set; }
        public bool HasAllKeys => KeysCollected >= TotalKeys;

        public KeyRingState(int totalKeys) => TotalKeys = totalKeys;

        public int AddKey()
        {
            KeysCollected = Mathf.Min(TotalKeys, KeysCollected + 1);
            return KeysCollected;
        }
    }

    public class KeyRing : MonoBehaviour
    {
        public static KeyRing Instance { get; private set; }

        [SerializeField] private int totalKeys = 3;

        public event Action<int> OnKeyCountChanged;

        public int KeysCollected => _state.KeysCollected;
        public bool HasAllKeys => _state.HasAllKeys;

        private KeyRingState _state;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            _state = new KeyRingState(totalKeys);
        }

        public void AddKey()
        {
            int count = _state.AddKey();
            OnKeyCountChanged?.Invoke(count);
        }
    }
}

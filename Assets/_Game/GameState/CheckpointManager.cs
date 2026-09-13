using UnityEngine;

namespace WhereAreMyKeys.GameState
{
    /// <summary>
    /// Tracks the last lit candle. Death respawns here — no other state is
    /// touched, because there is no lose condition (GDD section 2).
    /// </summary>
    public class CheckpointManager : MonoBehaviour
    {
        public static CheckpointManager Instance { get; private set; }

        public Vector3 CurrentCheckpoint { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            CurrentCheckpoint = transform.position; // fallback: this object's own spawn point
        }

        public void SetCheckpoint(Vector3 worldPosition) => CurrentCheckpoint = worldPosition;
    }
}

using System;
using UnityEngine;
using WhereAreMyKeys.Combat;

namespace WhereAreMyKeys.GameState
{
    /// <summary>
    /// Win state and respawn-on-death (GDD section 2: no lose condition,
    /// just a return to the last candle). Deliberately owns no game-over
    /// path — there isn't one.
    /// </summary>
    public class GameFlowController : MonoBehaviour
    {
        public static GameFlowController Instance { get; private set; }

        [SerializeField] private GameObject winScreen;
        [SerializeField] private Health playerHealth;
        [SerializeField] private Transform playerTransform;

        public event Action OnWin;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void OnEnable()
        {
            if (playerHealth != null) playerHealth.OnDied += HandlePlayerDied;
        }

        private void OnDisable()
        {
            if (playerHealth != null) playerHealth.OnDied -= HandlePlayerDied;
        }

        public void Win()
        {
            if (winScreen != null) winScreen.SetActive(true);
            OnWin?.Invoke();
        }

        private void HandlePlayerDied()
        {
            if (CheckpointManager.Instance == null || playerTransform == null) return;
            playerTransform.position = CheckpointManager.Instance.CurrentCheckpoint;
            playerHealth.ResetToFull();
        }
    }
}

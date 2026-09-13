using UnityEngine;
using WhereAreMyKeys.Data;

namespace WhereAreMyKeys.UI
{
    /// <summary>
    /// Displays a candle memory, or any flavour line, as text over a
    /// dimmed screen (GDD section 4). Presentation timing (fade in/out) is
    /// left to the Inspector — this just owns the "show this text" entry
    /// point so Candle/Chest/Sign don't each need their own UI reference.
    ///
    /// ASSUMPTION: TextMeshPro — see README for the legacy Text swap.
    /// </summary>
    public class MemoryLog : MonoBehaviour
    {
        public static MemoryLog Instance { get; private set; }

        [SerializeField] private CanvasGroup dimOverlay;
        [SerializeField] private TMPro.TMP_Text memoryText;
        [SerializeField] private float displaySeconds = 3.5f;

        private float _hideAt;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            if (dimOverlay != null) dimOverlay.alpha = 0f;
        }

        public void ShowMemory(CandleMemory memory) => ShowText(memory != null ? memory.text : string.Empty);

        public void ShowText(string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            if (memoryText != null) memoryText.text = text;
            if (dimOverlay != null) dimOverlay.alpha = 1f;
            _hideAt = Time.time + displaySeconds;
            enabled = true;
        }

        private void Update()
        {
            if (Time.time < _hideAt) return;
            if (dimOverlay != null) dimOverlay.alpha = 0f;
            enabled = false;
        }
    }
}

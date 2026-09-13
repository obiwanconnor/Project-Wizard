using UnityEngine;

namespace WhereAreMyKeys.Interaction
{
    /// <summary>
    /// The single floating "you can search this" icon (GDD section 8: no
    /// text, no button legend). One instance is reused and repositioned
    /// rather than spawned per object.
    /// </summary>
    public class InteractionPromptView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer icon;
        [SerializeField] private float bobAmplitude = 0.06f;
        [SerializeField] private float bobSpeed = 2.5f;

        private Vector3 _anchor;
        private bool _visible;

        private void Awake() => Hide();

        public void Show(Vector3 worldPosition)
        {
            _anchor = worldPosition;
            if (!_visible && icon != null) icon.enabled = true;
            _visible = true;
        }

        public void Hide()
        {
            if (!_visible) return;
            _visible = false;
            if (icon != null) icon.enabled = false;
        }

        private void Update()
        {
            if (!_visible) return;
            float bob = Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
            transform.position = _anchor + Vector3.up * bob;
        }
    }
}

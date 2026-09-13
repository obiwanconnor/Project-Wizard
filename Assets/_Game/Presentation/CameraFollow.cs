using UnityEngine;

namespace WhereAreMyKeys.Presentation
{
    /// <summary>
    /// Follows a target (the player) on X/Y, smoothed with SmoothDamp so it
    /// settles rather than snapping. Two things are opt-in and both default
    /// off so a bare drop-in just follows:
    ///
    /// - Dead zone: the target can move this far from screen centre before
    ///   the camera reacts at all — cuts the small jitter a pixel-art camera
    ///   otherwise shows on every idle wobble. Leave at (0,0) to always
    ///   track the target directly.
    /// - Bounds: clamps the camera so it never shows past a room's edge.
    ///   Only meaningful for an orthographic camera; leave <see cref="useBounds"/>
    ///   off until level geometry has real edges to clamp to.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private GameObject target;
        [Tooltip("Added to the target's position. Z is the camera's fixed depth from the target.")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

        [Header("Smoothing")]
        [SerializeField] private float smoothTime = 0.15f;

        [Header("Dead zone — target can drift this far from centre before the camera follows")]
        [SerializeField] private Vector2 deadZoneSize = Vector2.zero;

        [Header("Bounds — world-space min/max the camera view is clamped inside")]
        [SerializeField] private bool useBounds = false;
        [SerializeField] private Vector2 minBounds;
        [SerializeField] private Vector2 maxBounds;

        private Camera _camera;
        private Transform _targetTransform;
        private Vector3 _velocity;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _targetTransform = target != null ? target.transform : null;
            if (_targetTransform == null)
                Debug.LogWarning($"{name}: CameraFollow has no target assigned.", this);
        }

        private void LateUpdate()
        {
            if (_targetTransform == null) return;

            Vector2 currentXY = transform.position;
            Vector2 desiredXY = (Vector2)_targetTransform.position + (Vector2)offset;
            Vector2 diff = desiredXY - currentXY;

            // Only pull the camera by however far the target has strayed
            // outside the dead zone, not the full distance to it.
            float halfWidth = deadZoneSize.x * 0.5f;
            float halfHeight = deadZoneSize.y * 0.5f;
            float pullX = Mathf.Max(0f, Mathf.Abs(diff.x) - halfWidth) * Mathf.Sign(diff.x);
            float pullY = Mathf.Max(0f, Mathf.Abs(diff.y) - halfHeight) * Mathf.Sign(diff.y);

            Vector3 desired = new Vector3(
                currentXY.x + pullX,
                currentXY.y + pullY,
                _targetTransform.position.z + offset.z);

            Vector3 next = Vector3.SmoothDamp(transform.position, desired, ref _velocity, smoothTime);

            if (useBounds && _camera.orthographic)
                next = ClampToBounds(next);

            transform.position = next;
        }

        private Vector3 ClampToBounds(Vector3 position)
        {
            float halfHeight = _camera.orthographicSize;
            float halfWidth = halfHeight * _camera.aspect;

            // If the room is narrower than the viewport, centre on it
            // rather than clamping to an inverted (min > max) range.
            float minX = minBounds.x + halfWidth;
            float maxX = maxBounds.x - halfWidth;
            float minY = minBounds.y + halfHeight;
            float maxY = maxBounds.y - halfHeight;

            position.x = minX <= maxX ? Mathf.Clamp(position.x, minX, maxX) : (minBounds.x + maxBounds.x) * 0.5f;
            position.y = minY <= maxY ? Mathf.Clamp(position.y, minY, maxY) : (minBounds.y + maxBounds.y) * 0.5f;
            return position;
        }
    }
}

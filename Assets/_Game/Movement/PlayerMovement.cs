using UnityEngine;

namespace WhereAreMyKeys.Movement
{
    /// <summary>
    /// Pure move-input-to-position logic — no MonoBehaviour, no Unity
    /// lifecycle — so it's covered by EditMode tests without a scene.
    /// <see cref="PlayerMovement"/> below just ticks this and wires it into
    /// the Input System.
    ///
    /// This is a grey-box stand-in for the Cainos character controller's
    /// own movement (see docs/DELIVERY-PLAN.md M1) — enough to get a block
    /// walking around the screen before that pack is imported and
    /// <c>PlayerRig</c> is pointed at the real thing instead.
    /// </summary>
    public class PlayerMoveState
    {
        private readonly float _speed;

        public Vector2 Position { get; private set; }

        public PlayerMoveState(float speed, Vector2 startPosition = default)
        {
            _speed = speed;
            Position = startPosition;
        }

        /// Advances Position toward rawInput's direction at up to _speed
        /// units/sec. rawInput is clamped to unit length first, so a
        /// keyboard composite's diagonal (length up to sqrt(2)) never
        /// moves faster than a single axis, while an analog gamepad
        /// stick pushed only halfway still moves at half speed.
        public void Tick(Vector2 rawInput, float deltaTime)
        {
            Vector2 direction = Vector2.ClampMagnitude(rawInput, 1f);
            Position += direction * _speed * deltaTime;
        }
    }

    /// <summary>
    /// Moves the block toward whatever direction <see cref="SetMoveInput"/>
    /// was last given, every frame. <c>PlayerRig</c> calls
    /// <see cref="SetMoveInput"/> from the Input System's Move action
    /// (Vector2 — arrow keys/WASD or a gamepad left stick, per
    /// InputSystem_Actions.inputactions) — this class knows nothing about
    /// input bindings.
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 4f;

        private PlayerMoveState _state;
        private Vector2 _moveInput;

        public Vector2 Position => _state != null ? _state.Position : (Vector2)transform.position;

        private void Awake()
        {
            _state = new PlayerMoveState(moveSpeed, transform.position);
        }

        private void Update()
        {
            _state.Tick(_moveInput, Time.deltaTime);
            Vector3 pos = transform.position;
            pos.x = _state.Position.x;
            pos.y = _state.Position.y;
            transform.position = pos;
        }

        /// Called by PlayerRig every frame with the current Move action value.
        public void SetMoveInput(Vector2 input) => _moveInput = input;
    }
}

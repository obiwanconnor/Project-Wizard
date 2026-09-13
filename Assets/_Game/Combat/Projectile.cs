using UnityEngine;
using WhereAreMyKeys.Interaction;

namespace WhereAreMyKeys.Combat
{
    /// <summary>
    /// The magic bolt (GDD sections 6 and 10). Travels, collides, damages,
    /// despawns — and can light an unlit Candle at range.
    ///
    /// ASSUMPTION: Unity 6 Rigidbody2D API (<c>linearVelocity</c>, the
    /// renamed <c>velocity</c>) and Physics2D generally — see README.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float maxDistance = 8f;
        [SerializeField] private LayerMask hitMask = ~0;

        private int _damage;
        private Vector3 _start;
        private Rigidbody2D _rb;

        public void Launch(Vector2 direction, float speed, int damage)
        {
            _damage = damage;
            _start = transform.position;
            _rb = GetComponent<Rigidbody2D>();
            _rb.linearVelocity = direction.normalized * speed;
        }

        private void Update()
        {
            if ((transform.position - _start).sqrMagnitude > maxDistance * maxDistance)
                Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & hitMask) == 0) return;

            var health = other.GetComponentInParent<Health>();
            health?.TakeDamage(_damage);

            var candle = other.GetComponentInParent<Candle>();
            if (candle != null && !candle.IsLit)
                candle.Interact(gameObject);

            if (health != null || candle != null)
                Destroy(gameObject);
        }
    }
}

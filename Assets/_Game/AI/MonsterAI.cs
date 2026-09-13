using UnityEngine;
using WhereAreMyKeys.Combat;
using WhereAreMyKeys.Data;

namespace WhereAreMyKeys.AI
{
    public enum MonsterState { Idle, Patrol, Chase, Attack, Dead }

    /// <summary>
    /// What <c>MonsterRig</c> should do this frame, computed without any
    /// reference to the Cainos MonsterController. MonsterRig reads this
    /// every frame and pushes it into the real Input Move / Input Attack
    /// fields (GDD section 6: "AI can't live on a shared base prefab").
    /// </summary>
    public struct MonsterIntent
    {
        public Vector2 moveDirection;
        public bool wantsAttack;
        public bool facingRight;
    }

    /// <summary>
    /// Idle → Patrol → Chase → Attack → Dead (GDD section 6). Pure
    /// transition logic, no MonoBehaviour, so it's EditMode-testable by
    /// constructing it directly against an <see cref="EnemyBehaviourProfile"/>.
    /// </summary>
    public class MonsterStateMachine
    {
        private readonly EnemyBehaviourProfile _profile;
        public MonsterState State { get; private set; } = MonsterState.Idle;

        private float _attackWindupTimer;
        private float _attackCooldownTimer;

        public MonsterStateMachine(EnemyBehaviourProfile profile) => _profile = profile;

        /// distanceToPlayer: pass float.PositiveInfinity if not sensed at all.
        public MonsterIntent Tick(float deltaTime, float distanceToPlayer, Vector2 towardPlayer, bool isDead)
        {
            if (isDead) State = MonsterState.Dead;
            if (_attackCooldownTimer > 0f) _attackCooldownTimer -= deltaTime;

            switch (State)
            {
                case MonsterState.Dead:
                    return new MonsterIntent();

                case MonsterState.Idle:
                case MonsterState.Patrol:
                    if (distanceToPlayer <= _profile.noticeRadius)
                        State = MonsterState.Chase;
                    break;

                case MonsterState.Chase:
                    if (distanceToPlayer > _profile.loseInterestRadius)
                        State = MonsterState.Patrol;
                    else if (distanceToPlayer <= _profile.attackRange && _attackCooldownTimer <= 0f)
                    {
                        State = MonsterState.Attack;
                        _attackWindupTimer = _profile.attackWindup;
                    }
                    break;

                case MonsterState.Attack:
                    _attackWindupTimer -= deltaTime;
                    if (_attackWindupTimer <= 0f)
                    {
                        _attackCooldownTimer = _profile.attackCooldown;
                        State = distanceToPlayer <= _profile.loseInterestRadius ? MonsterState.Chase : MonsterState.Patrol;
                    }
                    break;
            }

            bool committedToSwing = State == MonsterState.Attack && !_profile.canAttackWhileMoving;
            return new MonsterIntent
            {
                moveDirection = (State == MonsterState.Chase && !committedToSwing) ? towardPlayer.normalized : Vector2.zero,
                wantsAttack = State == MonsterState.Attack && _attackWindupTimer <= 0.01f,
                facingRight = towardPlayer.x >= 0f,
            };
        }
    }

    /// <summary>
    /// Senses the player each frame and drives a <see cref="MonsterStateMachine"/>.
    /// <c>MonsterRig</c> reads <see cref="CurrentIntent"/> and <see cref="State"/>
    /// to command the real Cainos controller — this class never references it.
    /// </summary>
    public class MonsterAI : MonoBehaviour
    {
        [SerializeField] private EnemyBehaviourProfile profile;
        [SerializeField] private Transform player;

        public MonsterIntent CurrentIntent { get; private set; }
        public MonsterState State => _fsm?.State ?? MonsterState.Idle;

        private MonsterStateMachine _fsm;
        private Health _health;

        private void Awake()
        {
            _fsm = new MonsterStateMachine(profile);
            _health = GetComponent<Health>();
            if (player == null)
            {
                var tagged = GameObject.FindGameObjectWithTag("Player");
                if (tagged != null) player = tagged.transform;
            }
        }

        private void Update()
        {
            if (player == null) return;

            Vector2 toPlayer = player.position - transform.position;
            float distance = toPlayer.magnitude;
            bool isDead = _health != null && _health.IsDead;

            CurrentIntent = _fsm.Tick(Time.deltaTime, distance, toPlayer, isDead);
        }
    }
}

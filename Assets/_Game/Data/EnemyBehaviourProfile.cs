using UnityEngine;

namespace WhereAreMyKeys.Data
{
    /// <summary>
    /// Drops onto any monster to define its archetype (GDD section 6).
    /// Because the roster isn't one shared prefab — Spider, Zombie and Orc
    /// are different bodies — the tuning has to live on a droppable
    /// component/profile, not baked per-prefab.
    /// </summary>
    [CreateAssetMenu(menuName = "Where Are My Keys/Enemy Behaviour Profile", fileName = "EnemyBehaviourProfile")]
    public class EnemyBehaviourProfile : ScriptableObject
    {
        [Header("Senses")]
        public float noticeRadius = 5f;
        public float attackRange = 1f;
        public float loseInterestRadius = 8f;

        [Header("Movement")]
        public float moveSpeed = 2f;

        [Header("Attack")]
        public float attackWindup = 0.3f;
        public float attackCooldown = 1.2f;
        public int damage = 1;

        [Tooltip("Off = commits to its swing and can't move while attacking — the Orc's fairness rule (GDD section 6): a player who reads the wind-up can walk around it.")]
        public bool canAttackWhileMoving = true;

        [Header("Health")]
        public int maxHealth = 3;
    }
}

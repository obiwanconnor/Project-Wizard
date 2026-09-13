using UnityEngine;
using WhereAreMyKeys.Combat;

namespace WhereAreMyKeys.Data
{
    /// <summary>
    /// A spell as data — projectile, cooldown, damage, cast sound — so a
    /// second spell later is a new asset, not new code (GDD section 10,
    /// "headroom worth protecting").
    /// </summary>
    [CreateAssetMenu(menuName = "Where Are My Keys/Spell Definition", fileName = "SpellDefinition")]
    public class SpellDefinition : ScriptableObject
    {
        public string spellName = "Bolt";
        public Projectile projectilePrefab;
        public float projectileSpeed = 9f;
        public float cooldownSeconds = 3f;
        public int damage = 1;
        public bool canLightCandles = true;
        public AudioClip castSound;
    }
}
